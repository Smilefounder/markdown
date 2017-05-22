using Markdig;
using Microsoft.DocAsCode.Plugins;

namespace MarkdigEngine
{
    public class MarkdigMarkdownService : IMarkdownService
    {
        private readonly MarkdownServiceParameters _parameters;
        private string m_BasePath;

        public MarkdigMarkdownService(MarkdownServiceParameters parameters)
        {
            _parameters = parameters;
            m_BasePath = parameters.BasePath;
        }

        public MarkupResult Markup(string src, string path)
        {
            var piplineBuilder = new MarkdownPipelineBuilder();
            piplineBuilder.Extensions.Add(new CodeSnippetExtension(m_BasePath, path));

            var pipline = piplineBuilder.Build();

            return new MarkupResult { Html = Markdown.ToHtml(src, pipline) };
        }
    }
}
