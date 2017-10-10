using System.Collections.Immutable;
using Markdig.Syntax;

namespace MarkdigEngine.Extensions
{
    public class TabContentBlock
    {
        private HeadingBlock block;
        private ImmutableArray<Block> blocks;

        public TabContentBlock(HeadingBlock block, ImmutableArray<Block> blocks)
        {
            this.block = block;
            this.blocks = blocks;
        }
    }
}