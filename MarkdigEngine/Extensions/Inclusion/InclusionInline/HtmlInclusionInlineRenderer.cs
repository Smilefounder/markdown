using System;
using System.IO;

using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;

using Microsoft.DocAsCode.Common;

namespace MarkdigEngine
{
    public class HtmlInclusionInlineRenderer : HtmlObjectRenderer<InclusionInline>
    {
        private MarkdownPipeline _pipeline;
        private MarkdownContext _context;

        public HtmlInclusionInlineRenderer(MarkdownPipeline pipeline, MarkdownContext context)
        {
            _pipeline = pipeline;
            _context = context;
        }

        protected override void Write(HtmlRenderer renderer, InclusionInline includeFile)
        {
            if (string.IsNullOrEmpty(includeFile.Context.RefFilePath))
            {
                throw new Exception("file path can't be empty or null in IncludeFile");
            }

            var currentFilePath = ((RelativePath)_context.FilePath).GetPathFromWorkingFolder();
            var refFilePath = includeFile.Context.RefFilePath;
            var includeFilePath = ((RelativePath)refFilePath).BasedOn(currentFilePath);

            if (!File.Exists(includeFilePath.RemoveWorkingFolder()))
            {
                Console.WriteLine($"Can't find {includeFilePath}.");
                renderer.Write(includeFile.Context.Syntax);

                return;
            }

            var parents = _context.InclusionSet;
            if (parents != null && parents.Contains(currentFilePath))
            {
                throw new Exception($"Circular dependency found in {currentFilePath}.");
            }

            _context = _context.AddIncludeFile(currentFilePath);
            _context.ReportDependency(includeFilePath);

            var context = new MarkdownContext(includeFilePath, _context.BasePath, _context.InclusionSet, _context.Dependency);

            string content;
            using (var sr = new StreamReader(includeFilePath.RemoveWorkingFolder()))
            {
                content = sr.ReadToEnd();
            }

            var document = Markdown.Parse(content, _pipeline);
            if (document != null && document.Count == 1 && document.LastChild is ParagraphBlock)
            {
                var block = (ParagraphBlock)document.LastChild;
                var inlines = block.Inline;
                var result = MarkdigMarked.Markup(inlines, context);

                renderer.Write(result);
            }
            else
            {
                Console.WriteLine($"[Warning]: Inline inclusion only support inline syntax.");
                var result = Markdown.ToHtml(content, _pipeline);

                renderer.Write(result);
            }
        }
    }
}
