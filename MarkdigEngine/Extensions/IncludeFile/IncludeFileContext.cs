using Markdig;

namespace MarkdigEngine
{
    public class IncludeFileContext
    {
        public string Title { get; set; }

        public string RefFilePath { get; set; }

        public string Syntax { get; set; }

        public MarkdownPipeline Pipeline { get; set; }
    }
}
