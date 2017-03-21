using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace api.Resources.Functions
{
    public class Functions
    {
        /// <summary>
        /// Decode base64 to string
        /// </summary>
        /// <param name="b64"></param>
        /// <returns>string</returns>
        public static string DecodeBase64toString(string b64)
        {
            return System.Text.Encoding.UTF8.GetString(
                Convert.FromBase64String(
                    b64
                ));
        }
        /// <summary>
        /// Encode string to base64
        /// </summary>
        /// <param name="b64"></param>
        /// <returns>string</returns>
        public static string EncodeToBase64(string b64)
        {
            return Convert.ToBase64String(
                Encoding.ASCII.GetBytes(b64)
                );
        }
    }
}