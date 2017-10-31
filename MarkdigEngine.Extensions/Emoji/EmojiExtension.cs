using Markdig;
using Markdig.Renderers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkdigEngine.Extensions
{
    public class EmojiExtension : IMarkdownExtension
    {
        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            if (!pipeline.InlineParsers.Contains<EmojiParser>())
            {
                pipeline.InlineParsers.Insert(0, new EmojiParser());
            }
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
        }
    }
}
