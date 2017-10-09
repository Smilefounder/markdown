namespace MarkdigEngine.Extensions
{
    public interface IBlockAggregator
    {
        bool Aggregate(BlockAggregateContext context);
    }
}
