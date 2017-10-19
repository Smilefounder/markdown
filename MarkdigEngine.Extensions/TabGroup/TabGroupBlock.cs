using System.Collections.Immutable;

using Markdig.Syntax;

namespace MarkdigEngine.Extensions
{
    public class TabGroupBlock : ContainerBlock
    {
        public string Id { get; }

        public int ActiveTabIndex { get; }

        public ImmutableArray<TabItemBlock> Items { get; }

        public TabGroupBlock(string id, HeadingBlock headBlock, ImmutableArray<TabItemBlock> blocks, int activeTabIndex) : base(null)
        {
            Id = id;
            ActiveTabIndex = activeTabIndex;
            Items = blocks;
            Line = headBlock.Line;

            foreach (var item in blocks)
            {
                Add(item.Title);
                Add(item.Content);
            }

            var length = blocks.Length;
            Span = new SourceSpan(headBlock.Span.Start, blocks[length-1].Content.Span.End);
        }
    }
}