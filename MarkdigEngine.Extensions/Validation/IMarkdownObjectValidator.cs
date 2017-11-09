using Markdig.Syntax;

namespace MarkdigEngine.Extensions
{
    public interface IMarkdownObjectValidator
    {
        void PreValidate(IMarkdownObject markdownObject);

        void Validate(IMarkdownObject markdownObject);

        void PostValidate(IMarkdownObject markdownObject);
    }
}