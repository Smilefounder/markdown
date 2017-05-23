using Markdig;

namespace MarkdigEngine
{
    public class MarkdigMarked
    {
        public static string Markup(string src)
        {
            var pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .UseDfmExtensions()
                .Build();
            return Markdown.ToHtml(src, pipeline);
        }
    }
}
