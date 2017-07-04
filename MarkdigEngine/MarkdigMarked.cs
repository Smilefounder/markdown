﻿using System;
using System.IO;

using Markdig;
using Markdig.Renderers;
using Markdig.Syntax.Inlines;

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

            var pipeline = CreatePipeline(context, content);

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

        public static MarkdownPipeline CreatePipeline(MarkdownContext context, string content = null)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .UseDfmExtensions(context);

            if (context.EnableSourceInfo)
            {
                var absoluteFilePath = Path.Combine(context.BasePath, context.FilePath);
                var lineNumberContext = LineNumberExtensionContext.Create(content, absoluteFilePath, context.FilePath);
                pipeline.UseLineNumber(lineNumberContext);
            }

            return pipeline.Build();
        }
    }
}
