using Markdig;
using MarkdigEngine.Extensions.IncludeFile;

namespace MarkdigEngine
{
    public static class MarkdownExtensions
    {
        public static MarkdownPipelineBuilder UseDfmExtensions(this MarkdownPipelineBuilder pipeline)
        {
            return pipeline
                .UseIncludeFile();
        }

        public static MarkdownPipelineBuilder UseIncludeFile(this MarkdownPipelineBuilder pipeline)
        {
            pipeline.Extensions.Insert(0, new IncludeFileExtension());
            return pipeline;
        }
    }
}
