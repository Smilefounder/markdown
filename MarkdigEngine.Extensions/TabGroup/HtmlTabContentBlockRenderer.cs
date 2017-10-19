using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace MarkdigEngine.Extensions
{
    class HtmlTabContentBlockRenderer : HtmlObjectRenderer<TabContentBlock>
    {
        protected override void Write(HtmlRenderer renderer, TabContentBlock block)
        {
            foreach(var item in block)
            {
                renderer.Render(item);
            }
        }
    }
}
