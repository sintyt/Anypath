public class Member
{
    public int Id { get; set; }
    public required Fullpath Folder { get; set; }
    public required string LastName { get; set; }
    public required string FirstName { get; set; }
    public string? MiddleName { get; set; }
    public string? Birth { get; set; }
    public string? Zipcode { get; set; }
    public string? Address { get; set; }
    public string? Memo { get; set; }
}
