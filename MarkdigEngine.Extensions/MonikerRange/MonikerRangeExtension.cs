using Markdig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Markdig.Renderers;
using Markdig.Extensions.CustomContainers;

namespace MarkdigEngine.Extensions
{
    public class MonikerRangeExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            pipeline.BlockParsers.InsertBefore<CustomContainerParser>(new MonikerRangeParser());
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            var htmlRenderer = renderer as HtmlRenderer;
            if (htmlRenderer != null && !htmlRenderer.ObjectRenderers.Contains<MonikerRangeRender>())
            {
                htmlRenderer.ObjectRenderers.Insert(0, new MonikerRangeRender());
            }
        }
    }
}
