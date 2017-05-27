using System;
using System.IO;

using Markdig.Renderers;
using Markdig.Renderers.Html;

using Microsoft.DocAsCode.Common;

namespace MarkdigEngine
{
    public class HtmlInclusionBlockRenderer : HtmlObjectRenderer<InclusionBlock>
    {
        private MarkdownContext _context;

        public HtmlInclusionBlockRenderer(MarkdownContext context)
        {
            _context = context;
        }

        protected override void Write(HtmlRenderer renderer, InclusionBlock inclusion)
        {
            if (string.IsNullOrEmpty(inclusion.Context.RefFilePath))
            {
                throw new Exception("file path can't be empty or null in IncludeFile");
            }

            var currentFilePath = ((RelativePath)_context.FilePath).GetPathFromWorkingFolder();
            var refFilePath = inclusion.Context.RefFilePath;
            var includeFilePath = ((RelativePath)refFilePath).BasedOn(currentFilePath);

            if (!File.Exists(includeFilePath.RemoveWorkingFolder()))
            {
                Console.WriteLine($"Can't find {includeFilePath}.");
                renderer.Write(inclusion.Context.Syntax);

                return;
            }

            var parents = _context.InclusionSet;
            if (parents != null && parents.Contains(currentFilePath))
            {
                throw new Exception($"Circular dependency found in {currentFilePath}.");
            }

            _context = _context.AddIncludeFile(currentFilePath);
            _context.ReportDependency(includeFilePath);
            var context = new MarkdownContext(includeFilePath.RemoveWorkingFolder(), _context.BasePath, _context.InclusionSet, _context.Dependency);

            string content;
            using (var sr = new StreamReader(includeFilePath.RemoveWorkingFolder()))
            {
                content = sr.ReadToEnd();
            }

            var result = MarkdigMarked.Markup(content, context);

            renderer.Write(result);
        }
    }
}
