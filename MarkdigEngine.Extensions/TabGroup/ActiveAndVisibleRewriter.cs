using System.Collections.Generic;

using Markdig.Syntax;
using Microsoft.DocAsCode.Common;

namespace MarkdigEngine.Extensions
{
    public class ActiveAndVisibleRewriter : IMarkdownObjectRewriter
    {
        private HashSet<string> selectedTabIds = new HashSet<string>();

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
                var items = block.Items;
                var firstVisibleTab = -1;
                var active = -1;

                for (var i = 0; i < items.Length; i++)
                {
                    var tab = items[i];
                    var visible = string.IsNullOrEmpty(tab.Condition) || selectedTabIds.Contains(tab.Condition);
                    if (visible && firstVisibleTab == -1)
                    {
                        firstVisibleTab = i;
                    }

                    if (active == -1 && visible && selectedTabIds.Contains(tab.Id))
                    {
                        active = i;
                    }

                    if (tab.Visible != visible)
                    {
                        items[i].Visible = visible;
                    }
                }

                if (active == -1)
                {
                    if (firstVisibleTab != -1)
                    {
                        active = firstVisibleTab;
                        selectedTabIds.Add(items[firstVisibleTab].Id);
                    }
                    else
                    {
                        active = 0;
                        Logger.LogWarning("All tabs are hidden in the tab group");
                    }
                }

                block.ActiveTabIndex = active;
                return block;
            }

            return markdownObject;
        }
    }
}
