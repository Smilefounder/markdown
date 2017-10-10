using Markdig.Syntax;

namespace MarkdigEngine.Extensions
{
    public class TabItemBlock
    {
        public HeadingBlock HeadToken { get; }

        public string Id { get; }

        public string Condition { get; }

        public TabTitleBlock Title { get; }

        public TabContentBlock Content { get; }

        public bool Visible { get; }

        public TabItemBlock(HeadingBlock headToken, string id, string condition, TabTitleBlock title, TabContentBlock content)
        {
            HeadToken = headToken;
            Id = id;
            Condition = condition;
            Title = title;
            Content = content;
        }
    }
}