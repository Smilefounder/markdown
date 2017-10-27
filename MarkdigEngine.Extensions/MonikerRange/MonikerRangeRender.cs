using System;

using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace MarkdigEngine.Extensions
{
    public class MonikerRangeRender : HtmlObjectRenderer<MonikerRangeBlock>
    {
        protected override void Write(HtmlRenderer renderer, MonikerRangeBlock obj)
        {
            renderer.Write("<div").WriteAttributes(obj).Write($" range=\"{obj.MonikerRange}\"").WriteLine(">");
            renderer.WriteChildren(obj);
            renderer.WriteLine("</div>");
        }
    }
}
