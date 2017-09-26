using System.Collections.Immutable;

using MarkdigEngine.Extensions;

using Microsoft.DocAsCode.Plugins;

namespace MarkdigEngine
{
    public class MarkdigMarkdownService : IMarkdownService
    {
        private readonly MarkdownServiceParameters _parameters;
        private readonly MarkdownValidatorBuilder _mvb;

        public MarkdigMarkdownService(
            MarkdownServiceParameters parameters,
            ICompositionContainer container = null)
        {
            _parameters = parameters;
            _mvb = MarkdownValidatorBuilder.Create(parameters, container);
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
