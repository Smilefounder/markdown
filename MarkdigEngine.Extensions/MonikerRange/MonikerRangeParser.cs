using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Syntax;
using Microsoft.DocAsCode.Common;

namespace MarkdigEngine.Extensions
{
    public class MonikerRangeParser : BlockParser
    {
        private const string StartString = "moniker";
        private const string EndString = "moniker-end";
        private const char Colon = ':';
        public MonikerRangeParser()
        {
            OpeningCharacters = new[] { ':' };
        }

        public override BlockState TryOpen(BlockProcessor processor)
        {
            if (processor.IsCodeIndent)
            {
                return BlockState.None;
            }

            if (ExtensionsHelper.IsEscaped(processor.Line))
            {
                return BlockState.None;
            }

            var column = processor.Column;
            var sourcePosition = processor.Start;
            var colonCount = 0;

            var c = processor.CurrentChar;
            while (c == Colon)
            {
                c = processor.NextChar();
                colonCount++;
            }

            if (colonCount < 3) return BlockState.None;

            SkipSapces(processor);

            if (!ExtensionsHelper.MatchStart(processor, "moniker", false))
            {
                return BlockState.None;
            }

            SkipSapces(processor);

            if (!ExtensionsHelper.MatchStart(processor, "range=\"", false))
            {
                return BlockState.None;
            }

            var range = processor.StringBuilders.Get();
            c = processor.CurrentChar;

            while (c != '"')
            {
                range.Append(c);
                c = processor.NextChar();
            }

            if (c != '"')
            {
                return BlockState.None;
            }

            c = processor.NextChar();
            while (c.IsSpace())
            {
                c = processor.NextChar();
            }

            if (!c.IsZero())
            {
                Logger.LogWarning($"MonikerRange have some invalid chars in the starting.");
            }

            processor.NewBlocks.Push(new MonikerRangeBlock(this)
            {
                MonikerRange = range.ToString(),
                ColonCount = colonCount,
                Column = column,
                Span = new SourceSpan(sourcePosition, processor.Line.End),
            });

            processor.StringBuilders.Release(range);

            return BlockState.Continue;
        }

        public override BlockState TryContinue(BlockProcessor processor, Block block)
        {
            if (processor.IsBlankLine)
            {
                return BlockState.Continue;
            }

            var monikerRange = (MonikerRangeBlock)block;

            SkipSapces(processor);

            if(!ExtensionsHelper.MatchStart(processor, new string(':', monikerRange.ColonCount)))
            {
                return BlockState.Continue;
            }

            SkipSapces(processor);

            if (!ExtensionsHelper.MatchStart(processor, "moniker-end", false))
            {
                return BlockState.Continue;
            }

            var c = SkipSapces(processor);

            if (!c.IsZero())
            {
                Logger.LogWarning($"MonikerRange have some invalid chars in the ending.");
            }
            block.UpdateSpanEnd(processor.Line.End);

            return BlockState.Break;
        }

        public char SkipSapces(BlockProcessor processor)
        {
            var c = processor.CurrentChar;

            while (c.IsSpaceOrTab())
            {
                c = processor.NextChar();
            }

            return c;
        }
    }
}
