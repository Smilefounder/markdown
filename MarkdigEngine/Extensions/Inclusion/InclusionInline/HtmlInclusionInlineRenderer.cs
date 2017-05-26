using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

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

            var currentFilePath = _context.FilePath;
            var refFilePath = includeFile.Context.RefFilePath;
            var includeFilePath = ((RelativePath)refFilePath).BasedOn((RelativePath)currentFilePath).RemoveWorkingFolder();

            if (!File.Exists(includeFilePath))
            {
                Console.WriteLine($"Can't find {includeFilePath}.");
                renderer.Write(includeFile.Context.Syntax);

                return;
            }

            var parents = _context.GetFilePathStack() ?? ImmutableHashSet<string>.Empty;

            if (parents.Contains(currentFilePath))
            {
                throw new Exception($"Circular dependency found in {currentFilePath}.");
            }

            parents = parents.Add(currentFilePath);
            var context = _context.SetFilePathStack(parents);
            context = new MarkdownContext(includeFilePath, context.BasePath, context.Variables);

            string content;
            using (var sr = new StreamReader(includeFilePath))
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
