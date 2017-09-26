using Markdig.Parsers;
using Markdig.Helpers;

namespace MarkdigEngine.Extensions
{
    public class InclusionInlineParser : InlineParser
    {
        private const string StartString = "[!include";

        public InclusionInlineParser()
        {
            OpeningCharacters = new[] { '[' };
        }

        public override bool Match(InlineProcessor processor, ref StringSlice slice)
        {
            if (!ExtensionsHelper.MatchStart(ref slice, StartString, false))
            {
                return false;
            }
            else
            {
                if(slice.CurrentChar == '-')
                {
                    slice.NextChar();
                }
            }

            var includeFile = new InclusionInline();
            var context = new InclusionContext();
            var stringBuilderCache = processor.StringBuilders;

            if (!ExtensionsHelper.MatchLink(stringBuilderCache, ref slice, ref context))
            {
                return false;
            }

            includeFile.Context = context;
            processor.Inline = includeFile;

            return true;
        }
    }
}
