using System.Collections.Immutable;

namespace MarkdigEngine.Extensions
{
    public interface IMarkdownObjectValidatorProvider
    {
        ImmutableArray<IMarkdownObjectValidator> GetValidators();
    }
}
