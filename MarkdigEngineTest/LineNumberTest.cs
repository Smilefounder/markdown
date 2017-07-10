﻿using MarkdigEngine;
using Microsoft.DocAsCode.Plugins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MarkdigEngineTest
{
    public class LineNumberTest
    {
        [Fact]
        [Trait("Related", "LineNumber")]
        public void LineNumberTest_General()
        {
            // prepare
            string content = @"
# a simple test for line number
- list member 1
- list member 2
***
[Two Line Link](
http://spec.commonmark.org/0.27/)";

            // act
            var parameter = new MarkdownServiceParameters
            {
                BasePath = ".",
                Extensions = new Dictionary<string, object>
                {
                    { LineNumberExtension.EnableSourceInfo, true }
                }
            };
            var service = new MarkdigMarkdownService(parameter);
            var marked = service.Markup(content, "Topic.md");

            // assert
            var expected = @"<h1 id=""a-simple-test-for-line-number"" sourceFile=""Topic.md"" sourceStartLineNumber=""2"" sourceEndLineNumber=""2"">a simple test for line number</h1>
<ul sourceFile=""Topic.md"" sourceStartLineNumber=""3"" sourceEndLineNumber=""4"">
<li sourceFile=""Topic.md"" sourceStartLineNumber=""3"" sourceEndLineNumber=""3"">list member 1</li>
<li sourceFile=""Topic.md"" sourceStartLineNumber=""4"" sourceEndLineNumber=""4"">list member 2</li>
</ul>
<hr sourceFile=""Topic.md"" sourceStartLineNumber=""5"" sourceEndLineNumber=""5"" />
<p sourceFile=""Topic.md"" sourceStartLineNumber=""6"" sourceEndLineNumber=""7""><a href=""http://spec.commonmark.org/0.27/"" sourceFile=""Topic.md"" sourceStartLineNumber=""6"" sourceEndLineNumber=""7"">Two Line Link</a></p>
";
            Assert.Equal(expected.Replace("\r\n", "\n"), marked.Html);
        }

        [Fact]
        [Trait("Related", "LineNumber")]
        public void LineNumberTest_CodeSnippet()
        {
            //arange
            var content = @"// <tag>
line1
// </tag>";
            File.WriteAllText("Program.cs", content.Replace("\r\n", "\n"));

            // act
            var parameter = new MarkdownServiceParameters
            {
                BasePath = ".",
                Extensions = new Dictionary<string, object>
                {
                    { LineNumberExtension.EnableSourceInfo, true }
                }
            };
            var service = new MarkdigMarkdownService(parameter);
            var marked = service.Markup("[!code[tag-test](Program.cs#Tag)]", "Topic.md");

            // assert
            var expected = @"<pre><code name=""tag-test"" sourceFile=""Topic.md"" sourceStartLineNumber=""1"" sourceEndLineNumber=""1"">line1
</code></pre>";
            Assert.Equal(expected.Replace("\r\n", "\n"), marked.Html);
        }

        [Fact]
        [Trait("Related", "LineNumber")]
        public void LineNumberTest_Inclusion()
        {
            var root = @"
# Root content
This is inline [!include[ref-inline](a.md)] inclusion
[!include[ref-block](b.md)]";

            var refa = @"[inline](
http://spec.commonmark.org/0.27/)";

            var refb = @"[block](
http://spec.commonmark.org/0.27/)";
            File.WriteAllText("r/root.md", root);
            File.WriteAllText("r/a.md", refa);
            File.WriteAllText("r/b.md", refb);

            var parameter = new MarkdownServiceParameters
            {
                BasePath = ".",
                Extensions = new Dictionary<string, object>
                {
                    { LineNumberExtension.EnableSourceInfo, true }
                }
            };
            var service = new MarkdigMarkdownService(parameter);

            var result = service.Markup(root, "r/root.md");
            var expected = @"<h1 id=""root-content"" sourceFile=""r/root.md"" sourceStartLineNumber=""2"" sourceEndLineNumber=""2"">Root content</h1>
<p sourceFile=""r/root.md"" sourceStartLineNumber=""3"" sourceEndLineNumber=""3"">This is inline <a href=""http://spec.commonmark.org/0.27/"" sourceFile=""r/a.md"" sourceStartLineNumber=""1"" sourceEndLineNumber=""2"">inline</a> inclusion</p>
<p sourceFile=""r/b.md"" sourceStartLineNumber=""1"" sourceEndLineNumber=""2""><a href=""http://spec.commonmark.org/0.27/"" sourceFile=""r/b.md"" sourceStartLineNumber=""1"" sourceEndLineNumber=""2"">block</a></p>
";
            Assert.Equal(expected.Replace("\r\n", "\n"), result.Html);
        }
    }
}