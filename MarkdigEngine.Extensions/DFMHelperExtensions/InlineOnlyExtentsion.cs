using Markdig;
using Markdig.Helpers;
using Markdig.Parsers;
using Markdig.Renderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkdigEngine.Extensions
{
    public class InlineOnlyExtentsion : IMarkdownExtension
    {
        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            var paragraphBlockParser = pipeline.BlockParsers.FindExact<ParagraphBlockParser>() ?? new ParagraphBlockParser();
            pipeline.BlockParsers.Clear();
            pipeline.BlockParsers.Add(paragraphBlockParser);
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            if (renderer is HtmlRenderer htmlRenderer)
            {
                htmlRenderer.ImplicitParagraph = true;
            }
        }
    }
}