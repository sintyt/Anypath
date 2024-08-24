using System.ComponentModel.DataAnnotations;

public interface IFullpath
{
    string Value { get; set; }
    string Id { get; }
    string Parent { get; set; }
    string Filename { get; set; }
    bool Exists { get; }
    Date CreatedAt { get; }
    Date UpdatedAt { get; }
    Date DeletedAt { get; }

    void Refresh();
}

public class Fullpath : IFullpath
{
    private string _id = Res.ZeroId;
    private string _fullpath = string.Empty;
    private string _parent = string.Empty;
    private string _filename = string.Empty;

    public string Id
    {
        get => _id;
    }

    public string Value
    {
        get => _fullpath;
        set
        {
            _fullpath = value.Replace('/', '\\');
            _parent = System.IO.Path.GetDirectoryName(_fullpath) ?? string.Empty;
            _filename = System.IO.Path.GetFileName(_fullpath);
            _id = Lib.Id(_fullpath);
        }
    }

    public string Parent
    {
        get => _parent;
        set
        {
            _parent = value.Replace('/', '\\');
            _fullpath = System.IO.Path.Combine(_parent, _filename);
            _id = Lib.Id(_fullpath);
        }
    }

    public string Filename
    {
        get => _filename;
        set
        {
            _filename = value.Replace('/', '\\');
            _fullpath = System.IO.Path.Combine(_parent, _filename);
            _id = Lib.Id(_fullpath);
        }
    }

    Date _createdAt = new();
    public Date CreatedAt => _createdAt;

    Date _updatedAt = new();
    public Date UpdatedAt => _updatedAt;

    Date _deletedAt = new();
    public Date DeletedAt => _deletedAt;

    public Fullpath() { }

    public Fullpath(string fullpath)
    {
        Value = fullpath;
    }

    public bool _isExist = false;
    public bool Exists
    {
        get
        {
            Refresh();
            return _isExist;
        }
    }

    public void Refresh()
    {
        FileInfo fi;
        try
        {
            fi = new FileInfo(Value);
            fi.Refresh();
        }
        catch
        {
            _createdAt = new();
            _updatedAt = new();
            _deletedAt = new();
            _isExist = false;
            return;
        }

        _createdAt.Value = fi.CreationTime == Date.UnknownStamp ? new() : fi.CreationTime;
        _updatedAt.Value = fi.LastWriteTime == Date.UnknownStamp ? new() : fi.LastWriteTime;
        _deletedAt.Value = new();
        _isExist = _createdAt.Value != Date.UnknownStamp;
    }
}