using Markdig;
using Microsoft.DocAsCode.Plugins;

namespace MarkdigEngine.Extensions
{
    public interface IMarkdigCompositor
    {
        string Markup(MarkdownContext context, MarkdownServiceParameters parameters);
    }
}
