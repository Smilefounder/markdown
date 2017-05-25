using Markdig.Syntax.Inlines;

namespace MarkdigEngine
{
    public class InclusionInline : LeafInline
    {
        public InclusionContext Context { get; set; }
    }
}
