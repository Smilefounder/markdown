using System.Composition;

using Microsoft.DocAsCode.Plugins;

namespace MarkdigEngine
{
    [Export("markdig", typeof(IMarkdownServiceProvider))]
    public class MarkdigServiceProvider : IMarkdownServiceProvider
    {
        [Import]
        public ICompositionContainer Container { get; set; }

        public IMarkdownService CreateMarkdownService(MarkdownServiceParameters parameters)
        {
            return new MarkdigMarkdownService(parameters, Container);
        }
    }
}