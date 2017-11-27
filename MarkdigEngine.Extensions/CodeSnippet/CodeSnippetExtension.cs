using Markdig;
using Markdig.Renderers;

namespace MarkdigEngine.Extensions
{
    public class CodeSnippetExtension : IMarkdownExtension
    {
        private IMarkdigCompositor _compositor;
        private MarkdownContext _context;

        public CodeSnippetExtension(IMarkdigCompositor compositor, MarkdownContext context)
        {
            _compositor = compositor;
            _context = context;
        }

        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            pipeline.BlockParsers.AddIfNotAlready<CodeSnippetParser>();
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            var htmlRenderer = renderer as HtmlRenderer;
            if (htmlRenderer != null && !htmlRenderer.ObjectRenderers.Contains<HtmlCodeSnippetRenderer>())
            {
                // Must be inserted before CodeBlockRenderer
                htmlRenderer.ObjectRenderers.Insert(0, new HtmlCodeSnippetRenderer(_compositor, _context));
            }
        }
    }
}
