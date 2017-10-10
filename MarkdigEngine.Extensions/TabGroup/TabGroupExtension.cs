using Markdig;
using Markdig.Renderers;

namespace MarkdigEngine.Extensions
{
    public class TabGroupExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipelineBuilder pipeline)
        {
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            if (renderer is HtmlRenderer htmlRenderer)
            {
                if (!htmlRenderer.ObjectRenderers.Contains<HtmlTabGroupBlockRenderer>())
                {
                    htmlRenderer.ObjectRenderers.Add(new HtmlTabGroupBlockRenderer());
                }
            }
        }
    }
}
