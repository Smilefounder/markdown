// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace MarkdigEngine.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Markdig;
    using Markdig.Helpers;
    using Markdig.Parsers;
    using Markdig.Renderers;

    public class InlineOnlyExtentsion : IMarkdownExtension
    {
        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            var paragraphBlockParser = pipeline.BlockParsers.FindExact<ParagraphBlockParser>() ?? new ParagraphBlockParser();
            pipeline.BlockParsers.Clear();
            pipeline.BlockParsers.Add(paragraphBlockParser);
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
            if (renderer is HtmlRenderer htmlRenderer)
            {
                htmlRenderer.ImplicitParagraph = true;
            }
        }
    }
}