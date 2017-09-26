using Markdig.Syntax.Inlines;

namespace MarkdigEngine.Extensions
{
    public class InclusionInline : LeafInline
    {
        public InclusionContext Context { get; set; }
    }
}
