using Markdig;
using Markdig.Parsers;
using Microsoft.DocAsCode.Common;
using Microsoft.DocAsCode.Plugins;

namespace MarkdigEngine
{
    public static class MarkdownExtensions
    {
        public static MarkdownPipelineBuilder UseDfmExtensions(this MarkdownPipelineBuilder pipeline, MarkdownContext context, MarkdownServiceParameters parameters)
        {
            return pipeline
                .UseIncludeFile(context, parameters)
                .UseCodeSnippet(context)
                .UseDFMCodeInfoPrefix()
                .UseQuoteSectionNote(parameters)
                .UseXref();
        }

        public static MarkdownPipelineBuilder UseDFMCodeInfoPrefix(this MarkdownPipelineBuilder pipeline)
        {
            var DFMFencedCodeBlockParser = new FencedCodeBlockParser()
            {
                InfoPrefix = "lang-"
            };

            if (!pipeline.BlockParsers.Replace<FencedCodeBlockParser>(DFMFencedCodeBlockParser))
            {
                Logger.LogWarning($"Can't find FencedCodeBlockParser to replace, insert DFMFencedCodeBlockParser directly.");
                pipeline.BlockParsers.Insert(0, DFMFencedCodeBlockParser);
            }
            return pipeline;
        }

        public static MarkdownPipelineBuilder UseQuoteSectionNote(this MarkdownPipelineBuilder pipeline, MarkdownServiceParameters parameters)
        {
            pipeline.Extensions.Insert(0, new QuoteSectionNoteExtension(parameters));
            return pipeline;
        }

        public static MarkdownPipelineBuilder UseLineNumber(this MarkdownPipelineBuilder pipeline, LineNumberExtensionContext lineNumberContext)
        {
            pipeline.PreciseSourceLocation = true;
            pipeline.DocumentProcessed += LineNumberExtension.GetProcessDocumentDelegate(lineNumberContext);
            return pipeline;
        }

        public static MarkdownPipelineBuilder UseIncludeFile(this MarkdownPipelineBuilder pipeline, MarkdownContext context, MarkdownServiceParameters parameters)
        {
            pipeline.Extensions.Insert(0, new InclusionExtension(context, parameters));
            return pipeline;
        }

        public static MarkdownPipelineBuilder UseCodeSnippet(this MarkdownPipelineBuilder pipeline, MarkdownContext context)
        {
            pipeline.Extensions.Insert(0, new CodeSnippetExtension(context));
            return pipeline;
        }

        public static MarkdownPipelineBuilder UseXref(this MarkdownPipelineBuilder pipeline)
        {
            pipeline.Extensions.Insert(0, new XrefInlineExtension());
            return pipeline;
        }
    }
}
