using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace BlueToque.Utility
{
    /// <summary>
    /// Extension methods for string
    /// </summary>
    public static class StringHelper
    {
        /// <summary>
        /// Convert a string collection to an array of strings.
        /// </summary>
        /// <param name="collection"></param>
        /// <returns></returns>
        public static string[] ToArray(this StringCollection collection)
        {
            if (collection == null)
                return [];

            string[] array = new string[collection.Count];
            collection.CopyTo(array, 0);
            return array;
        }

        /// <summary>
        /// Get the first part of a string that has multiple seperators
        /// </summary>
        /// <param name="value"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string GetFirstValue(this string value, char separator)
        {
            try
            {
                int num = value.IndexOf(separator);
                if (num == -1)
                    return value;

                return value[num..];
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error getting value {0}:\r\n{1}", value, ex);
                return value;
            }
        }

        /// <summary>
        /// Extension for the string class that gets the value of a name/value pair The name/value
        /// pair is delimited by the separator parameter.The code searches for the last
        /// instance of the separator character and returns the string after that character.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string GetValue(this string value, char separator)
        {
            try
            {
                string value2 = new([separator]);
                if (!value.Contains(value2))
                    return value;

                if (value.EndsWith(value2))
                    return string.Empty;

                int num = value.LastIndexOf(separator) + 1;
                if (num > value.Length)
                    return string.Empty;

                return value[num..];
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error getting value from name/value pair {0}:\r\n{1}", value, ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// Extension method for a string that get the name of a name/value pair Searches
        /// for the last instance of the "separator" parameter and returns the string that
        /// occurs before that separator.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        public static string GetName(this string value, char separator)
        {
            try
            {
                string value2 = new([separator]);
                if (!value.Contains(value2))
                    return value;

                return value[..value.IndexOf(separator)];
            }
            catch (Exception ex)
            {
                Trace.TraceError("Error getting name from name/value pair {0}:\r\n{1}", value, ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// Remove whitespace characters from a string
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public static string RemoveWhiteSpace(this string inputString) =>
            string.IsNullOrEmpty(inputString)
                ? string.Empty
                : new string((from c in inputString.ToCharArray()
                              where !char.IsWhiteSpace(c)
                              select c).ToArray());

        /// <summary>
        /// Remove strings
        /// </summary>
        /// <param name="inputString"></param>
        /// <param name="stringsToRemove"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public static string RemoveStrings(this string inputString, IEnumerable<string> stringsToRemove, bool ignoreCase = false)
        {
            if (string.IsNullOrEmpty(inputString) || stringsToRemove == null)
            {
                Trace.TraceWarning("Warning: input string is null");
                return inputString;
            }

            string text = inputString;
            foreach (string item in stringsToRemove)
                text = text.RemoveString(item, ignoreCase);

            return text;
        }

        /// <summary>
        /// remove the given string from the input string, ignoring case if the ignoreCase
        /// flag is set to true (default)
        /// </summary>
        /// <param name="inputString"></param>
        /// <param name="stringToRemove"></param>
        /// <param name="ignoreCase"></param>
        /// <returns></returns>
        public static string RemoveString(this string inputString, string stringToRemove, bool ignoreCase = false)
        {
            if (string.IsNullOrEmpty(inputString) || string.IsNullOrEmpty(stringToRemove))
            {
                Trace.TraceWarning("Warning: input string is null");
                return inputString;
            }

            if (ignoreCase)
            {
                int num = inputString.IndexOf(stringToRemove, StringComparison.CurrentCultureIgnoreCase);
                if (num < 0)
                    return inputString;

                return inputString.Remove(num, stringToRemove.Length);
            }

            return inputString.Replace(stringToRemove, string.Empty);
        }

        /// <summary>
        /// Hashes the string with the MD5 algorithm
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string MD5Hash(this string s) => BitConverter.ToString(MD5.HashData(new UnicodeEncoding().GetBytes(s)));

        /// <summary>
        /// Removes any HTML markup from the string
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string StripHTML(this string s) => Regex.Replace(s, "<(.|n)*?>", "");

        /// <summary>
        /// Determines whether the string contains an email address
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsEmailAddress(this string s) => new Regex("^[\\w-\\.]+@([\\w-]+\\.)+[\\w-]{2,4}$").IsMatch(s);

        /// <summary>
        /// Determines whether the string contains a URL
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsUrl(this string s) => new Regex("http(s)?://([\\w-]+\\.)+[\\w-]+(/[\\w- ./?%&=]*)?").IsMatch(s);

        /// <summary>
        /// Returns TRUE if the string contains only digits and "."
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsDigits(this string value)
        {
            if (value.IsNullOrEmpty())
                return false;

            foreach (char c in value)
            {
                if (!char.IsDigit(c) && c != '.')
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool StartsWithDigits(this string value) => !value.IsNullOrEmpty() && char.IsDigit(value[0]);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool ContainsDigits(this string value) => !value.IsNullOrEmpty() && value.Any(c => char.IsDigit(c));

        /// <summary>
        /// return true if a string is null, or an empty string
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty([NotNullWhen(false)] this string? s) => string.IsNullOrEmpty(s);

        /// <summary>
        /// A substring method that takes a start position and an end posiiton as parameters
        /// </summary>
        /// <param name="s"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public static string SubString(this string s, int start, int end) => (start >= end) ? string.Empty : s[start..end];

        /// <summary>
        /// Retrieve the first instance of a quoted string from the string
        /// </summary>
        /// <param name="s"></param>
        /// <param name="quote"></param>
        /// <returns></returns>
        public static string GetQuoted(this string s, char quote)
        {
            int num = s.IndexOf(quote) + 1;
            int end = s.IndexOf(quote, num);
            return s.SubString(num, end);
        }

        /// <summary>
        /// Convert a string to a list of values split on the delimiter
        /// </summary>
        /// <param name="values"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        public static List<string> FromDelimited(this string values, char delimiter = ',') => values.Split(delimiter).ToList();

        /// <summary>
        /// Convert a string array to a single delimited string
        /// </summary>
        /// <param name="values"></param>
        /// <param name="delimiter"></param>
        /// <returns></returns>
        public static string ToDelimited(this IEnumerable<string> values, char delimiter = ',') => string.Join(delimiter.ToString(), values);

        /// <summary>
        /// Convenience method that calls System.Uri.EscapeUriString(System.String)
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string EscapeUri(this string val) => Uri.EscapeDataString(val);

        /// <summary>
        /// Convenience method that calls System.Uri.UnescapeDataString(System.String)
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        public static string UnescapeUri(this string val) => Uri.UnescapeDataString(val);
    }
}