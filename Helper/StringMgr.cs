namespace WindowsStatsRepBldr.Helper;

internal class StringMgr
{
    public static string ContainsAny(string source, string[] substrings, StringComparison comparison)
    {
        foreach (var substring in substrings)
        {
            if (source.Contains(substring, comparison))
            {
                return substring;
            }
        }

        return string.Empty;
    }
}
