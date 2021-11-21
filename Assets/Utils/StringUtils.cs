namespace Piratera.Utils
{
    
public static class StringUtils
{
    /// <summary>
    /// Method that limits the length of text to a defined length.
    /// </summary>
    /// <param name="source">The source text.</param>
    /// <param name="maxLength">The maximum limit of the string to return.</param>
    public static string LimitLength(this string source, int maxLength)
    {
        if (source.Length <= maxLength)
        {
            return source;
        }

        return source.Substring(0, maxLength - 3) + "...";
    }

    public static string ShortNumber(float n, uint decimals = 1)
        {
            if (n < 1e3) return n.ToString($"N{decimals}");
            if (n >= 1e3 && n < 1e6) return (n / 1e3).ToString($"N{decimals}") + "K";
            if (n >= 1e6 && n < 1e9) return (n / 1e6).ToString($"N{decimals}") + "M";
            if (n >= 1e9 && n < 1e12) return (n / 1e9).ToString($"N{decimals}") + "B";
            if (n >= 1e12) return (n / 1e12).ToString($"N{decimals}") + "T";

            return n.ToString($"N{decimals}");
        }
    


    public static string ShortNumber(long n, uint decimals = 1)
    {
        if (n < 1e3) return n.ToString($"N{decimals}");
        if (n >= 1e3 && n < 1e6) return (n / 1e3).ToString($"N{decimals}") + "K";
        if (n >= 1e6 && n < 1e9) return (n / 1e6).ToString($"N{decimals}") + "M";
        if (n >= 1e9 && n < 1e12) return (n / 1e9).ToString($"N{decimals}") + "B";
        if (n >= 1e12) return (n / 1e12).ToString($"N{decimals}") + "T";

        return n.ToString($"N{decimals}");
    }
}
}