using Markdig.Parsers;
using Markdig.Syntax;

namespace MarkdigEngine.Extensions
{
    public class InclusionBlock : LeafBlock
    {
        public InclusionContext Context { get; set; }

        public InclusionBlock(BlockParser parser): base(parser)
        {

        }
    }
}
