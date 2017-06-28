using Markdig.Parsers;

namespace MarkdigEngine
{
    public class InclusionBlockParser : BlockParser
    {
        private const string StartString = "[!include";

        public InclusionBlockParser()
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
            var includeFile = new InclusionBlock(this);

            if (!ExtensionsHelper.MatchStart(ref line, StartString, false))
            {
                return BlockState.None;
            }

            var stringBuilderCache = processor.StringBuilders;
            var context = new InclusionContext();

            if (!ExtensionsHelper.MatchLink(stringBuilderCache, ref line, ref context))
            {
                return BlockState.None;
            }

            includeFile.Context = context;
            processor.NewBlocks.Push(includeFile);

            return BlockState.BreakDiscard;
        }
    }
}
