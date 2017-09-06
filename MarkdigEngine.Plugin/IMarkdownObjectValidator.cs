using Markdig.Syntax;

namespace MarkdigEngine.Plugin
{
    public interface IMarkdownObjectValidator
    {
        void Validate(IMarkdownObject markdownObject);
    }
}