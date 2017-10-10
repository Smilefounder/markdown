using Markdig.Syntax;

namespace MarkdigEngine.Extensions
{
    public class TabTitleBlock
    {
        public HeadingBlock HeadToken { get; }
        public string Content { get; }

        public TabTitleBlock(HeadingBlock headToken, string content)
        {
            HeadToken = headToken;
            Content = content;
        }
    }
}