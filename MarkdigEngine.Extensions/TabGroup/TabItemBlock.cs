﻿namespace MarkdigEngine.Extensions
{
    public class TabItemBlock
    {
        public string Id { get; }

        public string Condition { get; }

        public TabTitleBlock Title { get; }

        public TabContentBlock Content { get; }

        public bool Visible { get; }

        public TabItemBlock(string id, string condition, TabTitleBlock title, TabContentBlock content, bool visible)
        {
            Id = id;
            Condition = condition;
            Title = title;
            Content = content;
            Visible = visible;
        }
    }
}