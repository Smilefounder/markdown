using System;
using System.IO;

using Markdig;
using Markdig.Renderers;
using Markdig.Syntax.Inlines;
using Microsoft.DocAsCode.Plugins;

namespace MarkdigEngine
{
    public static class MarkdigMarked
    {
        public static string Markup(string content, MarkdownContext context, MarkdownServiceParameters parameters)
        {
            if (content == null)
            {
                throw new ArgumentNullException(nameof(content));
            }

            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            var pipeline = CreatePipeline(context, parameters, content);

            return Markdown.ToHtml(content, pipeline);
        }

        public static string Markup(ContainerInline inline, MarkdownPipeline pipeline)
        {
            var writer = new StringWriter();
            var renderer = new HtmlRenderer(writer);
            pipeline.Setup(renderer);
            renderer.Render(inline);
            writer.Flush();
            return writer.ToString();
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

            object enableSourceInfo = null;
            parameters?.Extensions?.TryGetValue(LineNumberExtension.EnableSourceInfo, out enableSourceInfo);

            if (enableSourceInfo != null && (bool)enableSourceInfo == true)
            {
                var absoluteFilePath = Path.Combine(context.BasePath, context.FilePath);
                var lineNumberContext = LineNumberExtensionContext.Create(content, absoluteFilePath, context.FilePath);
                pipeline.UseLineNumber(lineNumberContext);
            }

            return pipeline.Build();
        }
    }
}
