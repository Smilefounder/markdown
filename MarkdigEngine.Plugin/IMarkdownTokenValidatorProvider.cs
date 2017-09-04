using System.Collections.Immutable;

namespace MarkdigEngine.Plugin
{
    public interface IMarkdownTokenValidatorProvider
    {
        ImmutableArray<IMarkdownTokenValidator> GetValidators();
    }
}
