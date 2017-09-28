using Markdig.Syntax;

namespace MarkdigEngine.Extensions
{
    public interface IMarkdownObjectValidator
    {
        void Validate(IMarkdownObject markdownObject);
    }
}