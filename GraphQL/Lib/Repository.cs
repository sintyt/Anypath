using Microsoft.EntityFrameworkCore;
using System.Text;

public partial class Repository
{
    public Repository()
    {
        SetupAuthor();

        // Associate Job Data
        AssociateJob();
    }

    ~Repository()
    {
        DisposeJob();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(Res.AnypathDbPath);
    }
}