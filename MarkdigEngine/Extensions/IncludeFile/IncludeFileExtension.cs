using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Markdig.Renderers;
using Markdig.Parsers.Inlines;
using Markdig;

namespace MarkdigEngine.Extensions.IncludeFile
{
    /// <summary>
    /// Extension to enable include file.
    /// </summary>
    public class IncludeFileExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            pipeline.BlockParsers.AddIfNotAlready<IncludeFileBlockParser>();
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            var htmlRenderer = renderer as HtmlRenderer;
            if (htmlRenderer != null && !htmlRenderer.ObjectRenderers.Contains<HtmlIncludeFileRenderer>())
            {
                htmlRenderer.ObjectRenderers.Insert(0, new HtmlIncludeFileRenderer(pipeline));
            }
        }
    }
}
