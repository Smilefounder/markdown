using System.Collections.Immutable;

namespace MarkdigEngine.Plugin
{
    public interface IMarkdownObjectValidatorProvider
    {
        ImmutableArray<IMarkdownObjectValidator> GetValidators();
    }
}
