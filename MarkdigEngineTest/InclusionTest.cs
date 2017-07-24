﻿using System.IO;
using System.Collections.Generic;
using System.Collections.Immutable;

using Microsoft.DocAsCode.Plugins;
using Xunit;
using System.Linq;

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

            var parameter = new MarkdownServiceParameters
            {
                BasePath = "."
            };
            var service = new MarkdigMarkdownService(parameter);

            var result = service.Markup(root, "r/root.md");
            var expected = @"<h1 id=""hello-world"">Hello World</h1>
<p>Test Include File</p>
<h1 id=""hello-include-file-a"">Hello Include File A</h1>
<p>This is a file A included by another file.</p>
<h1 id=""hello-include-file-b"">Hello Include File B</h1>
";
            Assert.Equal(expected.Replace("\r\n", "\n"), result.Html);

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

            var parameter = new MarkdownServiceParameters
            {
                BasePath = "."
            };
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

            var parameter = new MarkdownServiceParameters
            {
                BasePath = "."
            };
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

            var parameter = new MarkdownServiceParameters
            {
                BasePath = "."
            };
            var service = new MarkdigMarkdownService(parameter);

            var result = service.Markup(root, "r/root.md");
            var expected = @"<h1 id=""hello-world"">Hello World</h1>
<p>Test Include File</p>
<h1 id=""hello-include-file-a"">Hello Include File A</h1>
<p>This is a file A included by another file.</p>
<h1 id=""hello-include-file-b"">Hello Include File B</h1>
<!-- BEGIN ERROR INCLUDE: Unable to resolve [!include[refa](a.md)]: Circular dependency found in &quot;r/b.md&quot; -->[!include[refa](a.md)]<!--END ERROR INCLUDE -->";

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

            var parameter = new MarkdownServiceParameters
            {
                BasePath = "."
            };
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

            var parameter = new MarkdownServiceParameters
            {
                BasePath = "."
            };
            var service = new MarkdigMarkdownService(parameter);

            var result = service.Markup(root, "r/root.md");
            var expected = @"<h1 id=""hello-world"">Hello World</h1>
<p>Test Inline Included File: This is a <strong>included</strong> token with <!-- BEGIN ERROR INCLUDE: Unable to resolve [!include[root](~/r/root.md)]: Circular dependency found in &quot;r/a.md&quot; -->[!include[root](~/r/root.md)]<!--END ERROR INCLUDE -->.</p>
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

            var parameter = new MarkdownServiceParameters
            {
                BasePath = "."
            };
            var service = new MarkdigMarkdownService(parameter);

            var result = service.Markup(root, "r/root.md");
            var expected = @"<h1 id=""hello-world"">Hello World</h1>
<p>Test Inline Included File: ## This is a included tokenblock content in Inline Inclusion..</p>
";
            Assert.Equal(expected.Replace("\r\n", "\n"), result.Html);

            var dependency = result.Dependency;
            var expectedDependency = new List<string> { "~/r/a.md" };
            Assert.Equal(expectedDependency.ToImmutableList(), dependency);
        }



		[Fact]
		[Trait("Related", "DfmMarkdown")]
		public void TestBlockLevelInclusion()
		{
			// -r
			//  |- root.md
			//  |- empty.md
			//  |- a
			//  |  |- refc.md
			//  |- b
			//  |  |- linkAndRefRoot.md
			//  |  |- a.md
			//  |  |- img
			//  |  |   |- img.jpg
			//  |- c
			//  |  |- c.md
			//  |- link
			//     |- link2.md
			//     |- md
			//         |- c.md
			var root = @"
[!include[linkAndRefRoot](b/linkAndRefRoot.md)]
[!include[refc](a/refc.md ""This is root"")]
[!include[refc_using_cache](a/refc.md)]
[!include[empty](empty.md)]
[!include[external](http://microsoft.com/a.md)]";

			var linkAndRefRoot = @"
Paragraph1
[link](a.md)
[!include-[link2](../link/link2.md)]
![Image](img/img.jpg)
[!include-[root](../root.md)]";
			var link2 = @"[link](md/c.md)";
			var refc = @"[!include[c](../c/c.md ""This is root"")]";
			var c = @"**Hello**";
			WriteToFile("r/root.md", root);

			WriteToFile("r/a/refc.md", refc);
			WriteToFile("r/b/linkAndRefRoot.md", linkAndRefRoot);
			WriteToFile("r/link/link2.md", link2);
			WriteToFile("r/c/c.md", c);
			WriteToFile("r/empty.md", string.Empty);
			var parameter = new MarkdownServiceParameters
			{
				BasePath = "."
			};
			var service = new MarkdigMarkdownService(parameter);
			var marked = service.Markup(root, "r/root.md");
			var dependency = marked.Dependency;
            var expected = @"<p>Paragraph1
<a href=""r/b/a.md"">link</a>
<a href=""r/link/md/c.md"">link</a>
<img src=""r/b/img/img.jpg"" alt=""Image"" />
<!-- BEGIN ERROR INCLUDE: Unable to resolve [!include[root](../root.md)]: Circular dependency found in &quot;r/b/linkAndRefRoot.md&quot; -->[!include[root](../root.md)]<!--END ERROR INCLUDE --></p>
<p><strong>Hello</strong></p>
<p><strong>Hello</strong></p>
<!-- BEGIN ERROR INCLUDE: Unable to resolve [!include[external](http://microsoft.com/a.md)]: Absolute path &quot;http://microsoft.com/a.md&quot; is not supported. -->[!include[external](http://microsoft.com/a.md)]<!--END ERROR INCLUDE -->".Replace("\r\n", "\n");

            Assert.Equal(expected, marked.Html);
			Assert.Equal(
				new[]
				{
					"~/r/a/refc.md",
                    "~/r/b/linkAndRefRoot.md",
                    "~/r/c/c.md",
                    "~/r/empty.md",
                    "~/r/link/link2.md",
				},
				dependency.OrderBy(x => x).ToArray());
		}

		[Fact]
		[Trait("Related", "DfmMarkdown")]
		public void TestBlockLevelInclusionWithSameFile()
		{
			// -r
			//  |- r.md
			//  |- a
			//  |  |- a.md
			//  |- b
			//  |  |- token.md
			//  |- c
			//     |- d
			//        |- d.md
			//  |- img
			//  |  |- img.jpg
			var r = @"
[!include[](a/a.md)]
[!include[](c/d/d.md)]
";
			var a = @"
[!include[](../b/token.md)]";
			var token = @"
![](../img/img.jpg)
[](#anchor)
[a](../a/a.md)
[](invalid.md)
[d](../c/d/d.md#anchor)
";
			var d = @"
[!include[](../../b/token.md)]";
			WriteToFile("r/r.md", r);
			WriteToFile("r/a/a.md", a);
			WriteToFile("r/b/token.md", token);
			WriteToFile("r/c/d/d.md", d);
			var parameter = new MarkdownServiceParameters
			{
				BasePath = "."
			};
			var service = new MarkdigMarkdownService(parameter);
			var marked = service.Markup(a, "r/a/a.md");
			var expected = @"<p><img src=""r/img/img.jpg"" alt="""" />
<a href=""#anchor""></a>
<a href=""r/a/a.md"">a</a>
<a href=""r/b/invalid.md""></a>
<a href=""r/c/d/d.md#anchor"">d</a></p>".Replace("\r\n", "\n") + "\n";
			var dependency = marked.Dependency;
			Assert.Equal(expected, marked.Html);
			Assert.Equal(
				new[] { "~/r/b/token.md" },
				dependency.OrderBy(x => x).ToArray());

			marked = service.Markup(d, "r/c/d/d.md");
			dependency = marked.Dependency;
			Assert.Equal(expected, marked.Html);
			Assert.Equal(
				new[] { "~/r/b/token.md" },
				dependency.OrderBy(x => x).ToArray());

			dependency.Clear();
			marked = service.Markup(r, "r/r.md");
			dependency = marked.Dependency;
			Assert.Equal($@"{expected}{expected}", marked.Html);
			Assert.Equal(
				new[] { "~/r/a/a.md", "~/r/b/token.md", "~/r/c/d/d.md" },
				dependency.OrderBy(x => x).ToArray());
		}

		[Fact]
		[Trait("Related", "DfmMarkdown")]
		public void TestBlockLevelInclusionWithWorkingFolder()
		{
			// -r
			//  |- root.md
			//  |- b
			//  |  |- linkAndRefRoot.md
			var root = @"[!include[linkAndRefRoot](~/r/b/linkAndRefRoot.md)]";
			var linkAndRefRoot = @"Paragraph1";
			WriteToFile("r/root.md", root);
			WriteToFile("r/b/linkAndRefRoot.md", linkAndRefRoot);
			var parameter = new MarkdownServiceParameters
			{
				BasePath = "."
			};
			var service = new MarkdigMarkdownService(parameter);
			var marked = service.Markup(root, "r/root.md");
			var expected = @"<p>Paragraph1</p>" + "\n";
			Assert.Equal(expected, marked.Html);
		}

		#region Fallback folders testing

		//[Fact]
		[Trait("Related", "DfmMarkdown")]
		public void TestFallback_Inclusion_random_name()
		{
			// -root_folder (this is also docset folder)
			//  |- root.md
			//  |- a_folder
			//  |  |- a.md
			//  |- token_folder
			//  |  |- token1.md
			// -fallback_folder
			//  |- token_folder
			//     |- token2.md

			// 1. Prepare data
			var uniqueFolderName = Path.GetRandomFileName();
			var root = $@"1markdown root.md main content start.

[!include[a](a_folder_{uniqueFolderName}/a_{uniqueFolderName}.md ""This is a.md"")]

markdown root.md main content end.";

			var a = $@"1markdown a.md main content start.

[!include[token1](../token_folder_{uniqueFolderName}/token1_{uniqueFolderName}.md ""This is token1.md"")]
[!include[token1](../token_folder_{uniqueFolderName}/token2_{uniqueFolderName}.md ""This is token2.md"")]

markdown a.md main content end.";

			var token1 = $@"1markdown token1.md content start.

[!include[token2](token2_{uniqueFolderName}.md ""This is token2.md"")]

markdown token1.md content end.";

			var token2 = @"**1markdown token2.md main content**";

			WriteToFile($"{uniqueFolderName}/root_folder_{uniqueFolderName}/root_{uniqueFolderName}.md", root);
			WriteToFile($"{uniqueFolderName}/root_folder_{uniqueFolderName}/a_folder_{uniqueFolderName}/a_{uniqueFolderName}.md", a);
			WriteToFile($"{uniqueFolderName}/root_folder_{uniqueFolderName}/token_folder_{uniqueFolderName}/token1_{uniqueFolderName}.md", token1);
			WriteToFile($"{uniqueFolderName}/fallback_folder_{uniqueFolderName}/token_folder_{uniqueFolderName}/token2_{uniqueFolderName}.md", token2);

			var fallbackFolders = new List<string> { { Path.Combine(Directory.GetCurrentDirectory(), $"{uniqueFolderName}/fallback_folder_{uniqueFolderName}") } };
			var parameter = new MarkdownServiceParameters
			{
				BasePath = "."
			};
			var service = new MarkdigMarkdownService(parameter);
			//var marked = service.Markup(Path.Combine(Directory.GetCurrentDirectory(), $"{uniqueFolderName}/root_folder_{uniqueFolderName}"), root, fallbackFolders, $"root_{uniqueFolderName}.md");
			var marked = service.Markup("place", "holder");
			var dependency = marked.Dependency;

			Assert.Equal($@"<p>1markdown root.md main content start.</p>
<p>1markdown a.md main content start.</p>
<p>1markdown token1.md content start.</p>
<p><strong>1markdown token2.md main content</strong></p>
<p>markdown token1.md content end.</p>
<p><strong>1markdown token2.md main content</strong></p>
<p>markdown a.md main content end.</p>
<p>markdown root.md main content end.</p>
".Replace("\r\n", "\n"), marked.Html);
			Assert.Equal(
				new[] { $"../fallback_folder_{uniqueFolderName}/token_folder_{uniqueFolderName}/token2_{uniqueFolderName}.md", $"a_folder_{uniqueFolderName}/a_{uniqueFolderName}.md", $"token_folder_{uniqueFolderName}/token1_{uniqueFolderName}.md", $"token_folder_{uniqueFolderName}/token2_{uniqueFolderName}.md" },
				dependency.OrderBy(x => x).ToArray());
		}

		//[Fact]
		[Trait("Related", "DfmMarkdown")]
		public void TestFallback_InclusionWithCodeFences()
		{
			// -root_folder (this is also docset folder)
			//  |- root.md
			//  |- a_folder
			//     |- a.md
			//  |- code_folder
			//     |- sample1.cs
			// -fallback_folder
			//  |- a_folder
			//     |- code_in_a.cs
			//  |- code_folder
			//     |- sample2.cs

			// 1. Prepare data
			var root = @"markdown root.md main content start.

mardown a content in root.md content start

[!include[a](a_folder/a.md ""This is a.md"")]

mardown a content in root.md content end

sample 1 code in root.md content start

[!CODE-cs[this is sample 1 code](code_folder/sample1.cs)]

sample 1 code in root.md content end

sample 2 code in root.md content start

[!CODE-cs[this is sample 2 code](code_folder/sample2.cs)]

sample 2 code in root.md content end

markdown root.md main content end.";

			var a = @"markdown a.md main content start.

code_in_a code in a.md content start

[!CODE-cs[this is code_in_a code](code_in_a.cs)]

code_in_a in a.md content end

markdown a.md a.md content end.";

			var code_in_a = @"namespace code_in_a{}";

			var sample1 = @"namespace sample1{}";

			var sample2 = @"namespace sample2{}";

			var uniqueFolderName = Path.GetRandomFileName();
			WriteToFile($"{uniqueFolderName}/root_folder/root.md", root);
			WriteToFile($"{uniqueFolderName}/root_folder/a_folder/a.md", a);
			WriteToFile($"{uniqueFolderName}/root_folder/code_folder/sample1.cs", sample1);
			WriteToFile($"{uniqueFolderName}/fallback_folder/a_folder/code_in_a.cs", code_in_a);
			WriteToFile($"{uniqueFolderName}/fallback_folder/code_folder/sample2.cs", sample2);

			var fallbackFolders = new List<string> { { Path.Combine(Directory.GetCurrentDirectory(), $"{uniqueFolderName}/fallback_folder") } };

			// Verify root.md markup result
			var parameter = new MarkdownServiceParameters
			{
				BasePath = "."
			};
			var service = new MarkdigMarkdownService(parameter);
			//var rootMarked = service.Markup(Path.Combine(Directory.GetCurrentDirectory(), $"{uniqueFolderName}/root_folder"), root, fallbackFolders, "root.md");
			var rootMarked = service.Markup("place", "holder");
			var rootDependency = rootMarked.Dependency;
			Assert.Equal(@"<p>markdown root.md main content start.</p>
<p>mardown a content in root.md content start</p>
<p>markdown a.md main content start.</p>
<p>code_in_a code in a.md content start</p>
<pre><code class=""lang-cs"" name=""this is code_in_a code"">namespace code_in_a{}
</code></pre><p>code_in_a in a.md content end</p>
<p>markdown a.md a.md content end.</p>
<p>mardown a content in root.md content end</p>
<p>sample 1 code in root.md content start</p>
<pre><code class=""lang-cs"" name=""this is sample 1 code"">namespace sample1{}
</code></pre><p>sample 1 code in root.md content end</p>
<p>sample 2 code in root.md content start</p>
<pre><code class=""lang-cs"" name=""this is sample 2 code"">namespace sample2{}
</code></pre><p>sample 2 code in root.md content end</p>
<p>markdown root.md main content end.</p>
".Replace("\r\n", "\n"), rootMarked.Html);
			Assert.Equal(
				new[] { "../fallback_folder/a_folder/code_in_a.cs", "../fallback_folder/code_folder/sample2.cs", "a_folder/a.md", "a_folder/code_in_a.cs", "code_folder/sample1.cs", "code_folder/sample2.cs" },
				rootDependency.OrderBy(x => x).ToArray());

			// Verify a.md markup result
			//var aMarked = service.Markup(Path.Combine(Directory.GetCurrentDirectory(), $"{uniqueFolderName}/root_folder"), a, fallbackFolders, "a_folder/a.md");
			var aMarked = service.Markup("place", "holder");
			var aDependency = aMarked.Dependency;
			Assert.Equal(@"<p>markdown a.md main content start.</p>
<p>code_in_a code in a.md content start</p>
<pre><code class=""lang-cs"" name=""this is code_in_a code"">namespace code_in_a{}
</code></pre><p>code_in_a in a.md content end</p>
<p>markdown a.md a.md content end.</p>
".Replace("\r\n", "\n"), aMarked.Html);
			Assert.Equal(
				new[] { "../../fallback_folder/a_folder/code_in_a.cs", "code_in_a.cs" },
				aDependency.OrderBy(x => x).ToArray());
		}

		#endregion

		[Fact]
		[Trait("Related", "DfmMarkdown")]
		public void TestInclusion_InlineLevel()
		{
			// 1. Prepare data
			var root = @"
Inline [!include[ref1](ref1.md ""This is root"")]
Inline [!include[ref3](ref3.md ""This is root"")]
";

			var ref1 = @"[!include[ref2](ref2.md ""This is root"")]";
			var ref2 = @"## Inline inclusion do not parse header [!include[root](root.md ""This is root"")]";
			var ref3 = @"**Hello**  ";
			File.WriteAllText("root.md", root);
			File.WriteAllText("ref1.md", ref1);
			File.WriteAllText("ref2.md", ref2);
			File.WriteAllText("ref3.md", ref3);

			var parameter = new MarkdownServiceParameters
			{
				BasePath = "."
			};
			var service = new MarkdigMarkdownService(parameter);
			var marked = service.Markup(root, "root.md");
			var dependency = marked.Dependency;
            var expected = "<p>Inline ## Inline inclusion do not parse header <!-- BEGIN ERROR INCLUDE: Unable to resolve [!include[root](root.md)]: Circular dependency found in &quot;ref2.md&quot; -->[!include[root](root.md)]<!--END ERROR INCLUDE -->\nInline <strong>Hello</strong></p>\n";

            Assert.Equal(expected, marked.Html);
			Assert.Equal(
				new[] { "~/ref1.md", "~/ref2.md", "~/ref3.md" },
				dependency.OrderBy(x => x).ToArray());
		}

		[Fact]
		[Trait("Related", "DfmMarkdown")]
		public void TestBlockInclude_ShouldExcludeBracketInRegex()
		{
			// 1. Prepare data
			var root = @"[!INCLUDE [azure-probe-intro-include](inc1.md)].

[!INCLUDE [azure-arm-classic-important-include](inc2.md)] [Resource Manager model](inc1.md).


[!INCLUDE [azure-ps-prerequisites-include.md](inc3.md)]";

			var expected = @"<p>inc1.</p>
<p>inc2 <a href=""inc1.md"">Resource Manager model</a>.</p>
<p>inc3</p>
";

			var inc1 = @"inc1";
			var inc2 = @"inc2";
			var inc3 = @"inc3";
			File.WriteAllText("root.md", root);
			File.WriteAllText("inc1.md", inc1);
			File.WriteAllText("inc2.md", inc2);
			File.WriteAllText("inc3.md", inc3);

			var parameter = new MarkdownServiceParameters
			{
				BasePath = "."
			};
			var service = new MarkdigMarkdownService(parameter);
			var marked = service.Markup(root, "root.md");
			var dependency = marked.Dependency;
			Assert.Equal(expected.Replace("\r\n", "\n"), marked.Html);
			Assert.Equal(
			  new[] { "~/inc1.md", "~/inc2.md", "~/inc3.md" },
			  dependency.OrderBy(x => x).ToArray());
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
