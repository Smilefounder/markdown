using Markdig.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Markdig.Parsers;

namespace MarkdigEngine.Extensions
{
    public class MonikerRangeBlock : ContainerBlock
    {
        public string MonikerRange { get; set; }
        public int ColonCount { get; set; }
        public MonikerRangeBlock(BlockParser parser) : base(parser)
        {
        }
    }
}
