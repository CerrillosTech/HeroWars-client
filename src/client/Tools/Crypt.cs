using System;
using System.Security.Cryptography;
using System.Text;

namespace HeroWars.Tools
{
    static class Crypt
    {
        /// <summary>
        /// returns a md5 hash as string
        /// </summary>
        /// <param name="text">string to hash.</param>
        /// <returns>md5 hash as string.</returns>
        public static string MD5(string text)
        {
            //Prüfen ob Daten übergeben wurden.
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }
    
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] textToHash = Encoding.Default.GetBytes(text);
            byte[] hash = md5.ComputeHash(textToHash);

            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }
    }
}
