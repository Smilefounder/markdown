using Markdig;

namespace MarkdigEngine
{
    public class MarkdigMarked
    {
        public static string Markup(string src, MarkdownContext context)
        {
            var pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .UseDfmExtensions(context)
                .Build();

            return Markdown.ToHtml(src, pipeline);
        }
    }
}
