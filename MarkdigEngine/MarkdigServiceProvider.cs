using System.Composition;

using Microsoft.DocAsCode.Plugins;
using System.Collections.Generic;
using System.Linq;

using MarkdigEngine.Plugin;

namespace MarkdigEngine
{
    [Export("markdig", typeof(IMarkdownServiceProvider))]
    public class MarkdigServiceProvider : IMarkdownServiceProvider
    {
        [ImportMany]
        public IEnumerable<IMarkdigCustomizer> MarkdigCustomizers { get; set; } = Enumerable.Empty<IMarkdigCustomizer>();

        [Import]
        public ICompositionContainer Container { get; set; }

        public IMarkdownService CreateMarkdownService(MarkdownServiceParameters parameters)
        {
            return new MarkdigMarkdownService(parameters, Container, MarkdigCustomizers);
        }
    }
}