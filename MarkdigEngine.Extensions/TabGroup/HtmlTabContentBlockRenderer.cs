using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace MarkdigEngine.Extensions
{
    class HtmlTabContentBlockRenderer : HtmlObjectRenderer<TabContentBlock>
    {
        protected override void Write(HtmlRenderer renderer, TabContentBlock block)
        {
            foreach(var item in block)
            {
                if (!(item is ThematicBreakBlock))
                {
                    renderer.Render(item);
                }
            }
        }
    }
}
