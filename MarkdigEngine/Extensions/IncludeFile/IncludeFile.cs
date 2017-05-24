using Markdig;
using Markdig.Parsers;
using Markdig.Syntax;

namespace MarkdigEngine.Extensions.IncludeFile
{
    public class IncludeFile : LeafBlock
    {
        public string Title { get; set; }

        public string RefFilePath { get; set; }

        public string Syntax { get; set; }

        public MarkdownPipeline Pipeline { get; set; }

        public IncludeFile(BlockParser parser): base(parser)
        {

        }
    }
}
