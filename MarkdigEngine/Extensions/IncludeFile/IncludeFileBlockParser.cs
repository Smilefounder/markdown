using Markdig.Parsers;
using Markdig.Helpers;

namespace MarkdigEngine.Extensions.IncludeFile
{
    public class IncludeFileBlockParser : BlockParser
    {
        private const string StartString = "[!include";

        public IncludeFileBlockParser()
        {
            OpeningCharacters = new char[] { '[' };
        }

        public override BlockState TryOpen(BlockProcessor processor)
        {
            if (processor.IsCodeIndent)
            {
                return BlockState.None;
            }

            // [!include[<title>](<filepath>)]
            var column = processor.Column;
            var line = processor.Line;
            var command = line.ToString();
            var includeFile = new IncludeFile(this);

            if (!MatchStart(ref line))
            {
                return BlockState.None;
            }

            if (!MatchLink(processor, ref line, ref includeFile))
            {
                return BlockState.None;
            }

            processor.NewBlocks.Push(includeFile);

            return BlockState.BreakDiscard;
        }

        private bool MatchStart(ref StringSlice slice)
        {
            if (IsEscaped(slice))
            {
                return false;
            }

            var c = slice.CurrentChar;
            var index = 0;

            while (c != '\0' && index < StartString.Length && c == StartString[index])
            {
                c = slice.NextChar();
                index++;
            }

            return index == StartString.Length;
        }

        private bool MatchLink(BlockProcessor processor, ref StringSlice slice, ref IncludeFile includeFile)
        {
            if (IsEscaped(slice))
            {
                return false;
            }

            return MatchTitle(processor, ref slice, ref includeFile) && MatchPath(processor, ref slice, ref includeFile);
        }

        private bool MatchTitle(BlockProcessor processor, ref StringSlice slice, ref IncludeFile includeFile)
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
            var title = processor.StringBuilders.Get();
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
                includeFile.Title = title.ToString().Trim();
                slice.NextChar();
                return true;
            }

            return false;
        }

        private bool IsEscaped(StringSlice slice)
        {
            return slice.PeekCharExtra(-1) == '\\';
        }

        private bool MatchPath(BlockProcessor processor, ref StringSlice slice, ref IncludeFile includeFile)
        {
            if (slice.CurrentChar != '(')
            {
                return false;
            }

            var c = slice.NextChar();
            var filePath = processor.StringBuilders.Get();
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
                includeFile.RefFilePath = filePath.ToString().Trim();
                slice.NextChar();
                return true;
            }

            return false;
        }
    }
}
