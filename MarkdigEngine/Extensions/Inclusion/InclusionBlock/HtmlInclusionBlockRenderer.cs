using System;
using System.IO;

using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html;

using Microsoft.DocAsCode.Common;

namespace MarkdigEngine
{
    public class HtmlInclusionBlockRenderer : HtmlObjectRenderer<InclusionBlock>
    {
        private MarkdownPipeline _pipeline;
        private MarkdownContext _context;

        public HtmlInclusionBlockRenderer(MarkdownPipeline pipeline, MarkdownContext context)
        {
            _pipeline = pipeline;
            _context = context;
        }

        protected override void Write(HtmlRenderer renderer, InclusionBlock includeFile)
        {
            if (string.IsNullOrEmpty(includeFile.Context.RefFilePath))
            {
                throw new Exception("file path can't be empty or null in IncludeFile");
            }

            var currentFilePath = _context.FilePath;
            var refFilePath = includeFile.Context.RefFilePath;
            var includeFilePath = ((RelativePath)refFilePath).BasedOn((RelativePath)currentFilePath).RemoveWorkingFolder();

            if (!File.Exists(includeFilePath))
            {
                Console.WriteLine($"Can't find {includeFilePath}.");
                renderer.Write(includeFile.Context.Syntax);

                return;
            }

            string content;
            using (var sr = new StreamReader(includeFilePath))
            {
                content = sr.ReadToEnd();
            }

            var result = Markdown.ToHtml(content, _pipeline);
            renderer.Write(result);
        }
    }
}
