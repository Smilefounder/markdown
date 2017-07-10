using Markdig.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Markdig.Helpers;
using Markdig.Syntax;

namespace MarkdigEngine
{
    class XrefInlineShortParser : InlineParser
    {
        private const string Punctuation = ".,;:!?`~";

        public XrefInlineShortParser()
        {
            OpeningCharacters = new[] { '@' };
        }

        public override bool Match(InlineProcessor processor, ref StringSlice slice)
        {
            var c = slice.PeekCharExtra(-1);

            if (c == '\\' || c == ' ')
            {
                return false;
            }

            var href = processor.StringBuilders.Get();
            var saved = slice;
            var startChar = '\0';
            int line;
            int column;

            c = slice.NextChar();

            if (c == '\'' || c == '"')
            {
                startChar = c;
                c = slice.NextChar();
            }

            if(startChar != '\0')
            {
                while (c != startChar && c != '\0' && c != '\n')
                {
                    href.Append(c);
                    c = slice.NextChar();
                }
                slice.NextChar();
            }
            else
            {
                while (c != '\0' && c != ' ' && c != '\n')
                {
                    if(Punctuation.Contains(c))
                    {
                        var next = slice.PeekCharExtra(1);
                        if (next == ' ' || next == '\0' || c == '\n' || Punctuation.Contains(next)) break;
                    }
                    href.Append(c);
                    c = slice.NextChar();
                }
            }

            var xrefInline = new XrefInline
            {
                Href = href.ToString().Trim(),
                Span = new SourceSpan(processor.GetSourcePosition(saved.Start, out line, out column), processor.GetSourcePosition(slice.Start - 1)),
                Line = line,
                Column = column
            };
            processor.Inline = xrefInline;

            return true;
        }
    }
}
