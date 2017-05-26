using Markdig.Renderers;
using Markdig;
using Markdig.Parsers.Inlines;

namespace MarkdigEngine
{
    /// <summary>
    /// Extension to enable extension IncludeFile.
    /// </summary>
    public class InclusionExtension : IMarkdownExtension
    {
        public MarkdownContext Context { get; set; }

        public InclusionExtension(MarkdownContext context)
        {
            Context = context;
        }

        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            pipeline.BlockParsers.AddIfNotAlready<InclusionBlockParser>();
            pipeline.InlineParsers.InsertBefore<LinkInlineParser>(new InclusionInlineParser());
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            if (renderer is HtmlRenderer htmlRenderer)
            {
                if (!htmlRenderer.ObjectRenderers.Contains<HtmlInclusionInlineRenderer>())
                {
                    htmlRenderer.ObjectRenderers.Insert(0, new HtmlInclusionInlineRenderer(pipeline, Context));
                }

                if (!htmlRenderer.ObjectRenderers.Contains<HtmlInclusionBlockRenderer>())
                {
                    htmlRenderer.ObjectRenderers.Insert(0, new HtmlInclusionBlockRenderer(Context));
                }
            }
        }
    }
}
