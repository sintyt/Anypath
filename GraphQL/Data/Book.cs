/// <summary>
/// 
/// </summary>
public class Book
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public Author? Author { get; set; }  
}

public partial class Query
{
    public IEnumerable<Book> Books([Service] Repository repository)
    {
        return repository.GetBooks();
    }

    public Book? Book([Service] Repository repository, int id)
    {
        return repository.GetBook(id);
    }
}

public partial class Repository
{
    public IEnumerable<Book> GetBooks() => _authors switch
    {
        null => [],
        _ => _authors.SelectMany(x => x.Books ?? []),
    };

    public Book? GetBook(int id)
    {
        return GetBooks().FirstOrDefault(x => x.Id == id);
    }
}

