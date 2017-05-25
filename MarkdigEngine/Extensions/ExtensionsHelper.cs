using System;
using System.IO;

using Markdig.Helpers;

namespace MarkdigEngine
{
    public static class ExtensionsHelper
    {
        public static string GetAbsolutePathOfRefFile(string basePath, string filePath, string refPath)
        {
            if (refPath[0] == '~')
            {
                return GetAbsolutePathWithTilde(basePath, refPath);
            }

            var currentDirectory = Path.GetDirectoryName(filePath);

            return NormalizePath(Path.Combine(basePath, currentDirectory, refPath));
        }

        public static string GetAbsolutePathWithTilde(string basePath, string tildePath)
        {
            if (string.IsNullOrEmpty(tildePath))
            {
                throw new ArgumentException($"{nameof(tildePath)} can't be null or empty.");
            }

            if (string.IsNullOrEmpty(basePath))
            {
                throw new ArgumentException($"{nameof(basePath)} can't be null or empty.");
            }

            if (!tildePath.StartsWith("~"))
            {
                throw new ArgumentException($"{nameof(tildePath)} should start with ~.");
            }

            if (!Path.IsPathRooted(basePath))
            {
                throw new ArgumentException($"{nameof(basePath)} should be an absolute path.");
            }

            return GetAbsolutePathWithTildeCore(basePath, tildePath);
        }

        public static string NormalizePath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return path;
            }

            return Path.GetFullPath(path).Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }

        public static bool MatchStart(ref StringSlice slice, string startString)
        {
            if (IsEscaped(slice))
            {
                return false;
            }

            var c = slice.CurrentChar;
            var index = 0;

            while (c != '\0' && index < startString.Length && c == startString[index])
            {
                c = slice.NextChar();
                index++;
            }

            return index == startString.Length;
        }

        public static bool MatchLink(StringBuilderCache stringBuilderCache, ref StringSlice slice, ref IncludeFileContext context)
        {
            if (IsEscaped(slice))
            {
                return false;
            }

            if (MatchTitle(stringBuilderCache, ref slice, ref context) && MatchPath(stringBuilderCache, ref slice, ref context))
            {
                slice.NextChar();
                return true;
            }

            return false;
        }



        #region private methods
        private static string GetAbsolutePathWithTildeCore(string basePath, string tildePath)
        {
            var index = 1;
            var ch = tildePath[index];
            while (ch == '/' || ch == '\\')
            {
                index++;
                ch = tildePath[index];
            }

            if (index == tildePath.Length)
            {
                return basePath;
            }

            var pathWithoutTilde = tildePath.Substring(index);

            return NormalizePath(Path.Combine(basePath, pathWithoutTilde));
        }

        private static bool MatchTitle(StringBuilderCache stringBuilderCache, ref StringSlice slice, ref IncludeFileContext context)
        {
            if (IsEscaped(slice))
            {
                return false;
            }

            if (slice.CurrentChar != '[')
            {
                return false;
            }

            var c = slice.NextChar();
            var title = stringBuilderCache.Get();
            var hasExcape = false;

            while (c != '\0' && (c != ']' || hasExcape))
            {
                if (c == '\\' && !hasExcape)
                {
                    hasExcape = true;
                }
                else
                {
                    title.Append(c);
                    hasExcape = false;
                }
                c = slice.NextChar();
            }

            if (c == ']')
            {
                context.Title = title.ToString().Trim();
                slice.NextChar();
                stringBuilderCache.Release(title);
                return true;
            }

            stringBuilderCache.Release(title);
            return false;
        }

        private static bool IsEscaped(StringSlice slice)
        {
            return slice.PeekCharExtra(-1) == '\\';
        }

        private static bool MatchPath(StringBuilderCache stringBuilderCache, ref StringSlice slice, ref IncludeFileContext context)
        {
            if (slice.CurrentChar != '(')
            {
                return false;
            }

            var c = slice.NextChar();
            var filePath = stringBuilderCache.Get();
            var hasEscape = false;

            while (c != '\0' && (c != ')' || hasEscape))
            {
                if (c == '\\' && !hasEscape)
                {
                    hasEscape = true;
                }
                else
                {
                    filePath.Append(c);
                    hasEscape = false;
                }
                c = slice.NextChar();
            }

            if (c == ')')
            {
                context.RefFilePath = filePath.ToString().Trim();
                slice.NextChar();
                stringBuilderCache.Release(filePath);
                return true;
            }

            stringBuilderCache.Release(filePath);
            return false;
        }
        #endregion
    }
}
