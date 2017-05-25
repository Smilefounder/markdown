using Markdig.Syntax.Inlines;

namespace MarkdigEngine
{
    public class IncludeFileInline : LeafInline
    {
        public IncludeFileContext Context { get; set; }
    }
}
