using System.Collections.Immutable;
using Markdig.Syntax;

namespace MarkdigEngine.Extensions
{
    public class TabContentBlock : LeafBlock
    {
        public ImmutableArray<Block> Content { get; }

        public TabContentBlock(ImmutableArray<Block> content) : base(null)
        {
            Content = content;
        }
    }
}