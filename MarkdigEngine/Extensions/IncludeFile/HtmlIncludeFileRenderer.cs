using System;
using System.IO;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig;

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
            var filePath = obj.FilePath;
            if (string.IsNullOrEmpty(obj.FilePath))
            {
                throw new Exception("file path can't be empty or null in IncludeFile");
            }

            var currentDir = Path.GetDirectoryName(_context.FilePath);
            filePath = Path.Combine(currentDir, obj.FilePath);
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Can't find {filePath}.");
                renderer.Write(obj.Command);
            }
            else
            {
                using (var sr = new StreamReader(filePath))
                {
                    var content = sr.ReadToEnd();
                    var result = Markdown.ToHtml(content, _pipeline);
                    renderer.Write(result);
                }
            }
        }
    }
}
