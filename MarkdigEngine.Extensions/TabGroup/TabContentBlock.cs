using System.Collections.Generic;
using Markdig.Syntax;

namespace MarkdigEngine.Extensions
{
    public class TabContentBlock : ContainerBlock
    {
        public TabContentBlock(List<Block> blocks) : base(null)
        {
            foreach(var item in blocks)
            {
                Add(item);
            }
        }
    }
}