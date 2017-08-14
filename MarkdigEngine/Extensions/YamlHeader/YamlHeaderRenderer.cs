﻿using Markdig.Extensions.Yaml;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using System;
using System.Collections.Generic;
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
            renderer.Write("<yamlheader").Write($" start=\"{obj.Line + 1}\" end=\"{obj.Line + obj.Lines.Count + 2}\"");
            renderer.WriteAttributes(obj).Write(">");
            renderer.Write(WebUtility.HtmlEncode(obj.Lines.ToString()));
            renderer.Write("</yamlheader>");
        }
    }
}
