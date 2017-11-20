using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Markdig;
using Markdig.Renderers;
using Markdig.Parsers;
using Markdig.Extensions.AutoIdentifiers;
using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using Markdig.Helpers;
using Markdig.Renderers.Html;

namespace MarkdigEngine.Extensions
{
    public class DFMHeadingIdExtension : IMarkdownExtension
    {
        private const string AutoIdentifierKey = "AutoIdentifier";
        private static readonly Regex OpenARegex = new Regex(@"^\<a +(?:name|id)=\""([\w \-\.]+)\"" *\>$", RegexOptions.Compiled);
        private static readonly Regex CloseARegex = new Regex(@"^\<\/a\>$", RegexOptions.Compiled);

        public void Setup(MarkdownPipelineBuilder pipeline)
        {
            var headingBlockParser = pipeline.BlockParsers.Find<HeadingBlockParser>();
            if (headingBlockParser != null)
            {
                // Install a hook on the HeadingBlockParser when a HeadingBlock is actually processed
                headingBlockParser.Closed -= HeadingBlockParser_Closed_DFM;
                headingBlockParser.Closed += HeadingBlockParser_Closed_DFM;
            }
            var paragraphBlockParser = pipeline.BlockParsers.FindExact<ParagraphBlockParser>();
            if (paragraphBlockParser != null)
            {
                // Install a hook on the ParagraphBlockParser when a HeadingBlock is actually processed as a Setex heading
                paragraphBlockParser.Closed -= HeadingBlockParser_Closed_DFM;
                paragraphBlockParser.Closed += HeadingBlockParser_Closed_DFM;
            }
        }

        public void Setup(MarkdownPipeline pipeline, IMarkdownRenderer renderer)
        {
        }

        private void HeadingBlockParser_Closed_DFM(BlockProcessor processor, Block block)
        {
            var headingBlock = block as HeadingBlock;
            if (headingBlock == null)
            {
                return;
            }

            // Same as AutoIdentifierExtension, register a LinkReferenceDefinition at the document level
            var headingLine = headingBlock.Lines.Lines[0];

            var text = headingLine.ToString();

            var linkRef = new HeadingLinkReferenceDefinition()
            {
                Heading = headingBlock,
                CreateLinkInline = CreateLinkInlineForHeading
            };
            processor.Document.SetLinkReferenceDefinition(text, linkRef);

            // Then we register after inline have been processed to actually generate the proper #id
            headingBlock.ProcessInlinesEnd += GetDFM_HeadingBlock_ProcessInlinesEnd(headingLine.ToString());
        }

        /// <summary>
        /// Callback when there is a reference to found to a heading. 
        /// Note that reference are only working if they are declared after.
        /// </summary>
        private Inline CreateLinkInlineForHeading(InlineProcessor inlineState, LinkReferenceDefinition linkRef, Inline child)
        {
            var headingRef = (HeadingLinkReferenceDefinition)linkRef;
            return new LinkInline()
            {
                // Use GetDynamicUrl to allow late binding of the Url (as a link may occur before the heading is declared and
                // the inlines of the heading are actually processed by HeadingBlock_ProcessInlinesEnd)
                GetDynamicUrl = () => HtmlHelper.Unescape("#" + headingRef.Heading.GetAttributes().Id),
                Title = HtmlHelper.Unescape(linkRef.Title),
            };
        }


        /// <summary>
        /// Process the source of the heading to create a unique identifier
        /// </summary>
        /// <param name="processor">The processor.</param>
        /// <param name="inline">The inline.</param>
        private ProcessInlineDelegate GetDFM_HeadingBlock_ProcessInlinesEnd(string source)
        {
            return (InlineProcessor processor, Inline inline) =>
            {
                var identifiers = processor.Document.GetData(AutoIdentifierKey) as HashSet<string>;
                if (identifiers == null)
                {
                    identifiers = new HashSet<string>();
                    processor.Document.SetData(AutoIdentifierKey, identifiers);
                }

                var headingBlock = (HeadingBlock)processor.Block;
                if (headingBlock == null)
                {
                    return;
                }

                // If id is already set, don't try to modify it
                var attributes = processor.Block.GetAttributes();
                if (attributes.Id != null)
                {
                    return;
                }

                string headingText = GetARegexIdAndRemove(headingBlock);
                if (string.IsNullOrEmpty(headingText))
                {
                    headingText = LinkHelper.UrilizeAsGfm(source);
                }

                var baseHeadingId = string.IsNullOrEmpty(headingText) ? "section" : headingText;
                int index = 0;
                var headingId = baseHeadingId;
                var headingBuffer = StringBuilderCache.Local();
                while (!identifiers.Add(headingId))
                {
                    index++;
                    headingBuffer.Append(baseHeadingId);
                    headingBuffer.Append('-');
                    headingBuffer.Append(index);
                    headingId = headingBuffer.ToString();
                    headingBuffer.Length = 0;
                }

                attributes.Id = headingId;
            };
        }

        private string GetARegexIdAndRemove(HeadingBlock headingBlock)
        {
            if (headingBlock?.Inline == null || headingBlock.Inline.Count() <= 2)
            {
                return null;
            }

            var openInline = headingBlock?.Inline?.FirstChild as HtmlInline;
            var closeInline = openInline?.NextSibling as HtmlInline;

            if (openInline == null || closeInline == null)
            {
                return null;
            }

            var openMatch = OpenARegex.Match(openInline.Tag);

            if (!openMatch.Success)
            {
                return null;
            }

            if (!CloseARegex.IsMatch(closeInline.Tag))
            {
                return null;
            }

            openInline.Remove();
            closeInline.Remove();
            return openMatch.Groups[1].Value;
        }
    }
}
