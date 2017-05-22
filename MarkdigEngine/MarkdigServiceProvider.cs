using Microsoft.DocAsCode.Plugins;
using System.Composition;

namespace MarkdigEngine
{
    [Export("markdig", typeof(IMarkdownServiceProvider))]
    public class MarkdigServiceProvider
        : IMarkdownServiceProvider
    {
        public IMarkdownService CreateMarkdownService(MarkdownServiceParameters parameters)
        {
            return new MarkdigMarkdownService(parameters);
        }
    }
}
