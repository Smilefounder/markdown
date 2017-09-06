using Markdig.Syntax;

namespace MarkdigEngine
{
    public interface IMarkdownObjectRewriter
    {
        IMarkdownObject Rewrite(IMarkdownObject markdownObject);
    }
}