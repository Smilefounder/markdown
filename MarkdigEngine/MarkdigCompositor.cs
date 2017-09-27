using Microsoft.DocAsCode.Plugins;

using Markdig;
using MarkdigEngine.Extensions;

namespace MarkdigEngine
{
    public class MarkdigCompositor : IMarkdigCompositor
    {
        public string Markup(MarkdownContext context, MarkdownServiceParameters parameters)
        {
            var builder = new MarkdownPipelineBuilder()
                                .UseAdvancedExtensions()
                                .UseDfmExtensions(this, context, parameters);
            var pipeline = builder.Build();

            return Markdown.ToHtml(context.Content, pipeline);
        }
    }
}
