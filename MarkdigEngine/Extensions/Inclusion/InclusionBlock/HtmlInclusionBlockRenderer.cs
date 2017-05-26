using System;
using System.IO;

using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html;

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

            var includeFilePath = ExtensionsHelper.GetAbsolutePathOfRefFile(_context.BasePath, _context.FilePath, includeFile.Context.RefFilePath);

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
