using Markdig.Renderers.Html;
using Markdig.Renderers;

namespace MarkdigEngine.Extensions
{
    public class HtmlTabTitleBlockRenderer : HtmlObjectRenderer<TabTitleBlock>
    {
        protected override void Write(HtmlRenderer renderer, TabTitleBlock block)
        {
            foreach(var inline in block.Content)
            {
                renderer.Render(inline);
            }
        }
    }
}
