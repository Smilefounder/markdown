using Markdig;

namespace MarkdigEngine
{
    public static class MarkdownExtensions
    {
        public static MarkdownPipelineBuilder UseDfmExtensions(this MarkdownPipelineBuilder pipeline, MarkdownContext context)
        {
            return pipeline
				.UseLineNumber(context)
				.UseIncludeFile(context)
                .UseCodeSnippet(context);
        }

		public static MarkdownPipelineBuilder UseLineNumber(this MarkdownPipelineBuilder pipeline, MarkdownContext context)
		{
			if (context.EnableSourceInfo == true)
			{
				pipeline.PreciseSourceLocation = true;
				pipeline.DocumentProcessed += LineNumberExtension.GetProcessDocumentDelegate(context);
			}
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
    }
}
