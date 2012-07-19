using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HeroWars.Tools
{
    static class Extentions
    {
        public static byte[] ToByteArray(this string str)
        {
            ASCIIEncoding enc = new ASCIIEncoding();
            return enc.GetBytes(str);
        }

        public static string ToString(this byte[] arr)
        {
            ASCIIEncoding enc = new ASCIIEncoding();
            return enc.GetString(arr);
        }

        public static string ToAscii(this string input)
        {
            StringBuilder output = new StringBuilder(string.Empty);
            if (!string.IsNullOrEmpty(input))
            {
                for (int i = 0; i < input.Length; i++)
                    output.AppendFormat("&#{0};",
                       Encoding.ASCII.GetBytes(input.Substring(i, 1))[0]);
            }
            return output.ToString();
        }
   }
}
