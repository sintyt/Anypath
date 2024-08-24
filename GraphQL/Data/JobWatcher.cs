using System;

public partial class Repository
{
    public void JobWatcher()
    {
        _JobWatcher?.Dispose();
        _JobWatcher = new FileSystemWatcher(Res.JobDefaultPath)
        {
            NotifyFilter = NotifyFilters.Attributes
                             | NotifyFilters.CreationTime
                             | NotifyFilters.DirectoryName
                             | NotifyFilters.FileName
                             | NotifyFilters.LastAccess
                             | NotifyFilters.LastWrite
                             | NotifyFilters.Security
                             | NotifyFilters.Size
        };

        _JobWatcher.Changed += OnChanged;
        _JobWatcher.Created += OnCreated;
        _JobWatcher.Deleted += OnDeleted;
        _JobWatcher.Renamed += OnRenamed;
        _JobWatcher.Error += OnError;

        _JobWatcher.Filter = "*";
        _JobWatcher.IncludeSubdirectories = true;
        _JobWatcher.EnableRaisingEvents = true;
    }

    private static void OnChanged(object sender, FileSystemEventArgs e)
    {
        if (e.ChangeType != WatcherChangeTypes.Changed)
        {
            return;
        }
        Console.WriteLine($"Changed: {e.FullPath}");
    }

    private static void OnCreated(object sender, FileSystemEventArgs e)
    {
        string value = $"Created: {e.FullPath}";
        Console.WriteLine(value);
    }

    private static void OnDeleted(object sender, FileSystemEventArgs e) =>
        Console.WriteLine($"Deleted: {e.FullPath}");

    private static void OnRenamed(object sender, RenamedEventArgs e)
    {
        Console.WriteLine($"Renamed:");
        Console.WriteLine($"    Old: {e.OldFullPath}");
        Console.WriteLine($"    New: {e.FullPath}");
    }

    private static void OnError(object sender, ErrorEventArgs e) =>
        PrintException(e.GetException());


}
