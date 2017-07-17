using System.IO;

using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;

using Microsoft.DocAsCode.Common;
using Microsoft.DocAsCode.Plugins;

namespace MarkdigEngine
{
    public class HtmlInclusionInlineRenderer : HtmlObjectRenderer<InclusionInline>
    {
        private MarkdownPipeline _pipeline;
        private MarkdownContext _context;
        private MarkdownServiceParameters _parameters;

        public HtmlInclusionInlineRenderer(MarkdownPipeline pipeline, MarkdownContext context, MarkdownServiceParameters parameters)
        {
            _pipeline = pipeline;
            _context = context;
            _parameters = parameters;
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

            var refPath = Path.Combine(_context.BasePath, includeFilePath.RemoveWorkingFolder());
            if (!File.Exists(refPath))
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

            var context = new MarkdownContext(includeFilePath.RemoveWorkingFolder(), _context.BasePath, _context.InclusionSet, _context.Dependency);

            string content;
            using (var sr = new StreamReader(refPath))
            {
                content = sr.ReadToEnd();
            }

            var pipeline = MarkdigMarked.CreatePipeline(context, _parameters, content);

            var document = Markdown.Parse(content, pipeline);
            if (document != null && document.Count == 1 && document.LastChild is ParagraphBlock)
            {
                var block = (ParagraphBlock)document.LastChild;
                var inlines = block.Inline;
                var result = MarkdigMarked.Markup(inlines, pipeline);

                renderer.Write(result);
            }
            else
            {
                Logger.LogWarning($"Inline syntax for Inclusion only support inline syntax in {inclusion}.");
                var result = Markdown.ToHtml(content, pipeline);

                renderer.Write(result);
            }
        }
    }
}
