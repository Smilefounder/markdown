using Markdig;

namespace MarkdigEngine
{
    public class MarkdigMarked
    {
        public static string Markup(string src, string basePath = null, string filePath = null)
        {
            var context = new MarkdownContext
            {
                BasePath = basePath,
                FilePath = filePath
            };

            var pipeline = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .UseDfmExtensions(context)
                .Build();

            return Markdown.ToHtml(src, pipeline);
        }
    }
}
