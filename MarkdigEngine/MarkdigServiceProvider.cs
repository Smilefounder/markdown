using System.Composition;

using Microsoft.DocAsCode.Plugins;

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