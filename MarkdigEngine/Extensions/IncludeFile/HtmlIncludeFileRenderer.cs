using System;
using System.IO;
using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html;

namespace MarkdigEngine.Extensions.IncludeFile
{
    public class HtmlIncludeFileRenderer : HtmlObjectRenderer<IncludeFile>
    {
        private MarkdownPipeline _pipeline;
        private MarkdownContext _context;

        public HtmlIncludeFileRenderer(MarkdownPipeline pipeline, MarkdownContext context)
        {
            _pipeline = pipeline;
            _context = context;
        }

        protected override void Write(HtmlRenderer renderer, IncludeFile obj)
        {
            if (string.IsNullOrEmpty(obj.RefFilePath))
            {
                throw new Exception("file path can't be empty or null in IncludeFile");
            }

            var includeFilePath = ExtensionsHelper.GetAbsolutePathOfRefFile(_context.BasePath, _context.FilePath, obj.RefFilePath);

            if (!File.Exists(includeFilePath))
            {
                Console.WriteLine($"Can't find {includeFilePath}.");
                renderer.Write(obj.Syntax);
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
