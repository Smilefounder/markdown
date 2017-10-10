using System.Collections.Immutable;

using Markdig.Syntax;

namespace MarkdigEngine.Extensions
{
    public class TabGroupBlock : LeafBlock
    {
        private string guid;
        private HeadingBlock headBlock;
        private ImmutableArray<TabItemBlock> blocks;

        public TabGroupBlock(HeadingBlock headBlock, string guid, ImmutableArray<TabItemBlock> blocks) : base(null)
        {
            this.headBlock = headBlock;
            this.guid = guid;
            this.blocks = blocks;
        }
    }
}