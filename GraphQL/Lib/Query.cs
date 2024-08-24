using HotChocolate.Language;
using HotChocolate.Resolvers;
using static System.Net.Mime.MediaTypeNames;

public partial class Query
{
    /// <summary>
    /// 要求されているフィールド名を返す
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    private static string[] GetRequestFields(IResolverContext context)
    {
        var requestName = context.Selection.SyntaxNode.Name.Value;
        if (requestName == null)
        {
            return [];
        }

        var res = RequestFieldsRecursively(context.Selection.SelectionSet, requestName);
        return [.. res];
    }

    // SelectionSetNode で受け取って
    // 関数内でFieldNode に分解し
    // 必要に応じて SelectionSetNode再帰を行う
    private static List<string> RequestFieldsRecursively(SelectionSetNode? selectionSetNode, string requestName)
    {
        List<string> fields = [];
        foreach (var fieldNode in (selectionSetNode?.GetNodes() ?? []).Cast<FieldNode>())
        {
            var name = fieldNode?.Name.Value;
            if (string.IsNullOrEmpty(name))
            {
                continue;
            }

            // 
            var fullName = requestName + '.' + name;

            selectionSetNode = fieldNode?.SelectionSet;
            if (fieldNode?.SelectionSet == null)
            {
                fields.Add(fullName);
            }
            else
            {
                var addFields = RequestFieldsRecursively(fieldNode?.SelectionSet, fullName);
                fields.AddRange(addFields);
            }
        }
        return fields;
    }

    /// <summary>
    /// フィールド名が含まれているかチェック
    /// 大文字小文字は無視します
    /// </summary>
    /// <param name="requestFields"></param>
    /// <param name="targetnames"></param>
    /// <returns>一つでも含まれていれば true を返す</returns>
    public static bool HasRequestField(string[]? requestFields, params string[] targetnames)
    {
        foreach (var tar in targetnames)
        {
            var lowtar = tar?.ToLower() ?? string.Empty;
            foreach (var request in requestFields ?? [])
            {
                var lowreq = request?.ToLower() ?? string.Empty;

                if (lowreq.Contains(lowtar))
                {
                    return true;
                }
            }
        }

        return false;
    }

}
