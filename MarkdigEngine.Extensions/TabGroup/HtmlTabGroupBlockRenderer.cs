using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace MarkdigEngine.Extensions
{
    public class HtmlTabGroupBlockRenderer : HtmlObjectRenderer<TabGroupBlock>
    {
        protected override void Write(HtmlRenderer renderer, TabGroupBlock block)
        {
            // todo : Render TabGroupBlock
            var result = "<!-- todo: tab group -->\n";
            renderer.Write(result);
        }
    }
}
