using Markdig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Markdig.Renderers;
using Markdig.Parsers.Inlines;

namespace MarkdigEngine
{
    public class CodeSnippetExtension : IMarkdownExtension
    {
        protected string m_Path;
        protected string m_BasePath;

        public CodeSnippetExtension(string basePath, string path)
        {
            this.m_BasePath = basePath;
            this.m_Path = path;
        }

        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            pipeline.InlineParsers.InsertBefore<LinkInlineParser>(new CodeSnippetParser());
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            var htmlRenderer = renderer as HtmlRenderer;
            if (htmlRenderer != null && !htmlRenderer.ObjectRenderers.Contains<HtmlCodeSnippetRenderer>())
            {
                // Must be inserted before CodeBlockRenderer
                htmlRenderer.ObjectRenderers.Insert(0, new HtmlCodeSnippetRenderer(this.m_BasePath, this.m_Path));
            }
        }
    }
}
