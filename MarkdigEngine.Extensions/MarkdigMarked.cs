using System;

using Markdig;
using Microsoft.DocAsCode.Plugins;

namespace MarkdigEngine.Extensions
{
    public static class MarkdigMarked
    {
        public static string Markup(MarkdownContext context, MarkdownServiceParameters parameters)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var pipeline = CreatePipeline(context, parameters);

            return Markup(context.Content, pipeline);
        }

        public static MarkdownPipeline CreatePipeline(MarkdownContext context, MarkdownServiceParameters parameters, string content = null)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .UseDfmExtensions(context, parameters);

            return pipeline.Build();
        }

        public static string Markup(string content, MarkdownPipeline pipeline)
        {
            return Markdown.ToHtml(content, pipeline);
        }
    }
}
