using System;
using System.IO;

using Markdig;
using Markdig.Renderers;
using Markdig.Syntax.Inlines;
using Markdig.Syntax;
using Markdig.Renderers.Html;

namespace MarkdigEngine
{
    public static class MarkdigMarked
    {
        public static string Markup(string content, MarkdownContext context)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

			if (context.EnableSourceInfo) context.ResetlineEnds(content);
            var pipeline = CreatePipeline(context);

			return Markdown.ToHtml(content, pipeline);
		}

        public static string Markup(ContainerInline inline, MarkdownContext context)
        {
            var writer = new StringWriter();
            var renderer = new HtmlRenderer(writer);
            var pipeline = CreatePipeline(context);

            pipeline.Setup(renderer);
            renderer.Render(inline);
            writer.Flush();

            return writer.ToString();
        }

        public static MarkdownPipeline CreatePipeline(MarkdownContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .UseDfmExtensions(context)
				.Build();
        }
	}
}
