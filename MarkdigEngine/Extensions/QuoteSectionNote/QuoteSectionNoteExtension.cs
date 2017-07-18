using Markdig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Markdig.Renderers;
using Markdig.Parsers;
using MarkdigEngine.Parsers;
using Microsoft.DocAsCode.Common;
using Markdig.Renderers.Html;
using Microsoft.DocAsCode.Plugins;

namespace MarkdigEngine
{
    class QuoteSectionNoteExtension : IMarkdownExtension
    {
        private MarkdownServiceParameters _parameters;

        public QuoteSectionNoteExtension(MarkdownServiceParameters parameters)
        {
            _parameters = parameters;
        }

        void IMarkdownExtension.Setup(MarkdownPipelineBuilder pipeline)
        {
            if (!pipeline.BlockParsers.Replace<QuoteBlockParser>(new QuoteSectionNoteParser()))
            {
                Logger.LogWarning($"Can't find QuoteBlockParser to replace, insert QuoteSectionNoteParser directly.");
                pipeline.BlockParsers.Insert(0, new QuoteSectionNoteParser());
            }
        }

        void IMarkdownExtension.Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            var htmlRenderer = renderer as HtmlRenderer;
            if (htmlRenderer != null)
            {
                QuoteSectionNoteRender quoteSectionNoteRender = new QuoteSectionNoteRender(_parameters.Tokens);

                if (!renderer.ObjectRenderers.Replace<QuoteBlockRenderer>(quoteSectionNoteRender))
                {
                    Logger.LogWarning($"Can't find QuoteBlockRenderer to replace, insert QuoteSectionNoteRender directly.");
                    renderer.ObjectRenderers.Insert(0, quoteSectionNoteRender);
                }
            }
        }
    }
}
