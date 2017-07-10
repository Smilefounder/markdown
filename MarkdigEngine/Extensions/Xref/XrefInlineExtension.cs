using Markdig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Markdig.Renderers;
using Markdig.Parsers.Inlines;

namespace MarkdigEngine
{
    public class XrefInlineExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            pipeline.InlineParsers.InsertBefore<AutolineInlineParser>(new XrefInlineParser());
            pipeline.InlineParsers.AddIfNotAlready(new XrefInlineShortParser());
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            var htmlRenderer = renderer as HtmlRenderer;
            if (htmlRenderer != null && !htmlRenderer.ObjectRenderers.Contains<HtmlXrefInlineRender>())
            {
                // Must be inserted before CodeBlockRenderer
                htmlRenderer.ObjectRenderers.Insert(0, new HtmlXrefInlineRender());
            }
        }
    }
}
