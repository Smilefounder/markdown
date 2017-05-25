using Markdig.Renderers;
using Markdig;
using Markdig.Parsers.Inlines;

namespace MarkdigEngine
{
    /// <summary>
    /// Extension to enable extension IncludeFile.
    /// </summary>
    public class IncludeFileExtension : IMarkdownExtension
    {
        public MarkdownContext Context { get; set; }

        public IncludeFileExtension(MarkdownContext context)
        {
            Context = context;
        }

        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            pipeline.BlockParsers.AddIfNotAlready<IncludeFileBlockParser>();
            pipeline.InlineParsers.InsertBefore<LinkInlineParser>(new IncludeFileInlineParser());
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            if (renderer is HtmlRenderer htmlRenderer)
            {
                if (!htmlRenderer.ObjectRenderers.Contains<HtmlIncludeFileInlineRenderer>())
                {
                    htmlRenderer.ObjectRenderers.Insert(0, new HtmlIncludeFileInlineRenderer(pipeline, Context));
                }

                if (!htmlRenderer.ObjectRenderers.Contains<HtmlIncludeFileBlockRenderer>())
                {
                    htmlRenderer.ObjectRenderers.Insert(0, new HtmlIncludeFileBlockRenderer(pipeline, Context));
                }
            }
        }
    }
}
