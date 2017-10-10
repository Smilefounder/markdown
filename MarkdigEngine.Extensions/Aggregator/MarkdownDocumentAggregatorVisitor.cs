using Markdig.Syntax;

namespace MarkdigEngine.Extensions
{
    public class MarkdownDocumentAggregatorVisitor
    {
        private readonly IBlockAggregator _aggregator;

        public MarkdownDocumentAggregatorVisitor(IBlockAggregator aggregator)
        {
            _aggregator = aggregator;
        }

        public void Visit(MarkdownDocument document)
        {
            if (_aggregator == null)
            {
                return;
            }

            VisitContainerBlock(document);
        }

        private void VisitContainerBlock(ContainerBlock blocks)
        {
            for (var i = 0; i < blocks.Count; i++)
            {
                var block = blocks[i];
                if (block is ContainerBlock containerBlock)
                {
                    VisitContainerBlock(containerBlock);
                }

                var context = new BlockAggregateContext(blocks);
                Aggregate(context);
            }
        }

        private void Aggregate(BlockAggregateContext context)
        {
            while (context.NextBlock())
            {
                _aggregator.Aggregate(context);
            }
        }
    }
}
