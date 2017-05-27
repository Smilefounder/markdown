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

        protected override void Write(HtmlRenderer renderer, InclusionInline inclusion)
        {
            if (string.IsNullOrEmpty(inclusion.Context.RefFilePath))
            {
                Logger.LogError("file path can't be empty or null in IncludeFile");
                renderer.Write(inclusion.Context.GetRaw());

                return;
            }

            var currentFilePath = ((RelativePath)_context.FilePath).GetPathFromWorkingFolder();
            var refFilePath = inclusion.Context.RefFilePath;
            var includeFilePath = ((RelativePath)refFilePath).BasedOn(currentFilePath);

            if (!File.Exists(includeFilePath.RemoveWorkingFolder()))
            {
                Logger.LogWarning($"Can't find {includeFilePath}.");
                renderer.Write(inclusion.Context.GetRaw());

                return;
            }

            var parents = _context.InclusionSet;
            if (parents != null && parents.Contains(currentFilePath))
            {
                Logger.LogError($"Circular dependency found in {currentFilePath}.");
                renderer.Write(inclusion.Context.GetRaw());

                return;
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
                Logger.LogWarning($"Inline syntax for Inclusion only support inline syntax in {inclusion}.");
                var result = Markdown.ToHtml(content, _pipeline);

                renderer.Write(result);
            }
        }
    }
}
