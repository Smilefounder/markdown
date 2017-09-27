﻿using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using Markdig.Helpers;
using Markdig.Renderers;
using Microsoft.DocAsCode.Common;

namespace MarkdigEngine.Extensions
{
    public static class ExtensionsHelper
    {
        public static string NormalizePath(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return path;
            }

            return Path.GetFullPath(path).Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        }

        public static bool MatchStart(ref StringSlice slice, string startString, bool isCaseSensitive = true)
        {
            if (IsEscaped(slice))
            {
                return false;
            }

            var c = slice.CurrentChar;
            var index = 0;

            while (c != '\0' && index < startString.Length && CharEqual(c, startString[index], isCaseSensitive))
            {
                c = slice.NextChar();
                index++;
            }

            return index == startString.Length;
        }

        public static bool MatchLink(StringBuilderCache stringBuilderCache, ref StringSlice slice, ref InclusionContext context)
        {
            if (IsEscaped(slice))
            {
                return false;
            }

            if (MatchTitle(stringBuilderCache, ref slice, ref context) && MatchPath(ref slice, ref context))
            {
                slice.NextChar();
                return true;
            }

            return false;
        }

        public static void SkipWhitespace(ref StringSlice slice)
        {
            var c = slice.CurrentChar;
            while (c != '\0' && c.IsWhitespace())
            {
                c = slice.NextChar();
            }
        }

        public static void GenerateNodeWithCommentWrapper(HtmlRenderer htmlRenderer, string tag, string message, string rawContent, int lineNumber)
        {
            Logger.LogWarning($"At line {lineNumber}: {message}");
            htmlRenderer.Write("<!-- BEGIN ");
            htmlRenderer.WriteEscape(tag).Write(": ");
            htmlRenderer.WriteEscape(message).Write(" -->");
            htmlRenderer.WriteEscape(rawContent);
            htmlRenderer.Write("<!--END ").WriteEscape(tag).Write(" -->");
        }

        public static string TryGetStringBeforeChars(IEnumerable<char> chars, ref StringSlice slice, bool breakOnWhitespace = false)
        {
            StringSlice savedSlice = slice;
            var c = slice.CurrentChar;
            var hasEscape = false;
            var builder = StringBuilderCache.Local();

            while (c != '\0' && (!breakOnWhitespace || !c.IsWhitespace()) && (hasEscape || !chars.Contains(c)))
            {
                if (c == '\\' && !hasEscape)
                {
                    hasEscape = true;
                }
                else
                {
                    builder.Append(c);
                    hasEscape = false;
                }
                c = slice.NextChar();
            }

            if (c == '\0' && !chars.Contains('\0'))
            {
                slice = savedSlice;
                builder.Length = 0;
                return null;
            }
            else
            {
                var result = builder.ToString().Trim();
                builder.Length = 0;
                return result;
            }
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

        private static bool CharEqual(char ch1, char ch2, bool isCaseSensitive)
        {
            return isCaseSensitive ? ch1 == ch2 : Char.ToLower(ch1) == Char.ToLower(ch2);
        }

        private static bool MatchTitle(StringBuilderCache stringBuilderCache, ref StringSlice slice, ref InclusionContext context)
        {
            if (IsEscaped(slice))
            {
                return false;
            }

            while (slice.CurrentChar == ' ')
            {
                slice.NextChar();
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

        private static bool MatchPath(ref StringSlice slice, ref InclusionContext context)
        {
            if (slice.CurrentChar != '(')
            {
                return false;
            }

            slice.NextChar();
            SkipWhitespace(ref slice);

            string includedFilePath;
            if (slice.CurrentChar == '<')
            {
                includedFilePath = TryGetStringBeforeChars(new char[] { ')', '>' }, ref slice, breakOnWhitespace: true);
            }
            else
            {
                includedFilePath = TryGetStringBeforeChars(new char[] { ')' }, ref slice, breakOnWhitespace: true);
            };

            if (includedFilePath == null)
            {
                return false;
            }

            if(includedFilePath.Length >= 1 && includedFilePath.First() == '<' && slice.CurrentChar == '>')
            {
                includedFilePath = includedFilePath.Substring(1, includedFilePath.Length - 1).Trim();
            }

            if (slice.CurrentChar == ')')
            {
                context.IncludedFilePath = includedFilePath;
                slice.NextChar();
                return true;
            }
            else
            {
                var title = TryGetStringBeforeChars(new char[] { ')' }, ref slice, breakOnWhitespace: false);
                if (title == null)
                {
                    return false;
                }
                else
                {
                    if (title.Length >= 2 && title.First() == '\'' && title.Last() == '\'')
                    {
                        title = title.Substring(1, title.Length - 2).Trim();
                    }
                    else if(title.Length >= 2 && title.First() == '\"' && title.Last() == '\"')
                    {
                        title = title.Substring(1, title.Length - 2).Trim();
                    }
                    context.IncludedFilePath = includedFilePath;
                    slice.NextChar();
                    return true;
                }
            }
        }
        #endregion
    }
}