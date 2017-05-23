using Markdig;

namespace MarkdigEngine
{
    public class MarkdigMarked
    {
        public static string Markup(string src, string filePath = null)
        {
            var context = new MarkdownContext
            {
                FilePath = filePath
            };
            var pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().UseDfmExtensions(context).Build();
            return Markdown.ToHtml(src, pipeline);
        }
    }
}
