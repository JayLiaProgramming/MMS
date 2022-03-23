using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace MMS_UI.Extensions
{
    static class Strings
    {
        public static IEnumerable<string> SplitAndKeep(this string s, char[] delims)
        {
            int start = 0, index;

            while ((index = s.IndexOfAny(delims, start)) != -1)
            {
                if (index - start > 0)
                    yield return s.Substring(start, index + 1);
                //yield return s.Substring(index, 1);
                start = index + 1;
            }

            if (start < s.Length)
            {
                yield return s.Substring(start);
            }
        }
        public static string RemoveAll(this string value, params string[] args)
        {
            try
            {
                if (string.IsNullOrEmpty(value))
                    return value;

                string retVal = value;
                foreach (string arg in args)
                {
                    if (!string.IsNullOrEmpty(arg))
                        retVal = retVal.Replace(arg, "");
                }

                return retVal;
            }
            catch { }
            return string.Empty;
        }
        static public string Substring(this string search, string start, string end, bool useLast)
        {
            int startIndex = (useLast ? search.LastIndexOf(start) : search.IndexOf(start)) + start.Length;
            int length = search.Length - startIndex;
            if (end != "")
                length = search.IndexOf(end, startIndex) - startIndex;
            if (length > 0)
                return search.Substring(startIndex, length);

            return search;
        }        
        static public string Substring(this string search, int start, string end, bool useLast)
        {
            int startIndex = start;
            int length = search.Length - startIndex;
            if (end != "")
                length = (useLast ? search.LastIndexOf(end) : search.IndexOf(end, startIndex)) - startIndex;
            if (length > 0)
                return search.Substring(startIndex, length);

            return search;
        }
        static public string Substring(this string search, string start, int end, bool useLast)
        {
            int startIndex = (useLast ? search.LastIndexOf(start) : search.IndexOf(start)) + start.Length;
            int length = search.Length - startIndex;
            length = end - startIndex;
            if (length > 0)
                return search.Substring(startIndex, length);

            return search;
        }
        static public string Substring(this string search, string start, bool useLast)
        {
            int startIndex = (useLast ? search.LastIndexOf(start) : search.IndexOf(start)) + start.Length;
            if (startIndex > -1)
                return search.Substring(startIndex);

            return search;
        }
        static public string Substring(this string search, string start, string end)
        {
            int startIndex = search.IndexOf(start);
            if (startIndex == -1) return string.Empty;
            startIndex += start.Length;
            int length = search.Length - startIndex;
            if (end != "")
                length = search.IndexOf(end, startIndex) - startIndex;
            if (length > 0)
                return search.Substring(startIndex, length);
            else
                return string.Empty;
        }
        static public string Substring(this string search, int start, string end)
        {
            int startIndex = start;
            int length = search.Length - startIndex;
            if (end != "")
                length = search.IndexOf(end, startIndex) - startIndex;
            if (length > 0)
                return search.Substring(startIndex, length);

            return search;
        }
        static public string Substring(this string search, string start, int end)
        {
            int startIndex = search.IndexOf(start) + start.Length;
            int length = search.Length - startIndex;
            length = end - startIndex;
            if (length > 0)
                return search.Substring(startIndex, length);

            return search;
        }
        static public string Substring(this string search, int start, int end)
        {
            int startIndex = start;
            int length = search.Length - startIndex;
            length = end - startIndex;
            if (length > 0)
                return search.Substring(startIndex, length);

            return search;
        }
        static public string Substring(this string search, string start)
        {
            int startIndex = search.IndexOf(start) + start.Length;
            if (startIndex > -1)
                return search.Substring(startIndex);

            return search;
        }
        static public string Substring(this string search, int start, string[] ends)
        {
            foreach (var end in ends)
            {
                var endMarker = search.IndexOf(end, start);
                if (endMarker > -1)
                    return search.Substring(start, endMarker - start);
            }
            if (start > -1)
                return search.Substring(start);
            return string.Empty;
        }
        static public string Substring(this string search, string start, string[] ends)
        {
            foreach (var end in ends)
            {
                var endMarker = search.IndexOf(end, search.IndexOf(start) + start.Length);
                if (endMarker > -1)
                    return search.Substring(start, end);
            }
            if (search.IndexOf(start) > -1)
                return search.Substring(start);
            return string.Empty;
        }
    }
}