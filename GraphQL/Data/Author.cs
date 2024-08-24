public class Author
{
    public required int Id { get; set; }
    public required string Name { get; set; }
    public List<Book>? Books { get; set; }
}

public partial class Query
{
    public IEnumerable<Author> Authors([Service] Repository repository)
    {
        return repository.GetAuthors();
    }

    public Author? AuthorById([Service] Repository repository, int id)
    {
        return repository.GetAuthorById(id);
    }
}

public partial class Repository
{
    private List<Author>? _authors;
    public void SetupAuthor()
    {
        // データの初期化
        _authors = [];

        // 川端康成のデータを登録
        var auther = new Author
        {
            Id = 1,
            Name = "川端康成",
        };

        auther.Books =
        [
            new() { Id = 11, Title = "雪国", Author = auther},
            new() { Id = 12, Title = "伊豆の踊り子", Author = auther}
        ];
        _authors.Add(auther);

        // 夏目漱石のデータを登録
        auther = new Author
        {
            Id = 2,
            Name = "夏目漱石",
        };
        auther.Books =
        [
            new() { Id = 21, Title = "坊ちゃん", Author = auther},
            new() { Id = 22, Title = "三四郎", Author = auther}
        ];
        _authors.Add(auther);

        // 豊田新のデータを登録
        auther = new Author
        {
            Id = 3,
            Name = "豊田新",
        };
        auther.Books =
        [
            new() { Id = 31, Title = "もちたん", Author = auther},
            new() { Id = 32, Title = "みーたん", Author = auther}
        ];
        _authors.Add(auther);

    }

    public IEnumerable<Author> GetAuthors()
    {
        return _authors ?? Enumerable.Empty<Author>();
    }

    public Author? GetAuthorById(int id)
    {
        try
        {
            return _authors?.First(author => author.Id == id);
        }
        catch
        {
            return null;
        }
    }

}

