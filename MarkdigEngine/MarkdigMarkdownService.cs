using System.Collections.Generic;
using System.Collections.Immutable;

using MarkdigEngine.Plugin;

using Microsoft.DocAsCode.Plugins;

namespace MarkdigEngine
{
    public class MarkdigMarkdownService : IMarkdownService
    {
        private readonly MarkdownServiceParameters _parameters;
        private readonly MarkdownValidatorBuilder _mvb;
        private readonly ImmutableArray<IMarkdigCustomizer> _markdigCustomizers;

        public MarkdigMarkdownService(
            MarkdownServiceParameters parameters,
            ICompositionContainer container,
            IEnumerable<IMarkdigCustomizer> markdigCustomizers)
        {
            _parameters = parameters;
            _mvb = MarkdownValidatorBuilder.Create(parameters, container);
            _markdigCustomizers = markdigCustomizers.ToImmutableArray();
        }

        public MarkupResult Markup(string content, string path)
        {
            var context = new MarkdownContext(path, _parameters.BasePath, _mvb, false, null, null);

            return new MarkupResult
            {
                Html = MarkdigMarked.Markup(content, context, _parameters),
                Dependency = context.Dependency.ToImmutableArray()
            };
        }
    }
}
