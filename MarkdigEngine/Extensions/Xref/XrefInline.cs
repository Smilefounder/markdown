using Markdig.Syntax.Inlines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkdigEngine
{
    public class XrefInline : LeafInline
    {
        public string Href { get; set; }
    }
}
