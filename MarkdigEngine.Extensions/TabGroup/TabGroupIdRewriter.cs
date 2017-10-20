﻿using System.Collections.Generic;

using Markdig.Syntax;
using MarkdigEngine.Extensions;

namespace MarkdigEngine
{
    public class TabGroupIdRewriter : IMarkdownObjectRewriter
    {
        private Dictionary<string, int> _dict = new Dictionary<string, int>();

        public void PostProcess(IMarkdownObject markdownObject)
        {
        }

        public void PreProcess(IMarkdownObject markdownObject)
        {
        }

        public IMarkdownObject Rewrite(IMarkdownObject markdownObject)
        {
            if (markdownObject is TabGroupBlock block)
            {
                var groupId = block.Id;
                while (true)
                {
                    if (_dict.TryGetValue(groupId, out int index))
                    {
                        groupId = $"{groupId}-{index}";
                        index += 1;
                        block.Id = groupId;
                        return block;
                    }
                    else
                    {
                        _dict.Add(groupId, 1);
                        return markdownObject;
                    }
                }
            }

            return markdownObject;
        }
    }
}