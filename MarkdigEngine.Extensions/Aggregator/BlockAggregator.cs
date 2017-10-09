using Markdig.Syntax;

namespace MarkdigEngine.Extensions
{
    public abstract class BlockAggregator<TBlock> : IBlockAggregator
        where TBlock : class, IBlock
    {
        public bool Aggregate(BlockAggregateContext context)
        {
            var block = context.CurrentBlock as TBlock;
            if (block != null)
            {
                return AggregateCore(block, context);
            }

            return false;
        }

        protected abstract bool AggregateCore(TBlock block, BlockAggregateContext context);
    }
}
