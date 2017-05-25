using System;
using System.IO;
using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;

namespace MarkdigEngine
{
    public class HtmlIncludeFileInlineRenderer : HtmlObjectRenderer<IncludeFileInline>
    {
        private MarkdownPipeline _pipeline;
        private MarkdownContext _context;

        public HtmlIncludeFileInlineRenderer(MarkdownPipeline pipeline, MarkdownContext context)
        {
            _pipeline = pipeline;
            _context = context;
        }

        protected override void Write(HtmlRenderer renderer, IncludeFileInline includeFile)
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
                    var document = Markdown.Parse(content, _pipeline);
                    var result = Markdown.ToHtml(content, _pipeline);

                    if (document != null && document.Count == 1 && document.LastChild is ParagraphBlock)
                    {
                        if (result.StartsWith("<p>") && result.EndsWith("</p>\n"))
                        {
                            result = result.Remove(0, 3);
                            result = result.Remove(result.Length - 5, 5);
                        }
                        else
                        {
                            Console.WriteLine($"[Warning]: Raw content in Inline inclusion should start with <p> and end with</p>\n");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"[Warning]: Inline inclusion only support inline syntax.");
                    }

                    renderer.Write(result);
                }
            }
        }
    }
}
