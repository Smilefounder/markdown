using Markdig;
using Microsoft.DocAsCode.Plugins;

namespace MarkdigEngine
{
    public class MarkdigMarkdownService : IMarkdownService
    {
        private readonly MarkdownServiceParameters _parameters;

        public MarkdigMarkdownService(MarkdownServiceParameters parameters)
        {
            _parameters = parameters;
        }

        public MarkupResult Markup(string src, string path)
        {
            var context = new MarkdownContext
            {
                FilePath = path,
                BasePath = _parameters.BasePath
            };

            return new MarkupResult
            {
                Html = MarkdigMarked.Markup(src, context)
            };
        }
    }
}
