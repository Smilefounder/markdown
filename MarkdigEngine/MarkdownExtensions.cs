using Markdig;
using Markdig.Parsers;
using Markdig.Parsers.Inlines;
using Microsoft.DocAsCode.Common;
using Microsoft.DocAsCode.Plugins;

using Markdig.Extensions;

namespace MarkdigEngine
{
    public static class MarkdownExtensions
    {
        public static MarkdownPipelineBuilder UseDfmExtensions(this MarkdownPipelineBuilder pipeline, MarkdownContext context, MarkdownServiceParameters parameters)
        {
            return pipeline
                .UseIncludeFile(context, parameters)
                .UseCodeSnippet(context)
                .UseYamlHeader()
                .UseDFMCodeInfoPrefix()
                .UseQuoteSectionNote(parameters)
                .UseXref()
                .UseEmojiAndSmiley();
        }

        /// <summary>
        /// This extension removes all the block parser except paragragh. Please use this extension in the last.
        /// </summary>
        /// <param name="pipeline"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static MarkdownPipelineBuilder UseInineParserOnly(this MarkdownPipelineBuilder pipeline)
        {
            pipeline.Extensions.Add(new InlineOnlyExtentsion());
            return pipeline;
        }

        public static MarkdownPipelineBuilder UseDFMCodeInfoPrefix(this MarkdownPipelineBuilder pipeline)
        {
            var fencedCodeBlockParser = pipeline.BlockParsers.FindExact<FencedCodeBlockParser>();
            if (fencedCodeBlockParser != null)
            {
                fencedCodeBlockParser.InfoPrefix = "lang-";
            }
            else
            {
                Logger.LogWarning($"Can't find FencedCodeBlockParser to set InfoPrefix, insert DFMFencedCodeBlockParser directly.");
                pipeline.BlockParsers.Insert(0, new FencedCodeBlockParser() { InfoPrefix = "lang-" });
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
            pipeline.DocumentProcessed += InclusionExtension.GetProcessDocumentDelegate(context);
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

        public static MarkdownPipelineBuilder UseYamlHeader(this MarkdownPipelineBuilder pipeline)
        {
            pipeline.Extensions.Insert(0, new YamlHeaderExtension());
            return pipeline;
        }
    }
}
