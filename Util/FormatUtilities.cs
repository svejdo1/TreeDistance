//    Copyright (C) 2012  Mateusz Pawlik and Nikolaus Augsten
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU Affero General Public License as
//    published by the Free Software Foundation, either version 3 of the
//    License, or (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU Affero General Public License for more details.
//
//    You should have received a copy of the GNU Affero General Public License
//    along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Text;

namespace Barbar.TreeDistance.Util
{

    /**
     * A set of formatting utilities.
     * 
     * @author Nikolaus Augsten from approxlib, available at http://www.inf.unibz.it/~augsten/src/ modified by Mateusz Pawlik
     */
    public class FormatUtilities
    {

        /**
         * Parses line and returns contents of field number fieldNr, where fields
         * are separated by seperator.
         * 
         * @param fieldNr number of field you want to get; numbering starts with 0
         * @param line string to parse
         * @param seperator field seperator
         * @return if the field exists, the value of the field without 
         *         leading and tailing spaces is returned; if the field does not
         *         exist or the parameter line is null than null is returned
         */
        public static string getField(int fieldNr, string line,
            char seperator)
        {
            if (line != null)
            {
                int pos = 0;
                for (int i = 0; i < fieldNr; i++)
                {
                    pos = line.IndexOf(seperator, pos);
                    if (pos == -1)
                    {
                        return null;
                    }
                    pos = pos + 1;
                }
                int pos2 = line.IndexOf(seperator, pos);
                string res;
                if (pos2 == -1)
                {
                    res = line.Substring(pos);
                }
                else
                {
                    res = line.JavaSubstring(pos, pos2);
                }
                return res.Trim();
            }
            else
            {
                return null;
            }
        }

        /**
         * 
         * @param line
         * @param separator
         * @return new string[0] for empty or null lines, string-array containing fields, otherwise
         */
        public static string[] getFields(string line,
            char separator)
        {
            if ((line != null) && (!line.Equals("")))
            {
                StringBuilder field = new StringBuilder();
                var fieldArr = new List<string>();
                for (int i = 0; i < line.Length; i++)
                {
                    char ch = line[i];
                    if (ch == separator)
                    {
                        fieldArr.Add(field.ToString().Trim());
                        field = new StringBuilder();
                    }
                    else
                    {
                        field.Append(ch);
                    }
                }
                fieldArr.Add(field.ToString().Trim());
                return fieldArr.ToArray();
            }
            else
            {
                return new string[0];
            }
        }

        public static string[] getFields(string line, char separator, char quote)
        {
            string[] parse = getFields(line, separator);
            for (int i = 0; i < parse.Length; i++)
            {
                parse[i] = stripQuotes(parse[i], quote);
            }
            return (parse);
        }

        public static string stripQuotes(string s, char quote)
        {
            if ((s.Length >= 2) && (s[0] == quote) && (s[s.Length - 1] == quote))
            {
                return s.JavaSubstring(1, s.Length - 1);
            }
            else
            {
                return s;
            }
        }

        public static string resizeEnd(string s, int size)
        {
            return resizeEnd(s, size, ' ');
        }

        public static string getRandomString(int length)
        {
            var r = new Random();
            string str = "";
            for (int i = 0; i < length; i++)
            {
                str += (char)(65 + r.Next(26));
            }
            return str;
        }

        public static string resizeEnd(string s, int size, char fillChar)
        {
            string res;
            try
            {
                res = s.JavaSubstring(0, size);
            }
            catch (IndexOutOfRangeException)
            {
                res = s;
                for (int i = s.Length; i < size; i++)
                {
                    res += fillChar;
                }
            }
            return res;
        }

        public static string resizeFront(string s, int size)
        {
            return resizeFront(s, size, ' ');
        }

        public static string resizeFront(string s, int size, char fillChar)
        {
            string res;
            try
            {
                res = s.JavaSubstring(0, size);
            }
            catch (IndexOutOfRangeException)
            {
                res = s;
                for (int i = s.Length; i < size; i++)
                {
                    res = fillChar + res;
                }
            }
            return res;
        }

        public static int matchingBracket(string s, int pos)
        {
            if ((s == null) || (pos > s.Length - 1))
            {
                return -1;
            }
            char open = s[pos];
            char close;
            switch (open)
            {
                case '{':
                    close = '}';
                    break;
                case '(':
                    close = ')';
                    break;
                case '[':
                    close = ']';
                    break;
                case '<':
                    close = '>';
                    break;
                default:
                    return -1;
            }

            pos++;
            int count = 1;
            while ((count != 0) && (pos < s.Length))
            {
                if (s[pos] == open)
                {
                    count++;
                }
                else if (s[pos] == close)
                {
                    count--;
                }
                pos++;
            }
            if (count != 0)
            {
                return -1;
            }
            else
            {
                return pos - 1;
            }
        }


        public static int getTreeID(string s)
        {
            if ((s != null) && (s.Length > 0))
            {
                int end = s.IndexOf(':', 1);
                if (end == -1)
                {
                    return -1;
                }
                else
                {
                    return int.Parse(s.JavaSubstring(0, end));
                }
            }
            else
            {
                return -1;
            }
        }

        public static string getRoot(string s)
        {
            if ((s != null) && (s.Length > 0) && s.StartsWith("{") && s.EndsWith("}"))
            {
                int end = s.IndexOf('{', 1);
                if (end == -1)
                {
                    end = s.IndexOf('}', 1);
                }
                return s.JavaSubstring(1, end);
            }
            else
            {
                return null;
            }
        }

        public static List<string> getChildren(string s)
        {
            if ((s != null) && (s.Length > 0) && s.StartsWith("{") && s.EndsWith("}"))
            {
                var children = new List<string>();
                int end = s.IndexOf('{', 1);
                if (end == -1)
                {
                    return children;
                }
                string rest = s.JavaSubstring(end, s.Length - 1);
                int match = 0;
                while ((rest.Length > 0) && ((match = matchingBracket(rest, 0)) != -1))
                {
                    children.Add(rest.JavaSubstring(0, match + 1));
                    if (match + 1 < rest.Length)
                    {
                        rest = rest.Substring(match + 1);
                    }
                    else
                    {
                        rest = "";
                    }
                }
                return children;
            }
            else
            {
                return null;
            }
        }

        public static string parseTree(string s, List<string> children)
        {
            children.Clear();
            string root;
            if ((s != null) && (s.Length > 0) && s.StartsWith("{") && s.EndsWith("}"))
            {
                int end = s.IndexOf('{', 1);
                if (end == -1)
                {
                    end = s.IndexOf('}', 1);
                    return s.JavaSubstring(1, end);
                }
                root = s.JavaSubstring(1, end);
                string rest = s.JavaSubstring(end, s.Length - 1);
                int match = 0;
                while ((rest.Length > 0) && (match = matchingBracket(rest, 0)) != -1)
                {
                    children.Add(rest.JavaSubstring(0, match + 1));
                    if (match + 1 < rest.Length)
                    {
                        rest = rest.Substring(match + 1);
                    }
                    else
                    {
                        rest = "";
                    }
                }
                return root;
            }
            else
            {
                return null;
            }
        }

        public static string commaSeparatedList(string[] list)
        {
            var s = new StringBuilder();
            for (int i = 0; i < list.Length; i++)
            {
                s.Append(list[i]);
                if (i != list.Length - 1)
                {
                    s.Append(",");
                }
            }
            return s.ToString();
        }

        /**
         * Encloses the strings of a list in quotes and separates them with
         * a comma.
         * 
         * @param list
         * @param quote
         * @return
         */
        public static string commaSeparatedList(string[] list, char quote)
        {
            StringBuilder s = new StringBuilder();
            for (int i = 0; i < list.Length; i++)
            {
                s.Append(quote + list[i] + quote);
                if (i != list.Length - 1)
                {
                    s.Append(",");
                }
            }
            return s.ToString();
        }

        /**
         * Replaces the numbers 0..9 in a string with the 
         * respective English word. All other characters will
         * not be changed, e.g. '12.3' --> 'onetwo.three'. 
         * @param num input string with numeric characters
         * @return num with numeric characters replaced
         */
        public static string spellOutNumber(string num)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < num.Length; i++)
            {
                char ch = num[i];
                switch (ch)
                {
                    case '0': sb.Append("zero"); break;
                    case '1': sb.Append("one"); break;
                    case '2': sb.Append("two"); break;
                    case '3': sb.Append("three"); break;
                    case '4': sb.Append("four"); break;
                    case '5': sb.Append("five"); break;
                    case '6': sb.Append("six"); break;
                    case '7': sb.Append("seven"); break;
                    case '8': sb.Append("eight"); break;
                    case '9': sb.Append("nine"); break;
                    default: sb.Append(ch); break;
                }
            }
            return sb.ToString();
        }

        public static string substituteBlanks(string s, string subst)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] != ' ')
                {
                    sb.Append(s[i]);
                }
                else
                {
                    sb.Append(subst);
                }
            }
            return sb.ToString();
        }

        public static string escapeLatex(string s)
        {
            var sb = new StringBuilder();
            foreach(char cr in s)
            {
                var c = cr.ToString();
                if (c.Equals("#"))
                {
                    c = "\\#";
                }
                if (c.Equals("&"))
                {
                    c = "\\&";
                }
                if (c.Equals("$"))
                {
                    c = "\\$";
                }
                if (c.Equals("_"))
                {
                    c = "\\_";
                }
                sb.Append(c);
            }
            return sb.ToString();

        }

    }

}