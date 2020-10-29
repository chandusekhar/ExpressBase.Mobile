﻿using ExpressBase.Mobile.Constants;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace ExpressBase.Mobile.Extensions
{
    public static class StringExtension
    {
        public static string ToMD5(this string str)
        {
            var md5 = MD5.Create();

            byte[] result = md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(str));

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                strBuilder.Append(result[i].ToString("x2"));
            }
            return strBuilder.ToString();
        }

        public static int ToObjId(this string refid)
        {
            return Convert.ToInt32(refid.Split(CharConstants.DASH)[3]);
        }

        public static string RemoveSubstring(this string current, string word)
        {
            return current.Replace(word, string.Empty);
        }

        public static string ToFontIcon(this string iconString, string onError = null)
        {
            try
            {
                if (iconString.Length != 4)
                    throw new Exception();

                return Regex.Unescape("\\u" + iconString);
            }
            catch (Exception)
            {
                if(onError != null)
                {
                    return Regex.Unescape("\\u" + onError);
                }
            }
            return null;
        }
    }
}
