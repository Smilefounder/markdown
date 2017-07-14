using Markdig.Renderers;
using Markdig;
using Markdig.Parsers.Inlines;
using Microsoft.DocAsCode.Plugins;

namespace MarkdigEngine
{
    /// <summary>
    /// Extension to enable extension IncludeFile.
    /// </summary>
    public class InclusionExtension : IMarkdownExtension
    {
        public MarkdownContext Context { get; set; }

        private MarkdownServiceParameters _parameters;

        public InclusionExtension(MarkdownContext context, MarkdownServiceParameters parameters)
        {
            Context = context;
            _parameters = parameters;
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
                    htmlRenderer.ObjectRenderers.Insert(0, new HtmlInclusionInlineRenderer(pipeline, Context, _parameters));
                }

                if (!htmlRenderer.ObjectRenderers.Contains<HtmlInclusionBlockRenderer>())
                {
                    htmlRenderer.ObjectRenderers.Insert(0, new HtmlInclusionBlockRenderer(Context, _parameters));
                }
            }
        }
    }
}
