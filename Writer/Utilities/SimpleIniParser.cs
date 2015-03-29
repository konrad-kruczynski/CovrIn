using System;
using System.IO;
using System.Text.RegularExpressions;

namespace CovrIn.Writer.Utilities
{
    public static class SimpleIniParser
    {
        public static bool ParseBool(string fileName, string optionName, bool defaultValue)
        {
            var regex = new Regex(string.Format(RegexStub, optionName));
            try
            {
                foreach(var line in File.ReadLines(fileName))
                {
                    var match = regex.Match(line);
                    if(match.Success)
                    {
                        var value = match.Groups[1].Value;
                        if(string.Compare(value, "true", true) == 0)
                        {
                            return true;
                        }
                        if(string.Compare(value, "false", true) == 0)
                        {
                            return false;
                        }
                    }
                }
            }
            catch(IOException)
            {
                // in such case we effectively return default value
            }
            return defaultValue;
        }

        public static string ParseString(string fileName, string optionName, string defaultValue)
        {
            var regex = new Regex(string.Format(RegexStub, optionName));
            try
            {
                foreach(var line in File.ReadLines(fileName))
                {
                    var match = regex.Match(line);
                    if(match.Success)
                    {
                        return match.Groups[1].Value;
                    }
                }
            }
            catch(IOException)
            {
                // in such case we effectively return default value
            }
            return defaultValue;
        }

        private const string RegexStub = @"^\s*{0}\s*=\s*(.+)\s*$";
    }
}

