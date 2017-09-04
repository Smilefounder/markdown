using Markdig.Syntax;

namespace MarkdigEngine.Plugin
{
    public interface IMarkdownTokenValidator
    {
        void Validate(MarkdownDocument document);
    }
}