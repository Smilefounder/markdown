using Markdig;
using Markdig.Extensions;

namespace MarkdigEngine
{
    public static class MarkdownExtensions
    {
        public static MarkdownPipelineBuilder UseDfmExtensions(this MarkdownPipelineBuilder pipeline, MarkdownContext context)
        {
            return pipeline
                .UseIncludeFile(context)
                .UseCodeSnippet(context)
                .UseXref()
                .UseYamlHeader();
        }

        public static MarkdownPipelineBuilder UseLineNumber(this MarkdownPipelineBuilder pipeline, LineNumberExtensionContext lineNumberContext)
        {
            pipeline.PreciseSourceLocation = true;
            pipeline.DocumentProcessed += LineNumberExtension.GetProcessDocumentDelegate(lineNumberContext);
            return pipeline;
        }

        public static MarkdownPipelineBuilder UseIncludeFile(this MarkdownPipelineBuilder pipeline, MarkdownContext context)
        {
            pipeline.Extensions.Insert(0, new InclusionExtension(context));
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
