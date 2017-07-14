using Markdig.Parsers;
using Markdig.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarkdigEngine
{
    public class QuoteSectionNoteBlock : ContainerBlock
    {
        public QuoteSectionNoteBlock(BlockParser parser) : base(parser)
        {
        }

        public char QuoteChar { get; set; }

        public QuoteSectionNoteType QuoteType { get; set; }

        public string SectionAttributeString { get; set; }

        public string NoteTypeString { get; set; }

        public string VideoLink { get; set; }
    }

    public enum QuoteSectionNoteType
    {
        MarkdownQuote = 0,
        DFMSection,
        DFMNote,
        DFMVideo
    }
}
