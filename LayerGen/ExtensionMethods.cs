using System;
using System.IO;
using System.Linq;
using System.Text;

namespace LayerGen
{
    public static class ExtensionMethods
    {
        /// <summary>
        /// Clears out a <see cref="System.Text.StringBuilder"/> object.
        /// </summary>
        /// <param name="value">The <see cref="System.Text.StringBuilder"/> object</param>
        public static void Clear(this StringBuilder value)
        {
            value.Length = 0;
        }

        /// <summary>
        /// Replaces all the text in a <see cref="System.Text.StringBuilder"/> object
        /// with the specified text.
        /// </summary>
        /// <param name="value">The <see cref="System.Text.StringBuilder"/> object</param>
        /// <param name="newValue">The text to replace it with.</param>
        public static void ReplaceAllText(this StringBuilder value, string newValue)
        {
            value.Clear();
            value.Append(newValue);
        }

        /// <summary>
        /// Searches the <see cref="System.Text.StringBuilder"/> object for the
        /// specified text.
        /// </summary>
        /// <param name="sb">The <see cref="System.Text.StringBuilder"/> object</param>
        /// <param name="value">The value to search for.</param>
        /// <param name="startIndex">The index in which to start the search.</param>
        /// <returns>The zero-based position in which the text appears, or -1 if it wasn't found.</returns>
        public static int IndexOf(this StringBuilder sb, string value, int startIndex)
        {
            int length = value.Length;
            int maxSearchLength = (sb.Length - length) + 1;

            for (int i = startIndex; i < maxSearchLength; ++i)
            {
                if (sb[i] == value[0])
                {
                    int index = 1;
                    while ((index < length) && (sb[i + index] == value[index]))
                        ++index;

                    if (index == length)
                        return i;
                }
            }

            return -1;
        }

        /// <summary>
        /// Searches the <see cref="System.Text.StringBuilder"/> object for the
        /// specified text.
        /// </summary>
        /// <param name="sb">The <see cref="System.Text.StringBuilder"/> object</param>
        /// <param name="value">The value to search for.</param>
        /// <returns>The zero-based position in which the text appears, or -1 if it wasn't found.</returns>
        public static int IndexOf(this StringBuilder sb, string value)
        {
            return IndexOf(sb, value, 0);
        }

        /// <summary>
        /// Returns a string with the first character converted to upper case.
        /// </summary>
        /// <param name="str">The string to capitalize</param>
        /// <returns>The string, with the first character converted to upper case.</returns>
        public static string FirstCharToUpper(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return "";
            if (str.Length == 1)
                return str.ToUpper();
            return str.First().ToString().ToUpper() + str.Substring(1);
        }

        /// <summary>
        /// Converts the string to a valid C# namespace name
        /// </summary>
        /// <param name="str">The string to convert</param>
        /// <returns>A proper C# namespace name</returns>
        public static string ConvertToValidCSharpNamespace(this string str)
        {
            string[] parts = str.Split('.');
            string namespaceName = parts.Aggregate("", (current, part) => current + DatabasePlugins.Common.GetSafeCsName(part) + ".");

            namespaceName = namespaceName.Trim('.');
            return namespaceName;
        }

        /// <summary>
        /// Converts the string to a valid Vb.Net namespace name
        /// </summary>
        /// <param name="str">The string to convert</param>
        /// <returns>A proper Vb.Net namespace name</returns>
        public static string ConvertToValidVbNamespace(this string str)
        {
            string[] parts = str.Split('.');
            string namespaceName = parts.Aggregate("", (current, part) => current + DatabasePlugins.Common.GetSafeVbName(part) + ".");

            namespaceName = namespaceName.Trim('.');
            return namespaceName;
        }

        /// <summary>
        /// Takes a string and formats in a way that is appropriate for a filename.
        /// </summary>
        /// <param name="str">The string to format</param>
        /// <returns>The formatted string, appropriate to use as a filename.</returns>
        public static string ToProperFileName(this string str)
        {
            bool lastUpper = false;

            if (string.IsNullOrEmpty(str))
                return "";
            if (str.Length == 1)
            {
                return Path.GetInvalidFileNameChars().Contains(str[0]) ? "_" : str.ToUpper();
            }
                
            str = char.ToUpper(str[0]) + str.Substring(1, str.Length - 1).Trim();

            string newStr = "";
            for (int i = 0; i < str.Length; i++)
            {
                if (str[i] == ' ')
                {
                    while (str[i] == ' ')
                    {
                        i += 1;
                    }
                    newStr = Path.GetInvalidFileNameChars().Contains(str[i])
                        ? newStr + "_"
                        : newStr + char.ToUpper(str[i]);
                    if (char.IsUpper(str[i]))
                        lastUpper = true;
                }
                else
                {
                    if (char.IsUpper(str[i]) && lastUpper)
                    {
                        if (Path.GetInvalidFileNameChars().Contains(str[i]))
                        {
                            newStr = newStr + "_";
                            lastUpper = false;
                        }
                        else
                        {
                            newStr = newStr + char.ToLower(str[i]);
                        }
                    }
                    else
                    {
                        lastUpper = char.IsUpper(str[i]);

                        newStr = Path.GetInvalidFileNameChars().Contains(str[i])
                            ? newStr + "_"
                            : newStr + str[i];
                        if (Path.GetInvalidFileNameChars().Contains(str[i]))
                        {
                            if (i >= str.Length - 1)
                                continue;
                            str = str.ModifyOneChar(char.ToUpper(str[i + 1]), i + 1);
                        }
                    }
                }
            }
            return newStr;
        }

        /// <summary>
        /// Modifies one character in a string
        /// </summary>
        /// <param name="str">The string to be modified</param>
        /// <param name="character">The new character that will be replaced in the string</param>
        /// <param name="index">The zero-based index of the character to modify.</param>
        /// <returns>The modified string</returns>
        private static string ModifyOneChar(this string str, char character, int index)
        {
            if (string.IsNullOrEmpty(str))
                return "";
            if (index >= str.Length)
                throw new Exception("Index is outside range of string length!");
            var sb = new StringBuilder(str);
            sb[index] = character;
            return sb.ToString();
        }
    }
}
