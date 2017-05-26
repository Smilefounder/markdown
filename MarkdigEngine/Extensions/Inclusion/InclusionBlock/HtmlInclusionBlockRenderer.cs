using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

using Markdig;
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

            var currentFilePath = (RelativePath)_context.FilePath;
            var refFilePath = inclusion.Context.RefFilePath;
            var includeFilePath = ((RelativePath)refFilePath).BasedOn(currentFilePath).RemoveWorkingFolder();

            if (!File.Exists(includeFilePath))
            {
                Console.WriteLine($"Can't find {includeFilePath}.");
                renderer.Write(inclusion.Context.Syntax);

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

            var result = MarkdigMarked.Markup(content, context);

            renderer.Write(result);
        }
    }
}
