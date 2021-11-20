using System.Text.RegularExpressions;

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
    //public static string AddComma(string x, char sign = ',') => Regex.Replace(x, "\B(?= (\d{ 3})+(?!\d))", RegexOptions.Multiline);


}

}