using HotChocolate.Resolvers;
using System.Text.RegularExpressions;
using OfficeOpenXml;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// 工事情報
/// </summary>
public partial class Job
{
    // Id: "開始日-"会社名-工事名"
    [Key]
    public string Id
    {
        get => string.Join("-", Start.Id, Company.Id, _nameId);
        set { }
    }

    public Fullpath Folder { get; set; } = new();

    public string? RecomFolder
    {
        get
        {
            if (Start.IsZero())
            {
                return null;
            }
            string recomFilename = string.Join(" ", Start.ToFileString(), Company.Name, Name);
            return System.IO.Path.Combine(Folder.Parent, recomFilename);
        }
    }


    public Date Start { get; set; } = new();

    public Date Finish { get; set; } = new();

    public Company Company { get; set; } = new();

    private string _nameId = Res.ZeroId;
    private string _name = string.Empty;
    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            _nameId = Lib.Id(value);
        }
    }

    public Fullpath JobFile { get; set; } = new();

    // Folderの値を返す
    public Date CreatedAt => Folder.CreatedAt;
    public Date UpdatedAt => Folder.UpdatedAt;
    public Date DeletedAt => Folder.DeletedAt;


    // 工事ファイル名解析用正規表現のプリコンパイル
    [GeneratedRegex(Res.RegexJobname, RegexOptions.Compiled)]
    private static partial Regex RegexJobname();

    [GeneratedRegex(Res.RegexJobFilename, RegexOptions.Compiled)]
    private static partial Regex RegexJobFilename();

    // 空のインスタンス
    public static readonly Job Empty = new(null);

    /// <summary>
    /// 工事情報をファイルシステムから取得
    /// </summary>
    /// <param name="path"></param>
    /// <param name="repository"></param>
    /// <returns></returns>
    public Job(string? path, Repository? repository = null)
    {
        if (path == null)
        {
            return;
        }
        try
        {
            // 
            Folder = new(path);

            // ファイル名の取得
            string filename = System.IO.Path.GetFileName(Folder.Value);

            // ファイル名を解析する
            Match match = RegexJobname().Match(filename);
            if (match.Groups.Count != 4)
            {
                return;
            }

            // パラメータを設定する
            Start = new Date(match.Groups[1].Value);
            Company = new Company(match.Groups[2].Value, repository);
            Name = match.Groups[3].Value;

            // JobFile フィールドのセット
            // 工事フォルダの存在も条件とする
            Folder.Refresh();
            if (Folder.Exists)
            {
                // 工事フォルダ内のJsonlファイルを取得
                var jsonPaths = Directory.GetFileSystemEntries(Folder.Value, "*.json");

                string? jsonFilename = null;
                foreach (var jsonpath in jsonPaths)
                {
                    jsonFilename = System.IO.Path.GetFileName(jsonpath);
                    if (RegexJobname().IsMatch(jsonFilename)
                        || RegexJobFilename().IsMatch(jsonFilename))
                    {
                        break;
                    }
                    jsonFilename = null;
                }

                if (string.IsNullOrEmpty(jsonFilename) == false)
                {
                    JobFile = new()
                    {
                        Current = System.IO.Path.Join(job.Folder.Current, jsonFilename),
                        Justice = System.IO.Path.Join(job.Folder.Justice,
                        System.IO.Path.GetFileName(job.Folder.Justice) + ".json")
                    };
                }
            }

            // JobFile が存在しない場合は JobFileを作成する
            string jobfile = job.JobFile?.Current ?? string.Empty;
            if (File.Exists(jobfile) == false)
            {
                File.Create(jobfile);
                return job;
            }

            using var jobPackage = new ExcelPackage(jobfile);
            var jobBook = jobPackage?.Workbook ?? null;
            if (jobBook == null)
            {
                return job;
            }

            var jobdataSheet = jobBook.Worksheets["JobData"];
            if (jobdataSheet == null)
            {
                if (ModifyJobfile("@renameJobdataSheetname", jobPackage) == false)
                {
                    return job;
                }
                jobdataSheet = jobBook.Worksheets["JobData"];
            }

            var jobdataFinish = jobdataSheet.Cells["B4"].Value;
            job.Finish = new Date(jobdataFinish).ToFileString();

            return job;
        }
        catch
        {
            return job ?? Job.Empty;
        }
    }

    /// <summary>
    /// ファイルシステムから工事一覧を取得
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<Job> GetJobs()
    {
        IEnumerable<Job> jobs = [];
        try
        {
            string folderpath = Res.JobDefaultPath;
            jobs = Directory.GetFileSystemEntries(folderpath)
#if DEBUG
                // No Parallel.
#else
                // Parallel processing.
                .AsParallel()
#endif
                .Select(x => GetJob(x))
                .Where(x => x != null)
                .OfType<Job>();
        }
        catch
        {
            jobs = [];
        }

        return jobs;
    }

    private static bool ModifyJobfile(string mode, ExcelPackage? jobPackage)
    {
        if (jobPackage == null)
        {
            return false;
        }

        switch (mode)
        {
            case "@renameJobdataSheetname":
                foreach (var sheet in jobPackage.Workbook.Worksheets)
                {
                    if (sheet.Name == "作業員")
                    {
                        sheet.Name = "JobData";
                        jobPackage.Save();
                        return true;
                    }
                }
                break;
        }

        return false;
    }
}

public partial class Query
{
    public IEnumerable<Job> GetJobs([Service] Repository repository, IResolverContext context, Fullpath? folder = null)
    {
        return repository.GetJobs();
    }

    public Job GetJobByPath([Service] Repository repository, IResolverContext context, string path)
    {
        return repository.GetJob(path) ?? Job.Empty;
    }

    public Job GetJobById([Service] Repository repository, IResolverContext context, string id)
    {
        return repository.GetJobByIdFromDatabase(id) ?? Job.Empty;
    }

    /// <summary>
    /// 要求されているフィールド名を返す
    /// Job用に追加
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public static string[]? GetRequestJobFields(IResolverContext? context)
    {
        // nullチェック
        if (context == null)
        {
            return null;
        }

        HashSet<string> fields = [.. GetRequestFields(context)];

        // 依存関係の定義
        (string name, string dpend)[] dependList = [
            ("finish", "@jobfileAccess"),
            ("memo", "@jobfileAccess"),
            ];

        foreach (var (name, dpend) in dependList)
        {
            if (HasRequestField([.. fields], name))
            {
                fields.Add(dpend);
            }
        }

        return [.. fields];
    }
}

//public class AnypathContext : DbContext
//{

//    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//    {
//        optionsBuilder.UseSqlite(Res.JobScheduleDBPath);
//    }
//}

public partial class Repository : DbContext
{
    public DbSet<Job> JobDb { get; set; }

    private FileSystemWatcher? _JobWatcher;
    //private SQLiteConnection? _JobScheduleConnection;

    public void AssociateJob()
    {
        // WatcherJob();

        // ファイルシステムから工事一覧を取得しデータベースを更新する

        // ファイルシステムから取得
        var fsjobs = Job.GetJobs();

        // ファイルシステムになくてデータベースにあるデータを削除する
        var dbjobs = JobDb.ToArray();
        foreach (var dbjob in dbjobs)
        {
            if (fsjobs.Any(x => x.Id == dbjob.Id) == false)
            {
                JobDb.Remove(dbjob);
            }
        }

        // ファイルシステムにあってデータベースにないデータを追加する
        foreach (var fsjob in fsjobs)
        {
            if (JobDb.Any(x => x.Id == fsjob.Id) == false)
            {
                JobDb.Add(fsjob);
            }
        }
        this.SaveChanges();
    }

    private void DisposeJob()
    {
        _JobWatcher?.Dispose();

        //_JobScheduleConnection?.Close();
        //_JobScheduleConnection?.Dispose();

    }

    public Job? GetJobByIdFromDatabase(string id)
    {
        return JobDb.Find(id);
    }

    public Job[] GetJobs()
    {
        return JobDb.ToArray();
    }

    public Job? GetJob(string path)
    {
        var folder = new Fullpath(path);
        return JobDb.Find(folder.Value);
    }

    private static void PrintException(Exception? ex)
    {
        if (ex != null)
        {
            Console.WriteLine($"Message: {ex.Message}");
            Console.WriteLine("Stacktrace:");
            Console.WriteLine(ex.StackTrace);
            Console.WriteLine();
            PrintException(ex.InnerException);
        }
    }
}
