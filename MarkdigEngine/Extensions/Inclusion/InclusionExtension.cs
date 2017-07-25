using Markdig.Renderers;
using Markdig;
using Markdig.Parsers.Inlines;
using Microsoft.DocAsCode.Plugins;
using Markdig.Parsers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using static Markdig.Syntax.Inlines.LinkInline;
using Microsoft.DocAsCode.Common;

namespace MarkdigEngine
{
    /// <summary>
    /// Extension to enable extension IncludeFile.
    /// </summary>
    public class InclusionExtension : IMarkdownExtension
    {
        public MarkdownContext Context { get; set; }

        private MarkdownServiceParameters _parameters;

        public InclusionExtension(MarkdownContext context, MarkdownServiceParameters parameters)
        {
            Context = context;
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
                    htmlRenderer.ObjectRenderers.Insert(0, new HtmlInclusionInlineRenderer(pipeline, Context, _parameters));
                }

                if (!htmlRenderer.ObjectRenderers.Contains<HtmlInclusionBlockRenderer>())
                {
                    htmlRenderer.ObjectRenderers.Insert(0, new HtmlInclusionBlockRenderer(Context, _parameters));
                }
            }
        }

        internal static ProcessDocumentDelegate GetProcessDocumentDelegate(MarkdownContext context)
        {
            return (MarkdownDocument document) =>
            {
                UpdateLinks(document, context);
            };
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
                    if (PathUtility.IsRelativePath(originalUrl) && !RelativePath.IsPathFromWorkingFolder(originalUrl) && !originalUrl.StartsWith("#"))
                    {
                        var currentFilePath = ((RelativePath)context.FilePath);
                        var newUrl = ((RelativePath)originalUrl).BasedOn(currentFilePath);
                        linkInline.GetDynamicUrl = () => { return newUrl; };
                    }
                }
            }
        }
    }
}
