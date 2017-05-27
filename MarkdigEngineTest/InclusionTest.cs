using System.IO;
using System.Collections.Generic;
using System.Collections.Immutable;

using Microsoft.DocAsCode.Plugins;
using Xunit;

namespace MarkdigEngine.Tests
{
    public class InclusionTest
    {
        [Fact]
        [Trait("Related", "Inclusion")]
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

            var parameter = new MarkdownServiceParameters { };
            var service = new MarkdigMarkdownService(parameter);

            var result = service.Markup(root, "r/root.md");
            var expected = @"<h1 id=""hello-world"">Hello World</h1>
<p>Test Include File</p>
<h1 id=""hello-include-file-a"">Hello Include File A</h1>
<p>This is a file A included by another file.</p>
<h1 id=""hello-include-file-b"">Hello Include File B</h1>
";
            Assert.Equal(expected.Replace("\r\n","\n"), result.Html);

            var dependency = result.Dependency;
            var expectedDependency = new List<string> { "~/r/a.md", "~/r/b.md" };
            Assert.Equal(expectedDependency.ToImmutableList(), dependency);
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

            var parameter = new MarkdownServiceParameters { };
            var service = new MarkdigMarkdownService(parameter);

            var result = service.Markup(root, "r/root.md");
            var expected = @"<h1 id=""hello-world"">Hello World</h1>
<p>Test Include File</p>
<h1 id=""hello-include-file-a"">Hello Include File A</h1>
<p>This is a file A included by another file.</p>
";
            Assert.Equal(expected.Replace("\r\n", "\n"), result.Html);

            var dependency = result.Dependency;
            var expectedDependency = new List<string> { "~/r/a(x).md" };
            Assert.Equal(expectedDependency.ToImmutableList(), dependency);
        }

        [Fact]
        [Trait("Related", "Inclusion")]
        public void TestBlockLevelInclusion_RelativePath()
        {
            var root = @"
# Hello World

Test Include File

[!include[refa](~/r/a.md)]

";

            var refa = @"
# Hello Include File A

This is a file A included by another file.
";

            WriteToFile("r/root.md", root);
            WriteToFile("r/a.md", refa);

            var parameter = new MarkdownServiceParameters { };
            var service = new MarkdigMarkdownService(parameter);

            var result = service.Markup(root, "r/root.md");
            var expected = @"<h1 id=""hello-world"">Hello World</h1>
<p>Test Include File</p>
<h1 id=""hello-include-file-a"">Hello Include File A</h1>
<p>This is a file A included by another file.</p>
";
            Assert.Equal(expected.Replace("\r\n", "\n"), result.Html);

            var dependency = result.Dependency;
            var expectedDependency = new List<string> { "~/r/a.md" };
            Assert.Equal(expectedDependency.ToImmutableList(), dependency);
        }

        [Fact]
        [Trait("Related", "Inclusion")]
        public void TestBlockLevelInclusion_CycleInclude()
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

[!include[refa](a.md)]
";
            WriteToFile("r/root.md", root);
            WriteToFile("r/a.md", refa);
            WriteToFile("r/b.md", refb);

            var parameter = new MarkdownServiceParameters { };
            var service = new MarkdigMarkdownService(parameter);

            var result = service.Markup(root, "r/root.md");
            var expected = @"<h1 id=""hello-world"">Hello World</h1>
<p>Test Include File</p>
<h1 id=""hello-include-file-a"">Hello Include File A</h1>
<p>This is a file A included by another file.</p>
<h1 id=""hello-include-file-b"">Hello Include File B</h1>
<h1 id=""hello-include-file-a"">Hello Include File A</h1>
<p>This is a file A included by another file.</p>
[!Include[refb](b.md)]";

            Assert.Equal<string>(expected.Replace("\r\n", "\n"), result.Html);
        }

        [Fact]
        [Trait("Related", "Inclusion")]
        public void TestInlineLevelInclusion_General()
        {
            var root = @"
# Hello World

Test Inline Included File: [!include[refa](~/r/a.md)].

";

            var refa = "This is a **included** token";

            WriteToFile("r/root.md", root);
            WriteToFile("r/a.md", refa);

            var parameter = new MarkdownServiceParameters { };
            var service = new MarkdigMarkdownService(parameter);

            var result = service.Markup(root, "r/root.md"); ;
            var expected = @"<h1 id=""hello-world"">Hello World</h1>
<p>Test Inline Included File: This is a <strong>included</strong> token.</p>
";
            Assert.Equal(expected.Replace("\r\n", "\n"), result.Html);

            var dependency = result.Dependency;
            var expectedDependency = new List<string> { "~/r/a.md" };
            Assert.Equal(expectedDependency.ToImmutableList(), dependency);
        }

        [Fact]
        [Trait("Related", "Inclusion")]
        public void TestInlineLevelInclusion_CycleInclude()
        {
            var root = @"
# Hello World

Test Inline Included File: [!include[refa](~/r/a.md)].

";

            var refa = "This is a **included** token with [!include[root](~/r/root.md)]";

            WriteToFile("r/root.md", root);
            WriteToFile("r/a.md", refa);

            var parameter = new MarkdownServiceParameters { };
            var service = new MarkdigMarkdownService(parameter);

            var result = service.Markup(root, "r/root.md");
            var expected = @"<h1 id=""hello-world"">Hello World</h1>
<p>Test Inline Included File: This is a <strong>included</strong> token with <h1 id=""hello-world"">Hello World</h1>
<p>Test Inline Included File: This is a <strong>included</strong> token with [!Include[root](~/r/root.md)].</p>
.</p>
";

            Assert.Equal<string>(expected.Replace("\r\n", "\n"), result.Html);
        }

        [Fact]
        [Trait("Related", "Inclusion")]
        public void TestInlineLevelInclusion_Block()
        {
            var root = @"
# Hello World

Test Inline Included File: [!include[refa](~/r/a.md)].

";

            var refa = @"## This is a included token

block content in Inline Inclusion.";

            WriteToFile("r/root.md", root);
            WriteToFile("r/a.md", refa);

            var parameter = new MarkdownServiceParameters { };
            var service = new MarkdigMarkdownService(parameter);

            var result = service.Markup(root, "r/root.md");
            var expected = @"<h1 id=""hello-world"">Hello World</h1>
<p>Test Inline Included File: <h2 id=""this-is-a-included-token"">This is a included token</h2>
<p>block content in Inline Inclusion.</p>
.</p>
";
            Assert.Equal(expected.Replace("\r\n", "\n"), result.Html);

            var dependency = result.Dependency;
            var expectedDependency = new List<string> { "~/r/a.md" };
            Assert.Equal(expectedDependency.ToImmutableList(), dependency);
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
