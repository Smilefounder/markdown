using Markdig.Renderers;
using Markdig;

namespace MarkdigEngine.Extensions.IncludeFile
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
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            var htmlRenderer = renderer as HtmlRenderer;
            if (htmlRenderer != null && !htmlRenderer.ObjectRenderers.Contains<HtmlIncludeFileRenderer>())
            {
                htmlRenderer.ObjectRenderers.Insert(0, new HtmlIncludeFileRenderer(pipeline, Context));
            }
        }
    }
}
