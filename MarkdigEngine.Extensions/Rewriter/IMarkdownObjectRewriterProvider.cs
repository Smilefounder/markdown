using System.Collections.Immutable;

namespace MarkdigEngine.Extensions
{
    public interface IMarkdownObjectRewriterProvider
    {
        ImmutableArray<IMarkdownObjectRewriter> GetRewriters();
    }
}
