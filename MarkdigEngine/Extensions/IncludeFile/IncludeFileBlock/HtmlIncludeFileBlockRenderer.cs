using System;
using System.IO;
using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace MarkdigEngine
{
    public class HtmlIncludeFileBlockRenderer : HtmlObjectRenderer<IncludeFileBlock>
    {
        private MarkdownPipeline _pipeline;
        private MarkdownContext _context;

        public HtmlIncludeFileBlockRenderer(MarkdownPipeline pipeline, MarkdownContext context)
        {
            _pipeline = pipeline;
            _context = context;
        }

        protected override void Write(HtmlRenderer renderer, IncludeFileBlock includeFile)
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
            }
            else
            {
                using (var sr = new StreamReader(includeFilePath))
                {
                    var content = sr.ReadToEnd();
                    var result = Markdown.ToHtml(content, _pipeline);
                    renderer.Write(result);
                }
            }
        }
    }
}
