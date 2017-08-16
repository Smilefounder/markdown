﻿using Markdig.Extensions.Yaml;
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

namespace MarkdigEngine
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
                        renderer.Write("<yamlheader").WriteAttributes(obj).Write(">").Write(WebUtility.HtmlEncode(content)).Write("</yamlheader>");
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
