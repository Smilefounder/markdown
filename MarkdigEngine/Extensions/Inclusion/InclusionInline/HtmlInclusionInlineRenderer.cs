using System.IO;

using Markdig;
using Markdig.Renderers;
using Markdig.Renderers.Html;
using Markdig.Syntax;

using Microsoft.DocAsCode.Common;
using Microsoft.DocAsCode.Plugins;
using System;

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

            if (!PathUtility.IsRelativePath(inclusion.Context.RefFilePath))
            {
                string tag = "ERROR INCLUDE";
                string message = $"Unable to resolve {inclusion.Context.GetRaw()}: Absolute path \"{inclusion.Context.RefFilePath}\" is not supported.";
                ExtensionsHelper.GenerateNodeWithCommentWrapper(renderer, tag, message, inclusion.Context.GetRaw(), inclusion.Line);

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
            if (parents != null && parents.Contains(includeFilePath))
            {
                string tag = "ERROR INCLUDE";
                string message = $"Unable to resolve {inclusion.Context.GetRaw()}: Circular dependency found in \"{_context.FilePath}\"";
                ExtensionsHelper.GenerateNodeWithCommentWrapper(renderer, tag, message, inclusion.Context.GetRaw(), inclusion.Line);

                return;
            }

            _context.ReportDependency(includeFilePath);

            var context = new MarkdownContext(includeFilePath.RemoveWorkingFolder(), _context.BasePath, true, _context.InclusionSet, _context.Dependency);
            context = context.AddIncludeFile(currentFilePath);

            string content;
            using (var sr = new StreamReader(refPath))
            {
                content = sr.ReadToEnd();
            }

            // Do not need to check if content is a single paragragh
            // context.IsInline = true will force it into a single paragragh and render with no <p></p>
            renderer.Write(MarkdigMarked.Markup(content, context, _parameters));
        }
    }
}
