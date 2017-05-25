using Markdig.Parsers;
using Markdig.Syntax;

namespace MarkdigEngine
{
    public class IncludeFileBlock : LeafBlock
    {
        public IncludeFileContext Context { get; set; }

        public IncludeFileBlock(BlockParser parser): base(parser)
        {

        }
    }
}
