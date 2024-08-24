static public class Res
{
    // Anypath
    public const string ZeroId = "00000";

    // Format
    public const string FileDateFormat = "yyyy-MMdd";          // ファイル名用
    public const string EditDateFormat = "yyyy-MM-dd";         // 入出力用
    public const string PureDateFormat = "yyyy-MM-dd'T'HH:mm:ss.fffffff";   // 実態表示用

    // Regex
    public const string RegexJobname = @"^(\d{4}-\d{4})[_\s]([^_]+)[_\s](.+)$";
    public const string RegexJobFilename = @"^(工事)?[_\s-]?(\d{4})-.json$";
    public const string CompanyNameRegex = @"^(\d{1})-(.+)$";

    // Path
    public const string JobDefaultPath = @"O:\1-工事";
    public const string JobDefaultOldPath = @"O:\1-工事\@過去";
    public const string CompanyDefaultPath = @"O:\2-築炉会社";

    public const string AnypathDbPath = @"Data Source=O:\0-豊田築炉\社員\豊田新 1971-0618 A型\Anypath.db";

}