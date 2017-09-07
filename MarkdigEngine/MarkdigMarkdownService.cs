using System.Collections.Immutable;

using Microsoft.DocAsCode.Plugins;

namespace MarkdigEngine
{
    public class MarkdigMarkdownService : IMarkdownService
    {
        private readonly MarkdownServiceParameters _parameters;
        private readonly ICompositionContainer _container;

        public MarkdigMarkdownService(MarkdownServiceParameters parameters, ICompositionContainer container = null)
        {
            _parameters = parameters;
            _container = container;
        }

        public MarkupResult Markup(string content, string path)
        {
            var context = new MarkdownContext(path, _parameters.BasePath, false, null, null, _container);

            return new MarkupResult
            {
                Html = MarkdigMarked.Markup(content, context, _parameters),
                Dependency = context.Dependency.ToImmutableArray()
            };
        }
    }
}
