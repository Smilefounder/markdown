using Markdig.Parsers;
using Markdig.Helpers;

namespace MarkdigEngine.Extensions.IncludeFile
{
    public class IncludeFileBlockParser : BlockParser
    {
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
            var lineStr = line.ToString().TrimEnd();
            var c = line.CurrentChar;

            if (!lineStr.StartsWith("[!include[") || lineStr[lineStr.Length-1] != ']')
            {
                return BlockState.None;
            }

            line = new StringSlice(lineStr, "[!include".Length, line.End - 1);
            if (!LinkHelper.TryParseLabel(ref line, out string title))
            {
                return BlockState.None;
            }

            line.NextChar();
            if (!LinkHelper.TryParseUrl(ref line, out string filePath))
            {
                return BlockState.None;
            }

            var includeFile = new IncludeFile(this)
            {
                Title = title,
                FilePath = filePath,
                Command = lineStr
            };

            processor.NewBlocks.Push(includeFile);

            return BlockState.BreakDiscard;
        }
    }
}
