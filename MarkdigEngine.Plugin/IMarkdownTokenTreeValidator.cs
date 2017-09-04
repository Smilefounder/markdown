using Markdig.Syntax;

namespace MarkdigEngine.Plugin
{
    public interface IMarkdownTokenTreeValidator
    {
        void Validate(MarkdownDocument document);
    }
}
