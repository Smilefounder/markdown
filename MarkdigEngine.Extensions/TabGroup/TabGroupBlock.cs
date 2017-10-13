using System.Collections.Immutable;

using Markdig.Syntax;

namespace MarkdigEngine.Extensions
{
    public class TabGroupBlock : LeafBlock
    {
        public string Id { get; }

        public int ActiveTabIndex { get; }

        public ImmutableArray<TabItemBlock> Items { get; }

        public TabGroupBlock(string id, ImmutableArray<TabItemBlock> blocks, int activeTabIndex) : base(null)
        {
            Id = id;
            ActiveTabIndex = activeTabIndex;
            Items = blocks;
        }
    }
}