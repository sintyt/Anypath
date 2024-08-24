using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

public class Company
{

    private string _id = Res.ZeroId;
    [Key]
    public string Id
    {
        get => _id;
        set { }
    }

    public Fullpath Folder { get; set; } = new();

    private string _name = string.Empty;
    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            _id = Lib.Id(value);
        }
    }
    public List<Member>? Members { get; set; }

    public Company() { }

    public Company(string name, Repository? repository = null)
    {
        Name = name;
        
        // 会社フォルダーを検索
        Folder = FindCompanyFolderOnName(name, repository) ?? new();
    }

    // 会社フォルダーを検索する関数
    public static Fullpath? FindCompanyFolderOnName(string name, Repository? repository = null)
    {
        if (repository == null)
        {
            return null;
        }

        var company = repository.Companies?.Find(name);
        return company?.Folder;
    }
}

public partial class Repository : DbContext
{
    public DbSet<Company>? Companies { get; set; }
}