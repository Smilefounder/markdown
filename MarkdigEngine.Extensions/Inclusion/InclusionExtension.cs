using Markdig.Renderers;
using Markdig;
using Markdig.Parsers.Inlines;
using Markdig.Parsers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using Microsoft.DocAsCode.Common;
using Microsoft.DocAsCode.Plugins;

namespace MarkdigEngine.Extensions
{
    /// <summary>
    /// Extension to enable extension IncludeFile.
    /// </summary>
    public class InclusionExtension : IMarkdownExtension
    {
        private IMarkdigCompositor _compositor;
        private MarkdownContext _context;
        private MarkdownServiceParameters _parameters;

        public InclusionExtension(IMarkdigCompositor compositor, MarkdownContext context, MarkdownServiceParameters parameters)
        {
            _compositor = compositor;
            _context = context;
            _parameters = parameters;
        }

        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            pipeline.BlockParsers.AddIfNotAlready<InclusionBlockParser>();
            pipeline.InlineParsers.InsertBefore<LinkInlineParser>(new InclusionInlineParser());
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            if (renderer is HtmlRenderer htmlRenderer)
            {
                if (!htmlRenderer.ObjectRenderers.Contains<HtmlInclusionInlineRenderer>())
                {
                    htmlRenderer.ObjectRenderers.Insert(0, new HtmlInclusionInlineRenderer(_compositor, _context, _parameters));
                }

                if (!htmlRenderer.ObjectRenderers.Contains<HtmlInclusionBlockRenderer>())
                {
                    htmlRenderer.ObjectRenderers.Insert(0, new HtmlInclusionBlockRenderer(_compositor, _context, _parameters));
                }
            }
        }

        public static ProcessDocumentDelegate GetProcessDocumentDelegate(MarkdownContext context)
        {
            return document => UpdateLinks(document, context);
        }

        private static void UpdateLinks(MarkdownObject markdownObject, MarkdownContext context)
        {
            if (markdownObject == null || context == null || string.IsNullOrEmpty(context.FilePath)) return;

            if (markdownObject is ContainerBlock containerBlock)
            {
                foreach (var subBlock in containerBlock)
                {
                    UpdateLinks(subBlock, context);
                }
            }
            else if (markdownObject is LeafBlock leafBlock)
            {
                if (leafBlock.Inline != null)
                {
                    foreach (var subInline in leafBlock.Inline)
                    {
                        UpdateLinks(subInline, context);
                    }
                }
            }
            else if (markdownObject is ContainerInline containerInline)
            {
                foreach (var subInline in containerInline)
                {
                    UpdateLinks(subInline, context);
                }

                if (markdownObject is LinkInline linkInline)
                {
                    var originalUrl = linkInline.Url;
                    if (RelativePath.IsRelativePath(originalUrl) && PathUtility.IsRelativePath(originalUrl) && !RelativePath.IsPathFromWorkingFolder(originalUrl) && !originalUrl.StartsWith("#"))
                    {
                        linkInline.GetDynamicUrl = () =>
                        {
                            return (RelativePath)originalUrl - (RelativePath)context.FilePath;
                        };
                    }
                }
            }
        }
    }
}
