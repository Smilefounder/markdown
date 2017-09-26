using Markdig.Extensions.Yaml;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Microsoft.DocAsCode.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MarkdigEngine.Extensions
{
    public class YamlHeaderRenderer : HtmlObjectRenderer<YamlFrontMatterBlock>
    {
        protected override void Write(HtmlRenderer renderer, YamlFrontMatterBlock obj)
        {            
            var content = obj.Lines.ToString();
            try
            {
                using (StringReader reader = new StringReader(content))
                {
                    var result = YamlUtility.Deserialize<Dictionary<string, object>>(reader);
                    if (result != null)
                    {
                        renderer.Write("<yamlheader").Write($" start=\"{obj.Line + 1}\" end=\"{obj.Line + obj.Lines.Count + 2}\"");
                        renderer.WriteAttributes(obj).Write(">");
                        renderer.Write(WebUtility.HtmlEncode(obj.Lines.ToString()));
                        renderer.Write("</yamlheader>");
                    }
                }
            }
            catch (Exception)
            {
                // not a valid ymlheader, do nothing
            }
        }
    }
}
