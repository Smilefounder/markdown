using System.Collections.Immutable;

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

        public MarkupResult Markup(string content, string path)
        {
            bool enableSourceInfo = _parameters?.Extensions != null && _parameters.Extensions.ContainsKey(LineNumberExtension.EnableSourceInfo) ?
                                   (bool)_parameters.Extensions[LineNumberExtension.EnableSourceInfo] :
                                   false;

            var context = new MarkdownContext(path, _parameters.BasePath, enableSourceInfo);

            return new MarkupResult
            {
                Html = MarkdigMarked.Markup(content, context),
                Dependency = context.Dependency.ToImmutableArray()
            };
        }
    }
}
