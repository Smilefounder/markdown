using Markdig;
using Markdig.Parsers;
using Microsoft.DocAsCode.Common;
using Microsoft.DocAsCode.Plugins;

using Markdig.Extensions.AutoIdentifiers;

namespace MarkdigEngine
{
    public static class MarkdownExtensions
    {
        public static MarkdownPipelineBuilder UseDfmExtensions(this MarkdownPipelineBuilder pipeline, MarkdownContext context, MarkdownServiceParameters parameters)
        {
            return pipeline
                .UseDFMHeadingId()
                .UseIncludeFile(context, parameters)
                .UseCodeSnippet(context)
                .UseYamlHeader()
                .UseDFMCodeInfoPrefix()
                .UseQuoteSectionNote(parameters)
                .UseXref()
                .UseEmojiAndSmiley();
        }

        public static MarkdownPipelineBuilder UseValidators(this MarkdownPipelineBuilder pipeline, MarkdownServiceParameters parameters)
        {
            var builder = MarkdownValidatorBuilder.Create(null, parameters.BasePath, parameters.TemplateDir);
            pipeline.DocumentProcessed += builder.CreateValidation();

            return pipeline;
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

        /// <summary>
        /// This extension removes AutoIdentifierExtension. Please use this extension after UseAdvancedExtensions().
        /// </summary>
        /// <param name="pipeline"></param>
        /// <returns></returns>
        public static MarkdownPipelineBuilder UseDFMHeadingId(this MarkdownPipelineBuilder pipeline)
        {
            if (pipeline.Extensions.Contains<AutoIdentifierExtension>())
            {
                pipeline.Extensions.Replace<AutoIdentifierExtension>(new DFMHeadingIdExtension());
            }
            else
            {
                pipeline.Extensions.Add(new DFMHeadingIdExtension());
            }
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
            if (context.InclusionSet != null && !context.InclusionSet.IsEmpty)
            {
                pipeline.DocumentProcessed += InclusionExtension.GetProcessDocumentDelegate(context);
            }
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
