
public class Lib
{
    /// <summary>
    /// �����񂩂�ID�p��5���̃n�b�V���l�̕�����𐶐�
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string Id(string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return Res.ZeroId;
        }
        int hashCode = str.GetHashCode();
        int id = Math.Abs(hashCode) % 100000;
        return id.ToString("D5");
    }

    /// <summary>
    /// ���l����ID�T���̃n�b�V���l�̕�����𐶐�
    /// </summary>
    /// <param name="num"></param>
    /// <returns></returns>
    public static string Id(long num)
    {
        if (num <= 0)
        {
            return Res.ZeroId;
        }
        long hashCode = (long)((ulong)num * 11400714819323198485) % 4294967296;
        long fiveDigitCode = hashCode % 100000;
        return fiveDigitCode.ToString("D5");
    }
}