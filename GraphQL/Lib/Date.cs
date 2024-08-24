using System.Globalization;
using System.Text.RegularExpressions;

/// <summary>
/// 日付処理をするクラス
/// 保持するデータはLocal時間で保持する
/// </summary>
public partial class Date
{
    // 未指定時の時間定義
    // 西暦1年１月１日０時０分０秒
    public static readonly DateTime Zero = new(1, 1, 1);
    public static readonly DateTime UnknownStamp = new(1601, 1, 1);

    // Id
    private string _id = Res.ZeroId;
    public string Id
    {
        get => _id;
    }

    // 日付保持変数
    private DateTime _value = Zero;
    public DateTime Value
    {
        get => _value;
        set
        {
            _value = value;
            _id = Lib.Id(_value.Ticks);
        }
    }

    public int Year => _value.Year;
    public int Month => _value.Month;
    public int Day => _value.Day;

    // 区切り文字の正規表現
    [GeneratedRegex(@"[-_/　\s]")]
    private static partial Regex RegexDateSplit();

    public Date() { }

    /// <summary>
    /// 文字列からDateインスタンスを生成。
    /// 文字列解析不可の時はId:Res.ZeroId, Value:nullを設定する。
    /// </summary>
    /// <param name="str">日付文字列</param>
    public Date(string str)
    {
        // Res.PureDateFormat形式の文字列の解析
        if (DateTime.TryParseExact(
            str,
            Res.PureDateFormat,
            null,
            DateTimeStyles.None,
            out DateTime pureParse))
        {
            Value = pureParse;
            return;
        }

        // 文字列を区切り文字で分割する
        var items = RegexDateSplit().Split(str);
        var len = items.Length;
        int year, month, day;

        // 文字列が1つのグループ yyyyMMdd or yyMMdd
        if (len == 1)
        {
            // 西暦年月日の決定
            if (int.TryParse(str, out int yyyyMMdd) == false)
            {
                return;
            }
            day = yyyyMMdd % 100;
            int yyyyMM = yyyyMMdd / 100;
            month = yyyyMM % 100;
            year = yyyyMM / 100;
        }

        // 文字列が2つのグループの場合 yy/MMdd or yyyy/MMdd
        else if (len == 2)
        {
            // 西暦の決定
            if (int.TryParse(items[0], out int yyyy) == false)
            {
                return;
            }
            year = yyyy;

            // 月日の決定
            if (int.TryParse(items[1], out int MMdd) == false)
            {
                return;
            }
            day = MMdd % 100;
            month = MMdd / 100;
        }

        // 文字列が3つのグループの場合 yy or yyyy, MM or M, dd or d
        else if (len == 3)
        {
            // 西暦の決定
            if (int.TryParse(items[0], out int yyyy) == false)
            {
                return;
            }
            year = yyyy;

            // 月の決定
            if (int.TryParse(items[1], out int MM) == false)
            {
                return;
            }
            month = MM;

            // 日の決定
            if (int.TryParse(items[2], out int dd) == false)
            {
                return;
            }
            day = dd;
        }

        // 文字列のグループ数が４つ以上は解析不可
        else
        {
            return;
        }

        Value = new(year, month, day);
    }

    public bool IsZero() => Value.Ticks == 0;

    override public string ToString() => Value.ToString(Res.EditDateFormat);

    public string ToString(string format) => Value.ToString(format);

    public string ToPureString() => Value.ToString(Res.PureDateFormat);

    public string ToFileString() => Value.ToString(Res.FileDateFormat);

    public string ToEditString() => Value.ToString(Res.EditDateFormat);
}
