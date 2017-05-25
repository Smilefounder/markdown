using Markdig.Parsers;
using Markdig.Helpers;

namespace MarkdigEngine
{
    public class IncludeFileInlineParser : InlineParser
    {
        private const string StartString = "[!include";

        public IncludeFileInlineParser()
        {
            OpeningCharacters = new[] { '[' };
        }

        public override bool Match(InlineProcessor processor, ref StringSlice slice)
        {
            if (!ExtensionsHelper.MatchStart(ref slice, StartString))
            {
                return false;
            }

            var includeFile = new IncludeFileInline();
            var context = new IncludeFileContext();
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
