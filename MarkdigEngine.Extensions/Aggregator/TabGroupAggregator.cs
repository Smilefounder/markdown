using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.RegularExpressions;

using Markdig.Syntax;
using Markdig.Syntax.Inlines;
using Markdig.Extensions.AutoIdentifiers;

namespace MarkdigEngine.Extensions
{
    public class TabGroupAggregator : BlockAggregator<HeadingBlock>
    {
        private static readonly Regex HrefRegex = new Regex(@"^#tab\/([a-zA-Z0-9\-]+)(?:\/([a-zA-Z0-9\-]+)?)?$", RegexOptions.Compiled);

        protected override bool AggregateCore(HeadingBlock headBlock, BlockAggregateContext context)
        {
            var pair = ParseHeading(headBlock);
            if (pair == null)
            {
                return false;
            }
            int offset = 1;
            var items = new List<TabItemBlock>();
            var list = new List<Block>();
            while (true)
            {
                var block = context.LookAhead(offset);
                switch (block)
                {
                    case HeadingBlock head:
                        var temp = ParseHeading(head);
                        if (temp == null)
                        {
                            goto default;
                        }
                        items.Add(CreateTabItem(headBlock, pair, list));
                        pair = temp;
                        list.Clear();
                        break;
                    case ThematicBreakBlock hr:
                        offset++;
                        goto case null;
                    case null:
                        items.Add(CreateTabItem(headBlock, pair, list));
                        context.AggregateTo(
                            new TabGroupBlock(
                                headBlock,
                                Guid.NewGuid().ToString(),
                                items.ToImmutableArray()),
                            offset);
                        return true;
                    default:
                        list.Add(block);
                        break;
                }
                offset++;
            }
        }

        private static TabItemBlock CreateTabItem(
            HeadingBlock headToken,
            Tuple<string, string, string> pair,
            List<Block> list)
        {
            var title = new TabTitleBlock(headToken, pair.Item3);
            var content = new TabContentBlock(headToken, list.ToImmutableArray());

            return new TabItemBlock(
                headToken,
                pair.Item1,
                pair.Item2,
                title,
                content);
        }

        private static Tuple<string, string, string> ParseHeading(HeadingBlock block)
        {
            var inlines = block.Inline.ToList();
            if (inlines.Count == 1 && inlines.First() is LinkInline link)
            {
                var m = HrefRegex.Match(link.Url);
                if (m.Success)
                {
                    return Tuple.Create(m.Groups[1].Value, m.Groups[2].Value, link.ToString());
                }
            }

            return null;
        }
    }
}
