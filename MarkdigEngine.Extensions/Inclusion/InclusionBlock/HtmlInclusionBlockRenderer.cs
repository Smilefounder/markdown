using System.IO;
using System.Text.RegularExpressions;

using Markdig.Renderers;
using Markdig.Renderers.Html;
using Microsoft.DocAsCode.Common;
using Microsoft.DocAsCode.Plugins;

namespace MarkdigEngine.Extensions
{
    public class HtmlInclusionBlockRenderer : HtmlObjectRenderer<InclusionBlock>
    {
        private IMarkdigCompositor _compositor;
        private MarkdownContext _context;
        private MarkdownServiceParameters _parameters;
        private Regex YamlHeaderRegex = new Regex(@"^<yamlheader[^>]*?>[\s\S]*?<\/yamlheader>", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public HtmlInclusionBlockRenderer(IMarkdigCompositor compositor, MarkdownContext context, MarkdownServiceParameters parameters)
        {
            _compositor = compositor;
            _context = context;
            _parameters = parameters;
        }

        protected override void Write(HtmlRenderer renderer, InclusionBlock inclusion)
        {
            if (string.IsNullOrEmpty(inclusion.Context.IncludedFilePath))
            {
                Logger.LogError("file path can't be empty or null in IncludeFile");
                renderer.Write(inclusion.Context.GetRaw());

                return;
            }

            if (!PathUtility.IsRelativePath(inclusion.Context.IncludedFilePath))
            {
                var tag = "ERROR INCLUDE";
                var message = $"Unable to resolve {inclusion.Context.GetRaw()}: Absolute path \"{inclusion.Context.IncludedFilePath}\" is not supported.";
                ExtensionsHelper.GenerateNodeWithCommentWrapper(renderer, tag, message, inclusion.Context.GetRaw(), inclusion.Line);

                return;
            }

            var currentFilePath = ((RelativePath)_context.FilePath).GetPathFromWorkingFolder();
            var includedFilePath = ((RelativePath)inclusion.Context.IncludedFilePath).BasedOn(currentFilePath);

            var filePath = Path.Combine(_context.BasePath, includedFilePath.RemoveWorkingFolder());
            if (!File.Exists(filePath))
            {
                Logger.LogWarning($"Can't find {includedFilePath}.");
                renderer.Write(inclusion.Context.GetRaw());

                return;
            }

            var parents = _context.InclusionSet;
            if (parents != null && parents.Contains(includedFilePath))
            {
                string tag = "ERROR INCLUDE";
                string message = $"Unable to resolve {inclusion.Context.GetRaw()}: Circular dependency found in \"{_context.FilePath}\"";
                ExtensionsHelper.GenerateNodeWithCommentWrapper(renderer, tag, message, inclusion.Context.GetRaw(), inclusion.Line);

                return;
            }

            _context.ReportDependency(includedFilePath);
            var content = File.ReadAllText(filePath);
            var context = new MarkdownContext(
                includedFilePath.RemoveWorkingFolder(), 
                _context.BasePath, 
                _context.Mvb, 
                content, 
                _context.IsInline, 
                _context.InclusionSet, 
                _context.Dependency);
            context = context.AddIncludeFile(currentFilePath);

            var result = _compositor.Markup(context, _parameters);
            result = SkipYamlHeader(result);

            renderer.Write(result);
        }

        private string SkipYamlHeader(string content)
        {
            return YamlHeaderRegex.Replace(content, "");
        }
    }
}
