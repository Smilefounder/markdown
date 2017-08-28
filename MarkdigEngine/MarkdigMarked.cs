using System;
using System.IO;

using Markdig;
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

            if (context.IsInline)
            {
                pipeline.UseInineParserOnly();
            }

            return pipeline.Build();
        }
    }
}
