using Markdig;
using Markdig.Parsers;
using Markdig.Syntax;

namespace MarkdigEngine.Extensions.IncludeFile
{
    public class IncludeFile : LeafBlock
    {
        public string Title { get; set; }

        public string FilePath { get; set; }

        public string Command { get; set; }

        public MarkdownPipeline Pipeline { get; set; }

        public IncludeFile(BlockParser parser): base(parser)
        {

        }
    }
}
