using Markdig;
using Markdig.Renderers;
using Markdig.Parsers.Inlines;

namespace MarkdigEngine
{
    public class CodeSnippetExtension : IMarkdownExtension
    {
        private MarkdownContext _context;

        public CodeSnippetExtension(MarkdownContext context)
        {
            _context = context;
        }

        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            pipeline.InlineParsers.InsertBefore<LinkInlineParser>(new CodeSnippetParser());
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            var htmlRenderer = renderer as HtmlRenderer;
            if (htmlRenderer != null && !htmlRenderer.ObjectRenderers.Contains<HtmlCodeSnippetRenderer>())
            {
                // Must be inserted before CodeBlockRenderer
                htmlRenderer.ObjectRenderers.Insert(0, new HtmlCodeSnippetRenderer(_context));
            }
        }
    }
}
