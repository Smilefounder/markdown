using System.IO;

using Markdig.Renderers;
using Markdig.Renderers.Html;
using Microsoft.DocAsCode.Common;
using Microsoft.DocAsCode.Plugins;

namespace MarkdigEngine.Extensions
{
    public class HtmlInclusionInlineRenderer : HtmlObjectRenderer<InclusionInline>
    {
        private MarkdownContext _context;
        private MarkdownServiceParameters _parameters;

        public HtmlInclusionInlineRenderer(MarkdownContext context, MarkdownServiceParameters parameters)
        {
            _context = context;
            _parameters = parameters;
        }

        protected override void Write(HtmlRenderer renderer, InclusionInline inclusion)
        {
            if (string.IsNullOrEmpty(inclusion.Context.IncludedFilePath))
            {
                Logger.LogError("file path can't be empty or null in IncludeFile");
                renderer.Write(inclusion.Context.GetRaw());

                return;
            }

            if (!PathUtility.IsRelativePath(inclusion.Context.IncludedFilePath))
            {
                string tag = "ERROR INCLUDE";
                string message = $"Unable to resolve {inclusion.Context.GetRaw()}: Absolute path \"{inclusion.Context.IncludedFilePath}\" is not supported.";
                ExtensionsHelper.GenerateNodeWithCommentWrapper(renderer, tag, message, inclusion.Context.GetRaw(), inclusion.Line);

                return;
            }

            var currentFilePath = ((RelativePath)_context.FilePath).GetPathFromWorkingFolder();
            var includeFilePath = ((RelativePath)inclusion.Context.IncludedFilePath).BasedOn(currentFilePath);

            var filePath = Path.Combine(_context.BasePath, includeFilePath.RemoveWorkingFolder());
            if (!File.Exists(filePath))
            {
                Logger.LogWarning($"Can't find {includeFilePath}.");
                renderer.Write(inclusion.Context.GetRaw());

                return;
            }

            var parents = _context.InclusionSet;
            if (parents != null && parents.Contains(includeFilePath))
            {
                string tag = "ERROR INCLUDE";
                string message = $"Unable to resolve {inclusion.Context.GetRaw()}: Circular dependency found in \"{_context.FilePath}\"";
                ExtensionsHelper.GenerateNodeWithCommentWrapper(renderer, tag, message, inclusion.Context.GetRaw(), inclusion.Line);

                return;
            }

            _context.ReportDependency(includeFilePath);
            var content = File.ReadAllText(filePath);
            var context = new MarkdownContext(includeFilePath.RemoveWorkingFolder(), _context.BasePath, _context.Mvb, content, true, _context.InclusionSet, _context.Dependency);
            context = context.AddIncludeFile(currentFilePath);

            // Do not need to check if content is a single paragragh
            // context.IsInline = true will force it into a single paragragh and render with no <p></p>
            renderer.Write(MarkdigMarked.Markup(context, _parameters));
        }
    }
}
