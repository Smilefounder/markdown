using Markdig.Syntax;
using Markdig.Syntax.Inlines;


namespace MarkdigEngine.Extensions
{
    public class TabTitleBlock : LeafBlock
    {
        public LinkInline Content { get; }

        public TabTitleBlock(LinkInline content) : base(null)
        {
            Content = content;
        }
    }
}