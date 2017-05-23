﻿using Markdig;
using MarkdigEngine.Extensions.IncludeFile;

namespace MarkdigEngine
{
    public static class MarkdownExtensions
    {
        public static MarkdownPipelineBuilder UseDfmExtensions(this MarkdownPipelineBuilder pipeline, MarkdownContext context)
        {
            return pipeline
                .UseIncludeFile(context);
        }

        public static MarkdownPipelineBuilder UseIncludeFile(this MarkdownPipelineBuilder pipeline, MarkdownContext context)
        {
            pipeline.Extensions.Insert(0, new IncludeFileExtension(context));
            return pipeline;
        }
    }
}
