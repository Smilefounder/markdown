using Markdig.Renderers.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Markdig.Renderers;

namespace MarkdigEngine
{
    public class HtmlXrefInlineRender : HtmlObjectRenderer<XrefInline>
    {
        protected override void Write(HtmlRenderer renderer, XrefInline obj)
        {
            if (renderer.EnableHtmlForInline)
            {
                renderer.Write("<xref href=\"").Write(obj.Href).Write("\"").WriteAttributes(obj).Write("></xref>");
            }
        }
    }
}
