using Markdig.Syntax;

namespace MarkdigEngine.Plugin
{
    public interface IMarkdownObjectRewriter
    {
        void PreProcess(IMarkdownObject markdownObject);

        IMarkdownObject Rewrite(IMarkdownObject markdownObject);

        void PostProcess(IMarkdownObject markdownObject);
    }
}