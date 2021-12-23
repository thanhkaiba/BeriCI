using System;

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

            if (string.IsNullOrEmpty(source))
            {
                return "";

            }
            if (source.Length <= maxLength)
            {
                return source;
            }

            return source.Substring(0, maxLength - 3) + "...";
        }


        public static string ShortNumber(long n, uint decimals = 0)
        {
            long min = 0;
            if (decimals > 0 && n > min)
            {
                min = (long)Math.Pow(10, decimals);
            }

            if (n >= 1000000000 && n > min)
            {
                return (n / 1000000000D).ToString("0.##B");
            }

            if (n >= 100000000 && n > min)
            {
                return (n / 1000000D).ToString("0.#M");
            }
            if (n >= 1000000 && n > min)
            {
                return (n / 1000000D).ToString("0.##M");
            }
            if (n >= 100000 && n > min)
            {
                return (n / 1000D).ToString("0.#k");
            }
            if (n >= 10000 && n > min)
            {
                return (n / 1000D).ToString("0.##k");
            }

            return n.ToString("#,0");
        }
    }
}