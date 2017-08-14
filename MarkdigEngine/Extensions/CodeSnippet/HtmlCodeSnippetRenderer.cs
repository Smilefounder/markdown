using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Markdig.Renderers;
using Markdig.Renderers.Html;

using Microsoft.DocAsCode.Common;
using Markdig.Helpers;

namespace MarkdigEngine
{
    public class HtmlCodeSnippetRenderer : HtmlObjectRenderer<CodeSnippet>
    {
        private MarkdownContext _context;
        private const string tagPrefix = "snippet";

        // C# code snippet comment block: // <[/]snippetname>
        private static readonly string CSharpCodeSnippetCommentStartLineTemplate = "//<{tagname}>";
        private static readonly string CSharpCodeSnippetCommentEndLineTemplate = "//</{tagname}>";

        // C# code snippet region block: start -> #region snippetname, end -> #endregion
        private static readonly string CSharpCodeSnippetRegionStartLineTemplate = "#region{tagname}";
        private static readonly string CSharpCodeSnippetRegionEndLineTemplate = "#endregion";

        // VB code snippet comment block: ' <[/]snippetname>
        private static readonly string VBCodeSnippetCommentStartLineTemplate = "'<{tagname}>";
        private static readonly string VBCodeSnippetCommentEndLineTemplate = "'</{tagname}>";

        // VB code snippet Region block: start -> # Region "snippetname", end -> # End Region
        private static readonly string VBCodeSnippetRegionRegionStartLineTemplate = "#region{tagname}";
        private static readonly string VBCodeSnippetRegionRegionEndLineTemplate = "#endregion";

        // C++ code snippet block: // <[/]snippetname>
        private static readonly string CPlusPlusCodeSnippetCommentStartLineTemplate = "//<{tagname}>";
        private static readonly string CPlusPlusCodeSnippetCommentEndLineTemplate = "//</{tagname}>";

        // F# code snippet block: // <[/]snippetname>
        private static readonly string FSharpCodeSnippetCommentStartLineTemplate = "//<{tagname}>";
        private static readonly string FSharpCodeSnippetCommentEndLineTemplate = "//</{tagname}>";

        // XML code snippet block: <!-- <[/]snippetname> -->
        private static readonly string XmlCodeSnippetCommentStartLineTemplate = "<!--<{tagname}>-->";
        private static readonly string XmlCodeSnippetCommentEndLineTemplate = "<!--</{tagname}>-->";

        // XAML code snippet block: <!-- <[/]snippetname> -->
        private static readonly string XamlCodeSnippetCommentStartLineTemplate = "<!--<{tagname}>-->";
        private static readonly string XamlCodeSnippetCommentEndLineTemplate = "<!--</{tagname}>-->";

        // HTML code snippet block: <!-- <[/]snippetname> -->
        private static readonly string HtmlCodeSnippetCommentStartLineTemplate = "<!--<{tagname}>-->";
        private static readonly string HtmlCodeSnippetCommentEndLineTemplate = "<!--</{tagname}>-->";

        // Sql code snippet block: -- <[/]snippetname>
        private static readonly string SqlCodeSnippetCommentStartLineTemplate = "--<{tagname}>";
        private static readonly string SqlCodeSnippetCommentEndLineTemplate = "--</{tagname}>";

        // Javascript code snippet block: <!-- <[/]snippetname> -->
        private static readonly string JavaScriptSnippetCommentStartLineTemplate = "//<{tagname}>";
        private static readonly string JavaScriptSnippetCommentEndLineTemplate = "//</{tagname}>";

        // Java code snippet comment block: // <[/]snippetname>
        private static readonly string JavaCodeSnippetCommentStartLineTemplate = "//<{tagname}>";
        private static readonly string JavaCodeSnippetCommentEndLineTemplate = "//</{tagname}>";

        // Python code snippet comment block: # <[/]snippetname>
        private static readonly string PythonCodeSnippetCommentStartLineTemplate = "#<{tagname}>";
        private static readonly string PythonCodeSnippetCommentEndLineTemplate = "#</{tagname}>";

        // Language names and aliases fllow http://highlightjs.readthedocs.org/en/latest/css-classes-reference.html#language-names-and-aliases
        // Language file extensions follow https://github.com/github/linguist/blob/master/lib/linguist/languages.yml
        // Currently only supports parts of the language names, aliases and extensions
        // Later we can move the repository's supported/custom language names, aliases, extensions and corresponding comments regexes to docfx build configuration
        private static readonly IReadOnlyDictionary<string, List<CodeSnippetExtrator>> CodeLanguageExtractors =
            new Dictionary<string, List<CodeSnippetExtrator>>(StringComparer.OrdinalIgnoreCase)
            {
                [".cs"] = new List<CodeSnippetExtrator>
                {
                    new CodeSnippetExtrator(CSharpCodeSnippetCommentStartLineTemplate, CSharpCodeSnippetCommentEndLineTemplate),
                    new CodeSnippetExtrator(CSharpCodeSnippetRegionStartLineTemplate, CSharpCodeSnippetRegionEndLineTemplate, false)
                },
                ["cs"] = new List<CodeSnippetExtrator>
                {
                    new CodeSnippetExtrator(CSharpCodeSnippetCommentStartLineTemplate, CSharpCodeSnippetCommentEndLineTemplate),
                    new CodeSnippetExtrator(CSharpCodeSnippetRegionStartLineTemplate, CSharpCodeSnippetRegionEndLineTemplate, false)
                },
                ["csharp"] = new List<CodeSnippetExtrator>
                {
                    new CodeSnippetExtrator(CSharpCodeSnippetCommentStartLineTemplate, CSharpCodeSnippetCommentEndLineTemplate),
                    new CodeSnippetExtrator(CSharpCodeSnippetRegionStartLineTemplate, CSharpCodeSnippetRegionEndLineTemplate, false)
                },
                [".vb"] = new List<CodeSnippetExtrator>
                {
                    new CodeSnippetExtrator(VBCodeSnippetCommentStartLineTemplate, VBCodeSnippetCommentEndLineTemplate),
                    new CodeSnippetExtrator(VBCodeSnippetRegionRegionStartLineTemplate, VBCodeSnippetRegionRegionEndLineTemplate, false)
                },
                ["vb"] = new List<CodeSnippetExtrator>
                {
                    new CodeSnippetExtrator(VBCodeSnippetCommentStartLineTemplate, VBCodeSnippetCommentEndLineTemplate),
                    new CodeSnippetExtrator(VBCodeSnippetRegionRegionStartLineTemplate, VBCodeSnippetRegionRegionEndLineTemplate, false)
                },
                ["vbnet"] = new List<CodeSnippetExtrator>
                {
                    new CodeSnippetExtrator(VBCodeSnippetCommentStartLineTemplate, VBCodeSnippetCommentEndLineTemplate),
                    new CodeSnippetExtrator(VBCodeSnippetRegionRegionStartLineTemplate, VBCodeSnippetRegionRegionEndLineTemplate, false)
                },
                [".cpp"] = new List<CodeSnippetExtrator>
                {
                    new CodeSnippetExtrator(CPlusPlusCodeSnippetCommentStartLineTemplate, CPlusPlusCodeSnippetCommentEndLineTemplate)
                },
                [".h"] = new List<CodeSnippetExtrator>
                {
                    new CodeSnippetExtrator(CPlusPlusCodeSnippetCommentStartLineTemplate, CPlusPlusCodeSnippetCommentEndLineTemplate)
                },
                [".hpp"] = new List<CodeSnippetExtrator>
                {
                    new CodeSnippetExtrator(CPlusPlusCodeSnippetCommentStartLineTemplate, CPlusPlusCodeSnippetCommentEndLineTemplate)
                },
                [".c"] = new List<CodeSnippetExtrator>
                {
                    new CodeSnippetExtrator(CPlusPlusCodeSnippetCommentStartLineTemplate, CPlusPlusCodeSnippetCommentEndLineTemplate)
                },
                [".cc"] = new List<CodeSnippetExtrator>
                {
                    new CodeSnippetExtrator(CPlusPlusCodeSnippetCommentStartLineTemplate, CPlusPlusCodeSnippetCommentEndLineTemplate)
                },
                ["cpp"] = new List<CodeSnippetExtrator>
                {
                    new CodeSnippetExtrator(CPlusPlusCodeSnippetCommentStartLineTemplate, CPlusPlusCodeSnippetCommentEndLineTemplate)
                },
                ["c++"] = new List<CodeSnippetExtrator>
                {
                    new CodeSnippetExtrator(CPlusPlusCodeSnippetCommentStartLineTemplate, CPlusPlusCodeSnippetCommentEndLineTemplate)
                },
                ["fs"] = new List<CodeSnippetExtrator>
                {
                    new CodeSnippetExtrator(FSharpCodeSnippetCommentStartLineTemplate, FSharpCodeSnippetCommentEndLineTemplate)
                },
                ["fsharp"] = new List<CodeSnippetExtrator>
                {
                    new CodeSnippetExtrator(FSharpCodeSnippetCommentStartLineTemplate, FSharpCodeSnippetCommentEndLineTemplate)
                },
                [".fs"] = new List<CodeSnippetExtrator>
                {
                    new CodeSnippetExtrator(FSharpCodeSnippetCommentStartLineTemplate, FSharpCodeSnippetCommentEndLineTemplate)
                },
                [".fsi"] = new List<CodeSnippetExtrator>
                {
                    new CodeSnippetExtrator(FSharpCodeSnippetCommentStartLineTemplate, FSharpCodeSnippetCommentEndLineTemplate)
                },
                [".fsx"] = new List<CodeSnippetExtrator>
                {
                    new CodeSnippetExtrator(FSharpCodeSnippetCommentStartLineTemplate, FSharpCodeSnippetCommentEndLineTemplate)
                },
                [".xml"] = new List<CodeSnippetExtrator>
                {
                    new CodeSnippetExtrator(XmlCodeSnippetCommentStartLineTemplate, XmlCodeSnippetCommentEndLineTemplate)
                },
                [".csdl"] = new List<CodeSnippetExtrator>
                {
                    new CodeSnippetExtrator(XmlCodeSnippetCommentStartLineTemplate, XmlCodeSnippetCommentEndLineTemplate)
                },
                [".edmx"] = new List<CodeSnippetExtrator>
                {
                    new CodeSnippetExtrator(XmlCodeSnippetCommentStartLineTemplate, XmlCodeSnippetCommentEndLineTemplate)
                },
                ["xml"] = new List<CodeSnippetExtrator>
                {
                    new CodeSnippetExtrator(XmlCodeSnippetCommentStartLineTemplate, XmlCodeSnippetCommentEndLineTemplate)
                },
                [".html"] = new List<CodeSnippetExtrator>
                {
                    new CodeSnippetExtrator(HtmlCodeSnippetCommentStartLineTemplate, HtmlCodeSnippetCommentEndLineTemplate)
                },
                ["html"] = new List<CodeSnippetExtrator>
                {
                    new CodeSnippetExtrator(HtmlCodeSnippetCommentStartLineTemplate, HtmlCodeSnippetCommentEndLineTemplate)
                },
                [".xaml"] = new List<CodeSnippetExtrator>
                {
                    new CodeSnippetExtrator(XamlCodeSnippetCommentStartLineTemplate, XamlCodeSnippetCommentEndLineTemplate)
                },
                [".sql"] = new List<CodeSnippetExtrator>
                {
                    new CodeSnippetExtrator(SqlCodeSnippetCommentStartLineTemplate, SqlCodeSnippetCommentEndLineTemplate)
                },
                ["sql"] = new List<CodeSnippetExtrator>
                {
                    new CodeSnippetExtrator(SqlCodeSnippetCommentStartLineTemplate, SqlCodeSnippetCommentEndLineTemplate)
                },
                [".js"] = new List<CodeSnippetExtrator>
                {
                    new CodeSnippetExtrator(JavaScriptSnippetCommentStartLineTemplate, JavaScriptSnippetCommentEndLineTemplate)
                },
                ["js"] = new List<CodeSnippetExtrator>
                {
                    new CodeSnippetExtrator(JavaScriptSnippetCommentStartLineTemplate, JavaScriptSnippetCommentEndLineTemplate)
                },
                ["javascript"] = new List<CodeSnippetExtrator>
                {
                    new CodeSnippetExtrator(JavaScriptSnippetCommentStartLineTemplate, JavaScriptSnippetCommentEndLineTemplate)
                },
                [".java"] = new List<CodeSnippetExtrator>
                {
                    new CodeSnippetExtrator(JavaCodeSnippetCommentStartLineTemplate, JavaCodeSnippetCommentEndLineTemplate)
                },
                ["java"] = new List<CodeSnippetExtrator>
                {
                    new CodeSnippetExtrator(JavaCodeSnippetCommentStartLineTemplate, JavaCodeSnippetCommentEndLineTemplate)
                },
                [".py"] = new List<CodeSnippetExtrator>
                {
                    new CodeSnippetExtrator(PythonCodeSnippetCommentStartLineTemplate, PythonCodeSnippetCommentEndLineTemplate)
                },
                ["python"] = new List<CodeSnippetExtrator>
                {
                    new CodeSnippetExtrator(PythonCodeSnippetCommentStartLineTemplate, PythonCodeSnippetCommentEndLineTemplate)
                }
            };

        public HtmlCodeSnippetRenderer(MarkdownContext context)
        {
            _context = context;
        }

        protected override void Write(HtmlRenderer renderer, CodeSnippet obj)
        {
            if (renderer.EnableHtmlForInline)
            {
                renderer.Write("<pre><code").Write(obj.ToAttributeString()).WriteAttributes(obj).Write(">");
            }

            renderer.WriteEscape(GetContent(obj));

            if (renderer.EnableHtmlForInline)
            {
                renderer.Write("</code></pre>");
            }
        }

        private string GetContent(CodeSnippet obj)
        {
            var currentFilePath = ((RelativePath)_context.FilePath).GetPathFromWorkingFolder();
            var refFileRelativePath = ((RelativePath)obj.CodePath).BasedOn(currentFilePath);
            _context.ReportDependency(refFileRelativePath);

            var refPath = Path.Combine(_context.BasePath, refFileRelativePath.RemoveWorkingFolder());
            var allLines = File.ReadAllLines(refPath);
            var allCodeRanges = obj.CodeRanges ?? new List<CodeRange>();
            HashSet<int> tagLines = new HashSet<int>();

            if (!string.IsNullOrEmpty(obj.TagName))
            {
                var lang = obj.Language ?? Path.GetExtension(refPath);
                List<CodeSnippetExtrator> extrators;
                if(!CodeLanguageExtractors.TryGetValue(lang, out extrators))
                {
                    Logger.LogError($"{lang} is not supported languaging name, alias or extension for parsing code snippet with tag name, you can use line numbers instead");
                }

                if(extrators != null)
                {
                    var tagWithPrefix = tagPrefix + obj.TagName;
                    foreach(var extrator in extrators)
                    {
                        var tagToCoderangeMapping = extrator.GetAllTags(allLines, ref tagLines);
                        CodeRange cr;
                        if (tagToCoderangeMapping.TryGetValue(obj.TagName, out cr)
                            || tagToCoderangeMapping.TryGetValue(tagWithPrefix, out cr))
                        {
                            allCodeRanges.Add(cr);
                            break;
                        }
                    }
                }
            }

            var showCode = new StringBuilder();
            List<string> codeLines = new List<string>();
            int commonIndent = int.MaxValue;

            for (int lineNumber = 0; lineNumber < allLines.Length; lineNumber++)
            {
                if (!tagLines.Contains(lineNumber) && IsLineInRange(lineNumber + 1, allCodeRanges))
                {
                    int indentSpaces = 0;
                    string rawCodeLine = CountAndReplaceIndentSpaces(allLines[lineNumber], out indentSpaces);
                    commonIndent = Math.Min(commonIndent, indentSpaces);
                    codeLines.Add(rawCodeLine);
                }
            }

            int dedent = obj.DedentLength == null ? commonIndent : Math.Min(commonIndent, (int)obj.DedentLength);
            foreach (var rawCodeLine in codeLines)
            {
                string dedentedLine = rawCodeLine.Substring(dedent);
                showCode.Append($"{dedentedLine}\n");
            }

            return showCode.ToString();
        }

        private string CountAndReplaceIndentSpaces(string line, out int count)
        {
            StringBuilder sb = new StringBuilder();
            count = 0;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (c == ' ')
                {
                    sb.Append(' ');
                    count++;
                }
                else if (c == '\t')
                {
                    int newCount = CharHelper.AddTab(count);
                    sb.Append(' ', newCount - count);
                    count = newCount;

                }
                else
                {
                    sb.Append(line, i, line.Length - i);
                    break;
                }
            }

            return sb.ToString();
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
