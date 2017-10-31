using Markdig.Helpers;
using Markdig.Syntax.Inlines;

namespace MarkdigEngine.Extensions
{
    public class EmojiInline : LiteralInline
    {
        public EmojiInline()
        {
        }

        public EmojiInline(string content)
        {
            Content = new StringSlice(content);
        }
        
        public string Match { get; set; }
    }
}
