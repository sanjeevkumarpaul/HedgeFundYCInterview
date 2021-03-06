﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Extensions
{
    public static partial class ExtString
    {
        public static string FriendlyEntityStateName(this string entityState)
        {
            switch (entityState.ToUpper().Trim())
            {
                case "ADDED": return "Inserted";
                case "MODIFIED": return "Modified";
                case "DELETED": return "Deleted";
                default: return "Unknown";
            }
        }

        public static string ToEmpty(this string str, string defaultValue = null)
        {
            if (string.IsNullOrWhiteSpace(str)) str = defaultValue != null ? defaultValue : string.Empty;

            return str;
        }

        public static string MaxMinDateToEmpty(this string date)
        {
            try
            {
                DateTime _date = DateTime.Parse(date);
                //return (_date == DateTime.MinValue || _date == DateTime.MaxValue) ? "" : date;
                return (_date <= AppConstant.SQLDateTimeMin || _date >= AppConstant.SQLDateTimeMax) ? "" : date;
            }
            catch { }

            return date;
        }

        public static string ToNull(this string str, bool suppressSQLNull = false)
        {
            if (str.Empty()) return null;

            if (suppressSQLNull && str.ToUpper().Equals("NULL")) return null;

            return str;
        }

        public static bool Empty(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }
        
        /// <summary>
        /// will test if the string is Numeric
        /// Note: Numeric Values eg:  -23, -23.0, -23., 23. 23.0, 23.0000
        /// Note: Non Numeric eg: -23.0-, j34.99
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsNumeric(this string str)
        {
            return Regex.IsMatch(str, @"^[-]?\d+([.](\d+)?)?$");  
        }
        
        public static string ToBoolText(this string str, string YesVal = "YES", string NoVal = "NO"  )
        {
            bool yesno = (YesVal.ToEmpty() == "YES" && NoVal.ToEmpty() == "NO");
            bool truefalse = YesVal.Empty() && NoVal.Empty();
            bool _flag = str.ToEmpty().ToBool();
            

            return  yesno ? (_flag ? "YES" : "NO") : ( truefalse ? (_flag ? "TRUE" : "FALSE")  : (_flag ? YesVal : NoVal )   );
        }
        
        public static string RemoveSeprators(this string str)
        {
            return Regex.Replace(str, "[^0-9a-zA-Z]+", "");
        }

        public static string[] SplitCamalized(this string str)
        {
            return Regex.Split(str, "([A-Z])");
        }

        public static string SpaceCamalized(this string str)
        {
            string res = string.Empty;
            var words = Regex.Split(str, "([A-Z])").ToList();

            if (words.Count > 1)
                words.ForEach(s => { res += $"{ (Regex.IsMatch(s, "^[A-Z]") ? " " : "") + s }"; });
            else
                res = words.JoinExt().Capitalize();

            return res.TrimEx(" ");
        }

        public static string RightTill(this string str, string delimiter = " ")
        {
            if (str.Empty()) return "";

            List<string> entries = str.SplitEx(delimiter).ToList();

            if (entries.Count() > 1)
                entries.RemoveAt(entries.Count() - 1);

            return entries.JoinExt(delimiter);
        }

        public static string Capitalize(this string str)
        {
            if (!str.Empty())
            {
                return str[0].ToString().ToUpper() + str.Substring(1);
            }

            return null;
        }

        public static string[] ToLower(this string[] strs)
        {
            for (var index = 0; index < strs.Length; index++) strs[index] = strs[index].ToLower();

            return strs;
        }
        public static string[] ToUpper(this string[] strs)
        {
            for (var index = 0; index < strs.Length; index++) strs[index] = strs[index].ToUpper();

            return strs;
        }

        public static string RepeatFormatPlaceHolder(this string str, int length, string separator = ",")
        {
            str = "{0}";

            for (int i = 1; i < length; i++)
                str += "," + "{" + i.ToString() + "}";

            return str;

        }

        public static string Repeat(this string str, int length)
        {
            if (str.Empty()) return null;
            var temp = str;

            for (int r = 1; r < length; r++) str += temp;

            return str;
        }

        public static string PadRightEx(this string str, int length, char delimiter = ' ')
        {
            if (str == null) return null;
            var temp = str;

            if (temp.Length >= length) return str;

            for (int r = 1; r < length - temp.Length; r++) str += delimiter;

            return str;
        }

        public static string[] SplitEx(this string str, char delimiter = ' ', bool removeEmpty = true)
        {
            return str.Split(new char[] { delimiter }, removeEmpty ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
        }

        public static string[] SplitEx(this string str, string delimiter = " ", bool removeEmpty = true)
        {
            return str.Split(new string[] { delimiter }, removeEmpty ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
        }

        public static int ShotterLength(this string str, int defaultLen = 3)
        {
            return (str.Length <= defaultLen) ? str.Length : defaultLen;
        }


        public static string FriendlyColumnName(this String str)
        {
            str = String.Join(" ", str.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries));

            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            str = textInfo.ToTitleCase(str);

            return str;
        }

        public static bool EqualsIgnoreCase(this string str, string withStr)
        {
            if (str.Null()) return false;

            return str.Equals(withStr, StringComparison.CurrentCultureIgnoreCase);
        }

        public static string RemoveDomain(this string str)
        {
            //int pos = -1;

            //return (!str.Empty() && (pos = str.LastIndexOf('\\')) >= 0) ? str.Substring(pos + 1) : str; 
            return str.TakeLast('\\');
        }

        public static string TakeLast(this string str, char delimter = '.')
        {
            int pos = -1;

            return (!str.Empty() && (pos = str.LastIndexOf(delimter)) >= 0) ? str.Substring(pos + 1) : str;
        }
        
        public static string EscapeXmlNotations(this string str)
        {
            if (str.Empty()) return str;

            return str.Replace("&", "&amp;");
        }

        public static string UnescapeXmlNotations(this string str)
        {
            if (str.Empty()) return str;

            return str.Replace("&lt;", "<").Replace("&gt;", ">").Replace("&quot;", "\"").Replace("&amp;", "&");
        }

        public static string HTMLEscape(this string str)
        {
            return System.Net.WebUtility.HtmlEncode(str);
        }


        public static Int32 ToInt(this string str)
        {
            int result = 0;

            Int32.TryParse(str, out result);

            return result;
        }

        public static float ToFloat(this string str)
        {
            float result = 0.0f;

            float.TryParse(str, out result);

            return result;
        }

        public static double ToDouble(this string str)
        {
            double result = 0.0d;

            double.TryParse(str, out result);

            return result;
        }
        

        public static bool ToBool(this string str)
        {
            bool result = false;

            if (!str.Empty()) { str = str.ToLower(); result = str.Equals("true") || str.Equals("1") || str.Equals("y"); }

            return result;
        }

        public static DateTime ToDateTime(this string str, bool OnlySeconds = false)
        {
            DateTime mDateTime;
            DateTime.TryParse(str, out mDateTime);
            mDateTime = (mDateTime.Equals(DateTime.MinValue)) ? DateTime.Now : mDateTime;

            return OnlySeconds ? Convert.ToDateTime(mDateTime.ToString(AppConstant.DateFormat)) : mDateTime;
        }

        public static string TrimEx(this string str, string param = " ")
        {
            if (str.Empty()) return str;
            if (!str.StartsWith(param) && !str.EndsWith(param)) return str;

            string tstr = str;

            while (tstr.EndsWith(param))
            {
                tstr = tstr.Substring(0, tstr.Length - param.Length);
            }
            while (tstr.StartsWith(param))
            {
                tstr = tstr.Substring(param.Length);
            }

            return tstr;
        }

        public static string SQLQueryFormat(this string strTable, string condition = null, bool EmptyWithoutCondition = true)
        {
            string query = "";
            if (!(EmptyWithoutCondition && condition.Empty()))
            {
                if (!strTable.Empty()) query += string.Format("SELECT * FROM [{0}] ", strTable.ToUpper());
                if (!condition.Empty()) query += string.Format(" WHERE ( {0} ) ", condition.ToUpper());
            }
            return query;
        }

        public static string SQLTrimScehma(this string strTable)
        {
            string[] schemas = strTable.SplitEx(".");
            return schemas[schemas.Length - 1];
        }

        public static string RemoveSQLVariableDeclarations(this string query)
        {
            if (!query.Empty())
            {
                string _query = query.ToUpper().Replace(" =", "=").Replace("= ", "=").Replace("=", "= ");

                Int32 FROM_Index = query.IndexOf("FROM");
                string _part = _query.Substring(0, FROM_Index - 1);

                Int32 AT_Index = _part.IndexOf("@");
                if (AT_Index <= 0) return query;

                foreach (string variable in Regex.Matches(_query, @"\@\w*=").OfType<Match>().Select(m => m.ToString()))
                {
                    _query = _query.Replace(variable, "");
                }
                query = _query;
            }

            return query;
        }

        public static bool IsSQL(this string str)
        {
            str = str.ToUpper();

            return !str.Empty() && (str.StartsWith("SELECT") || str.StartsWith("UPDATE") || str.StartsWith("DELETE") || str.StartsWith("BEGIN"));
        }

        public static string SqlQueryType(this string str)
        {
            str = str.ToUpper().Trim();
            if (str.StartsWith("SELECT")) return "SELECT";
            else if (str.StartsWith("UPDATE")) return "UPDATE";
            else if (str.StartsWith("DELETE")) return "DELETE";
            else if (str.StartsWith("BEGIN")) return "TRANSACTION";
            else return "";
        }

        public static string RemoveSpecialCharacters(this string str, char replaceableChar = ' ')
        {
            string _final = "";
            foreach(char c in str)
                _final +=  (char.IsLetterOrDigit(c) ? c : replaceableChar).ToString().Trim();
            return _final;
        }

        #region ^Permutation & combination
        public static string[] PermuteNested(this string pattern, Action<string> action = null)
        {
            List<string> perms = new List<string>();

            var _initial = pattern.Permutate();            
            for (var ind = 1; ind < pattern.Length - 1; ind++)
            {
                var _next = _initial.GroupBy(p => p.Substring(0, ind))
                                    .SelectMany(p => p.Select(s => s.Substring(ind)))
                                    .Distinct();
                perms.AddRange(_next);
            }
            perms.InsertRange(0, _initial);
            perms.AddRange(pattern.Combination());
            if (action != null) perms.ForEach(per => InvokeAction(per));
            
            return perms.ToArray();

            //Local function to invoke Action
            void InvokeAction(string perm)
            {
                action.Invoke(perm);
            }
        }

        private static IEnumerable<string> Permutate(this string source)
        {
            if (source.Length == 1) return new List<string> { source };

            var permutations = from c in source
                               from p in Permutate(new String(source.Where(x => x != c).ToArray()))
                               select c + p;

            return permutations;
        }

        private static List<string> Combination(this string source, int repeat = Int32.MaxValue )
        {
            if (repeat == 1) return new List<string>();
            if (repeat > source.Length) repeat = source.Length;

            var combination = (from c in source
                               select c.ToString().Repeat(repeat)).ToList();
            combination.AddRange(Combination(source, repeat - 1));

            return combination;
        }
        #endregion ~Permutation

        public static bool Match(this string str, string pattern)
        {
            return Regex.IsMatch(str, pattern);            
        }

        /// <summary>
        /// Aligns a text center to any width.
        /// Obvious reason being Text should be less than the Width specified.
        /// </summary>
        /// <param name="text"></param>
        /// <param name="width"></param>
        /// <returns></returns>
        public static string CenteredText(this string text, int width)
        {
            var len = width - text.Length;
            var padlen = ((int)len / 2) + text.Length;
            return text.PadLeft(padlen);
        }
    }
}
