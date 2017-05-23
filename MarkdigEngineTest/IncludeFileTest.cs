﻿using Xunit;
using System.IO;

namespace MarkdigEngine.Tests
{
    public class IncludeFileTest
    {
        [Fact]
        [Trait("Related", "IncludeFile")]
        public void TestBlockLevelInclusion_General()
        {
            var root = @"
# Hello World

Test Include File

[!include[refa](a.md)]

";

            var refa = @"
# Hello Include File A

This is a file A included by another file.

[!include[refb](b.md)]

";

            var refb = @"
# Hello Include File B
";
            WriteToFile("r/root.md", root);
            WriteToFile("r/a.md", refa);
            WriteToFile("r/b.md", refb);

            var context = new MarkdownContext
            {
                FilePath = "r/root.md"
            };
            var marked = MarkdigMarked.Markup(root, context);
            var expected = @"<h1 id=""hello-world"">Hello World</h1>
<p>Test Include File</p>
<h1 id=""hello-include-file-a"">Hello Include File A</h1>
<p>This is a file A included by another file.</p>
<h1 id=""hello-include-file-b"">Hello Include File B</h1>
";
            Assert.Equal(expected.Replace("\r\n","\n"), marked);
        }

        [Fact]
        [Trait("Related", "IncludeFile")]
        public void TestBlockLevelInclusion_Esacape()
        {
            var root = @"
# Hello World

Test Include File

[!include[refa](a\(x\).md)]

";

            var refa = @"
# Hello Include File A

This is a file A included by another file.
";

            WriteToFile("r/root.md", root);
            WriteToFile("r/a(x).md", refa);

            var context = new MarkdownContext
            {
                FilePath = "r/root.md"
            };
            var marked = MarkdigMarked.Markup(root, context);
            var expected = @"<h1 id=""hello-world"">Hello World</h1>
<p>Test Include File</p>
<h1 id=""hello-include-file-a"">Hello Include File A</h1>
<p>This is a file A included by another file.</p>
";
            Assert.Equal(expected.Replace("\r\n", "\n"), marked);
        }

        private static void WriteToFile(string file, string content)
        {
            var dir = Path.GetDirectoryName(file);
            if (!string.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }
            File.WriteAllText(file, content);
        }
    }
}
