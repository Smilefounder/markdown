using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Markdig.Renderers;
using Markdig.Renderers.Html;

using Microsoft.DocAsCode.Common;

namespace MarkdigEngine
{
    public class HtmlCodeSnippetRenderer : HtmlObjectRenderer<CodeSnippet>
    {
        private MarkdownContext _context;

        private static readonly string m_CSharpCodeSnippetCommentStartLineTemplate = "//<snippet{0}>";
        private static readonly string m_CSharpCodeSnippetCommentEndLineTemplate = "//</snippet{0}>";

        public HtmlCodeSnippetRenderer(MarkdownContext context)
        {
            _context = context;
        }

        protected override void Write(HtmlRenderer renderer, CodeSnippet obj)
        {
            if (renderer.EnableHtmlForInline)
            {
                renderer.Write("<pre><code").Write(obj.ToAttributeString()).Write(">");
            }

            renderer.WriteEscape(GetContent(obj));

            if (renderer.EnableHtmlForInline)
            {
                renderer.Write("</code></pre>");
            }
        }

        private string GetContent(CodeSnippet obj)
        {
            var refFileRelativePath = ((RelativePath)obj.CodePath).BasedOn((RelativePath)_context.FilePath);
            _context.ReportDependency(refFileRelativePath);

            var refPath = Path.Combine(_context.BasePath, refFileRelativePath.RemoveWorkingFolder());
            var allLines = File.ReadAllLines(refPath);
            var allCodeRanges = obj.CodeRanges ?? new List<CodeRange>();

            if (!string.IsNullOrEmpty(obj.TagName))
            {
                var codeRange = new CodeRange();
                var startTag = string.Format(m_CSharpCodeSnippetCommentStartLineTemplate, obj.TagName).ToUpper();
                var endTag = string.Format(m_CSharpCodeSnippetCommentEndLineTemplate, obj.TagName).ToUpper();

                allCodeRanges.Add(new CodeRange { Start = GetTagLineNumber(allLines, startTag) + 1, End = GetTagLineNumber(allLines, endTag) - 1 });
            }

            var showCode = new StringBuilder();

            for (int lineNumber = 0; lineNumber < allLines.Length; lineNumber++)
            {
                if (IsLineInRange(lineNumber + 1, allCodeRanges))
                {
                    showCode.AppendLine(allLines[lineNumber]);
                }
            }

            return showCode.ToString();
        }

        private bool IsLineInRange(int lineNumber, List<CodeRange> allCodeRanges)
        {
            for (int rangeNumber = 0; rangeNumber < allCodeRanges.Count(); rangeNumber++)
            {
                var range = allCodeRanges[rangeNumber];
                if (lineNumber >= range.Start && lineNumber <= range.End)
                    return true;
            }

            return false;
        }

        private int GetTagLineNumber(string[] lines, string tagLine)
        {
            for (int index = 0; index < lines.Length; index++)
            {
                var line = lines[index];
                var targetColumn = 0;
                var match = true;

                for (int column = 0; column < line.Length; column++)
                {
                    var c = line[column];
                    if (c != ' ')
                    {
                        if (targetColumn >= tagLine.Length || tagLine[targetColumn] != Char.ToUpper(c))
                        {
                            match = false;
                            break;
                        }

                        targetColumn++;
                    }
                }

                if (match && targetColumn == tagLine.Length) return index + 1;
            }

            return -1;
        }
    }
}
