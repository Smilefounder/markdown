using System.Collections.Immutable;

namespace MarkdigEngine.Plugin
{
    public interface IMarkdownObjectRewriterProvider
    {
        ImmutableArray<IMarkdownObjectRewriter> GetRewriters();
    }
}
