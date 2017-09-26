using System.IO;

using MarkdigEngine;
using Microsoft.DocAsCode.Plugins;
using Xunit;

namespace MarkdigEngineTest
{
    public class GeneralTest
    {
        private static MarkupResult SimpleMarkup(string source)
        {
            var parameter = new MarkdownServiceParameters
            {
                BasePath = "."
            };
            var service = new MarkdigMarkdownService(parameter);
            return service.Markup(source, "Topic.md");
        }

        [Fact]
        [Trait("Related", "DfmMarkdown")]
        public void TestDfm_EncodeInStrongEM()
        {
            var source = @"tag started with non-alphabet should be encoded <1-100>, <_hello>, <?world>, <1_2 href=""good"">, <1 att='bcd'>.
tag started with alphabet should not be encode: <abc> <a-hello> <a?world> <a_b href=""good""> <AC att='bcd'>";

            var expected = @"<p>tag started with non-alphabet should be encoded &lt;1-100&gt;, &lt;_hello&gt;, &lt;?world&gt;, &lt;1_2 href=&quot;good&quot;&gt;, &lt;1 att='bcd'&gt;.
tag started with alphabet should not be encode: <abc> <a-hello> &lt;a?world&gt; &lt;a_b href=&quot;good&quot;&gt; <AC att='bcd'></p>
";
            var marked = SimpleMarkup(source);
            Assert.Equal(expected.Replace("\r\n", "\n"), marked.Html);
        }

        [Fact]
        [Trait("Related", "DfmMarkdown")]
        public void TestDfmImageLink_WithSpecialCharactorsInAltText()
        {
            var source = @"![This is image alt text with quotation ' and double quotation ""hello"" world](girl.png)";

            var expected = @"<p><img src=""girl.png"" alt=""This is image alt text with quotation ' and double quotation &quot;hello&quot; world"" /></p>
";
            var marked = SimpleMarkup(source);
            Assert.Equal(expected.Replace("\r\n", "\n"), marked.Html);
        }

        [Theory]
        [Trait("Related", "DfmMarkdown")]
        #region Inline Data
        [InlineData("", "")]
        [InlineData("<address@example.com>", "<p><a href=\"mailto:address@example.com\">address@example.com</a></p>\n")]
        [InlineData(" https://github.com/dotnet/docfx/releases ", "<p><a href=\"https://github.com/dotnet/docfx/releases\">https://github.com/dotnet/docfx/releases</a></p>\n")]
        [InlineData("<http://example.com/>", "<p><a href=\"http://example.com/\">http://example.com/</a></p>\n")]
        [InlineData("# Hello World", "<h1 id=\"hello-world\">Hello World</h1>\n")]
        [InlineData("Hot keys: <kbd>Ctrl+[</kbd> and <kbd>Ctrl+]</kbd>", "<p>Hot keys: <kbd>Ctrl+[</kbd> and <kbd>Ctrl+]</kbd></p>\n")]
        [InlineData("<div>Some text here</div>", "<div>Some text here</div>\n")]
        [InlineData(@"---
a: b
b:
  c: e
---", "<yamlheader start=\"1\" end=\"5\">a: b\nb:\n  c: e</yamlheader>")]
        [InlineData(@"# Hello @CrossLink1 @'CrossLink2'dummy 
@World",
    "<h1 id=\"hello-crosslink1-crosslink2dummy\">Hello <xref href=\"CrossLink1\" data-throw-if-not-resolved=\"False\"></xref> <xref href=\"CrossLink2\" data-throw-if-not-resolved=\"False\"></xref>dummy</h1>\n<p><xref href=\"World\" data-throw-if-not-resolved=\"False\"></xref></p>\n")]
        [InlineData("a\n```\nc\n```",
    "<p>a</p>\n<pre><code>c\n</code></pre>\n")]
        [InlineData(@" *hello* abc @api__1",
    "<p><em>hello</em> abc <xref href=\"api__1\" data-throw-if-not-resolved=\"False\"></xref></p>\n")]
        [InlineData("@1abc", "<p>@1abc</p>\n")]
        [InlineData(@"@api1 @api__1 @api!1 @api@a <abc@api.com> <a.b.c@api.com> @'a p ';@""a!pi"",@api...@api",
    "<p><xref href=\"api1\" data-throw-if-not-resolved=\"False\"></xref> <xref href=\"api__1\" data-throw-if-not-resolved=\"False\"></xref> <xref href=\"api!1\" data-throw-if-not-resolved=\"False\"></xref> <xref href=\"api@a\" data-throw-if-not-resolved=\"False\"></xref> <a href=\"mailto:abc@api.com\">abc@api.com</a> <a href=\"mailto:a.b.c@api.com\">a.b.c@api.com</a> <xref href=\"a p\" data-throw-if-not-resolved=\"False\"></xref>;<xref href=\"a!pi\" data-throw-if-not-resolved=\"False\"></xref>,<xref href=\"api\" data-throw-if-not-resolved=\"False\"></xref>...<xref href=\"api\" data-throw-if-not-resolved=\"False\"></xref></p>\n")]
        [InlineData("[name](xref:uid \"title\")", "<p><a href=\"xref:uid\" title=\"title\">name</a></p>\n")]
        [InlineData("<xref:uid>text", "<p><xref href=\"uid\" data-throw-if-not-resolved=\"True\"></xref>text</p>\n")]
        [InlineData("<xref:'uid with space'>text", "<p><xref href=\"uid with space\" data-throw-if-not-resolved=\"True\"></xref>text</p>\n")]
        [InlineData(
    @"[*a*](xref:uid)",
    "<p><a href=\"xref:uid\"><em>a</em></a></p>\n")]
        [InlineData(
    @"# <a id=""x""></a>Y",
    @"<h1 id=""x"">Y</h1>
")]
        [InlineData(
    @"# <a name=""x""></a>Y",
    @"<h1 id=""x"">Y</h1>
")]
        #endregion
        public void TestDfmInGeneral(string source, string expected)
        {
            var result = SimpleMarkup(source).Html;
            Assert.Equal(expected.Replace("\r\n", "\n"), result);
        }

        [Fact]
        [Trait("Related", "DfmMarkdown")]
        [Trait("A wrong case need to be fixed in dfm", "' in title should be traslated to &#39; instead of &amp;#39;")]
        public void TestDfmLink_LinkWithSpecialCharactorsInTitle()
        {
            var source = @"[text's string](https://www.google.com.sg/?gfe_rd=cr&ei=Xk ""Google's homepage"")";
            var expected = @"<p><a href=""https://www.google.com.sg/?gfe_rd=cr&amp;ei=Xk"" title=""Google's homepage"">text's string</a></p>
";
            var marked = SimpleMarkup(source);
            Assert.Equal(expected.Replace("\r\n", "\n"), marked.Html);
        }

        [Fact]
        [Trait("Related", "DfmMarkdown")]
        public void TestDfmLink_WithSpecialCharactorsInTitle()
        {
            var source = @"[This is link text with quotation ' and double quotation ""hello"" world](girl.png ""title is \""hello\"" world."")";

            var expected = @"<p><a href=""girl.png"" title=""title is &quot;hello&quot; world."">This is link text with quotation ' and double quotation &quot;hello&quot; world</a></p>
";
            var marked = SimpleMarkup(source);
            Assert.Equal(expected.Replace("\r\n", "\n"), marked.Html);
        }

        [Fact]
        [Trait("Related", "DfmMarkdown")]
        public void TestDfmTagValidate()
        {
            var result = SimpleMarkup(@"<div><i>x</i><EM>y</EM><h1>z<pre><code>a*b*c</code></pre></h1></div>

<script>alert(1);</script>");

            Assert.Equal(@"<div><i>x</i><EM>y</EM><h1>z<pre><code>a*b*c</code></pre></h1></div>
<script>alert(1);</script>
".Replace("\r\n", "\n"), result.Html);
        }

        [Fact]
        [Trait("Related", "DfmMarkdown")]
        public void TestPathUtility_AbsoluteLinkWithBracketAndBrackt()
        {
            var source = @"[User-Defined Date/Time Formats (Format Function)](http://msdn2.microsoft.com/library/73ctwf33\(VS.90\).aspx)";
            var expected = @"<p><a href=""http://msdn2.microsoft.com/library/73ctwf33(VS.90).aspx"">User-Defined Date/Time Formats (Format Function)</a></p>
";
            var marked = SimpleMarkup(source);
            Assert.Equal(expected.Replace("\r\n", "\n"), marked.Html);
        }

        [Fact]
        [Trait("Related", "DfmMarkdown")]
        public void TestYaml_InvalidYamlInsideContent()
        {
            var source = @"# Title
---
Not yaml syntax
---
hello world";
            var expected = @"<h1 id=""title"">Title</h1>
<hr />
<h2 id=""not-yaml-syntax"">Not yaml syntax</h2>
<p>hello world</p>
";
            var marked = SimpleMarkup(source);
            Assert.Equal(expected.Replace("\r\n", "\n"), marked.Html);
        }


        [Fact]
        [Trait("Related", "DfmMarkdown")]
        public void TestTabGroup()
        {
            //var options = DocfxFlavoredMarked.CreateDefaultOptions();
            //options.ShouldExportSourceInfo = true;
            //var actual = DocfxFlavoredMarked.Markup(null, null, options, @"# [title-a](#tab/a)
            string actual = @"# [title-a](#tab/a)
            content - a
# <a id=""x""></a>[title-b](#tab/b/c)
content-b
- - -";
            var groupId = "uBn0rykxXo";
            var expected = $@"<div class=""tabGroup"" id=""tabgroup_{groupId}"" sourceFile=""test.md"" sourceStartLineNumber=""1"" sourceEndLineNumber=""5"">
<ul role=""tablist"">
<li role=""presentation"">
<a href=""#tabpanel_{groupId}_a"" role=""tab"" aria-controls=""tabpanel_{groupId}_a"" data-tab=""a"" tabindex=""0"" aria-selected=""true"" sourceFile=""test.md"" sourceStartLineNumber=""1"" sourceEndLineNumber=""1"">title-a</a>
</li>
<li role=""presentation"" aria-hidden=""true"" hidden=""hidden"">
<a href=""#tabpanel_{groupId}_b_c"" role=""tab"" aria-controls=""tabpanel_{groupId}_b_c"" data-tab=""b"" data-condition=""c"" tabindex=""-1"" sourceFile=""test.md"" sourceStartLineNumber=""3"" sourceEndLineNumber=""3"">title-b</a>
</li>
</ul>
<section id=""tabpanel_{groupId}_a"" role=""tabpanel"" data-tab=""a"">
<p sourceFile=""test.md"" sourceStartLineNumber=""2"" sourceEndLineNumber=""2"">content-a</p>
</section>
<section id=""tabpanel_{groupId}_b_c"" role=""tabpanel"" data-tab=""b"" data-condition=""c"" aria-hidden=""true"" hidden=""hidden"">
<p sourceFile=""test.md"" sourceStartLineNumber=""4"" sourceEndLineNumber=""4"">content-b</p>
</section>
</div>
";
            Assert.Equal(expected.Replace("\r\n", "\n"), actual);
        }

        [Fact]
        [Trait("Related", "DfmMarkdown")]
        public void TestTabGroup_2()
        {
            //var options = DocfxFlavoredMarked.CreateDefaultOptions();
            //options.ShouldExportSourceInfo = true;
            //var actual = DocfxFlavoredMarked.Markup(null, null, options, @"# [title-a](#tab/a)
            string actual = @"# [title-a](#tab/a)
            content - a
# [title-b](#tab/b/c)
content-b
- - -
# [title-a](#tab/a)
content-a
# [title-b](#tab/b/a)
content-b
- - -";
            var groupId = "uBn0rykxXo";
            var expected = $@"<div class=""tabGroup"" id=""tabgroup_{groupId}"" sourceFile=""test.md"" sourceStartLineNumber=""1"" sourceEndLineNumber=""5"">
<ul role=""tablist"">
<li role=""presentation"">
<a href=""#tabpanel_{groupId}_a"" role=""tab"" aria-controls=""tabpanel_{groupId}_a"" data-tab=""a"" tabindex=""0"" aria-selected=""true"" sourceFile=""test.md"" sourceStartLineNumber=""1"" sourceEndLineNumber=""1"">title-a</a>
</li>
<li role=""presentation"" aria-hidden=""true"" hidden=""hidden"">
<a href=""#tabpanel_{groupId}_b_c"" role=""tab"" aria-controls=""tabpanel_{groupId}_b_c"" data-tab=""b"" data-condition=""c"" tabindex=""-1"" sourceFile=""test.md"" sourceStartLineNumber=""3"" sourceEndLineNumber=""3"">title-b</a>
</li>
</ul>
<section id=""tabpanel_{groupId}_a"" role=""tabpanel"" data-tab=""a"">
<p sourceFile=""test.md"" sourceStartLineNumber=""2"" sourceEndLineNumber=""2"">content-a</p>
</section>
<section id=""tabpanel_{groupId}_b_c"" role=""tabpanel"" data-tab=""b"" data-condition=""c"" aria-hidden=""true"" hidden=""hidden"">
<p sourceFile=""test.md"" sourceStartLineNumber=""4"" sourceEndLineNumber=""4"">content-b</p>
</section>
</div>
<div class=""tabGroup"" id=""tabgroup_{groupId}-1"" sourceFile=""test.md"" sourceStartLineNumber=""6"" sourceEndLineNumber=""10"">
<ul role=""tablist"">
<li role=""presentation"">
<a href=""#tabpanel_{groupId}-1_a"" role=""tab"" aria-controls=""tabpanel_uBn0rykxXo-1_a"" data-tab=""a"" tabindex=""0"" aria-selected=""true"" sourceFile=""test.md"" sourceStartLineNumber=""6"" sourceEndLineNumber=""6"">title-a</a>
</li>
<li role=""presentation"">
<a href=""#tabpanel_{groupId}-1_b_a"" role=""tab"" aria-controls=""tabpanel_uBn0rykxXo-1_b_a"" data-tab=""b"" data-condition=""a"" tabindex=""-1"" sourceFile=""test.md"" sourceStartLineNumber=""8"" sourceEndLineNumber=""8"">title-b</a>
</li>
</ul>
<section id=""tabpanel_{groupId}-1_a"" role=""tabpanel"" data-tab=""a"">
<p sourceFile=""test.md"" sourceStartLineNumber=""7"" sourceEndLineNumber=""7"">content-a</p>
</section>
<section id=""tabpanel_{groupId}-1_b_a"" role=""tabpanel"" data-tab=""b"" data-condition=""a"" aria-hidden=""true"" hidden=""hidden"">
<p sourceFile=""test.md"" sourceStartLineNumber=""9"" sourceEndLineNumber=""9"">content-b</p>
</section>
</div>
";
            Assert.Equal(expected.Replace("\r\n", "\n"), actual);
        }

        [Fact]
        [Trait("Related", "DfmMarkdown")]
        public void TestAllExtentsions()
        {
            string source = @"---
title: ""如何使用 Visual C++ 工具集报告问题 | Microsoft Docs""
ms.custom: 
ms.date: 11/04/2016
ms.reviewer: 
ms.suite: 
ms.technology:
- cpp
ms.tgt_pltfrm: 
ms.topic: article
dev_langs:
- C++
ms.assetid: ec24a49c-411d-47ce-aa4b-8398b6d3e8f6
caps.latest.revision: 8
author: corob-msft
ms.author: corob
manager: ghogen
translation.priority.mt:
- cs-cz
- pl-pl
- pt-br
- tr-tr
translationtype: Human Translation
ms.sourcegitcommit: 5c6fbfc8699d7d66c40b0458972d8b6ef0dcc705
ms.openlocfilehash: 2ea129ac94cb1ddc7486ba69280dc0390896e088

---

## Inclusion

### Block inclusion

[!include[block](~/includes/blockIncludeFile.md)]

### Inline inclusion

Token is [!include[TEXT](includes/testtoken.md)]

## Code Snippet

[!code-cs[code](~/code/code.cs#1)]

[!code-cs[code](~/code/code.cs)]

[!code-cs[Main](~/code/code.cs?highlight=3-4&range=4-7,15-20 ""This includes the whole file with lines 3-4 highlighted"")]

[test link](topic.md)

[test link1](Topics/topic.md)

<xref:Microsoft.Build.Tasks>

@Microsoft.Build.Tasks

@""Microsoft.Build.Tasks?text=Tasks""

[link_text](xref:Microsoft.Build.Tasks)

<xref:Microsoft.Build.Tasks#Anchor_1>

<xref href=""Microsoft.Build.Tasks?alt=ImmutableArray""/>

<xref:""Microsoft.Build.Tasks?alt=ImmutableArray"">

<a href=""xref:Microsoft.Build.Tasks?displayProperty=fullName""/>

## Note / Section / Video

### Note Sample

> [!NOTE]
> note content
> [!TIP]
> tip content
> [!WARNING]
> warning content
> [!IMPORTANT]
> important content
> [!CAUTION]
> caution content

### Section Sample

> [!div class=""op_single_selector""]
> * [Google](https://www.google.com)
> * [Bing](https://www.bing.com/)

### Video Sample

> [!Video https://sec.ch9.ms/ch9/4393/7d7c7df7-3f15-4a65-a2f7-3e4d0bea4393/Episode208_mid.mp4]

## CommonMark sample

- # Foo
- Bar
  ---
  baz";
            string blockIncludeFile = @"Hello World.

[!code[Main](~/code/code.cs?range=2,4-7,9-20 ""Test in include file"")]

Update without force build, while a.md include b.md and b.md updated.";

            string testtoken = @"terry & jack";

            string code = @"// <snippet1>
using System;

public struct Temperature
{
   private decimal temp;
   private DateTime tempDate;

   public Temperature(decimal temperature, DateTime dateAndTime)
   {
      this.temp = temperature;
      this.tempDate = dateAndTime;
   }

   public decimal Degrees
   { get { return this.temp; } }

   public DateTime Date
   { get { return this.tempDate; } }
}
// </snippet1>

#region 2
csr
#endregion";

            if (!Directory.Exists("includes")) Directory.CreateDirectory("includes");
            if (!Directory.Exists("code")) Directory.CreateDirectory("code");
            File.WriteAllText("includes/blockIncludeFile.md", blockIncludeFile);
            File.WriteAllText("includes/testtoken.md", testtoken);
            File.WriteAllText("code/code.cs", code);

            string expected = @"<yamlheader start=""1"" end=""27"">title: &quot;如何使用 Visual C++ 工具集报告问题 | Microsoft Docs&quot;
ms.custom: 
ms.date: 11/04/2016
ms.reviewer: 
ms.suite: 
ms.technology:
- cpp
ms.tgt_pltfrm: 
ms.topic: article
dev_langs:
- C++
ms.assetid: ec24a49c-411d-47ce-aa4b-8398b6d3e8f6
caps.latest.revision: 8
author: corob-msft
ms.author: corob
manager: ghogen
translation.priority.mt:
- cs-cz
- pl-pl
- pt-br
- tr-tr
translationtype: Human Translation
ms.sourcegitcommit: 5c6fbfc8699d7d66c40b0458972d8b6ef0dcc705
ms.openlocfilehash: 2ea129ac94cb1ddc7486ba69280dc0390896e088
</yamlheader><h2 id=""inclusion"">Inclusion</h2>
<h3 id=""block-inclusion"">Block inclusion</h3>
<p>Hello World.</p>
<pre><code name=""Main"" title=""Test in include file"">using System;
public struct Temperature
{
   private decimal temp;
   private DateTime tempDate;
   public Temperature(decimal temperature, DateTime dateAndTime)
   {
      this.temp = temperature;
      this.tempDate = dateAndTime;
   }

   public decimal Degrees
   { get { return this.temp; } }

   public DateTime Date
   { get { return this.tempDate; } }
}
</code></pre>
<p>Update without force build, while a.md include b.md and b.md updated.</p>
<h3 id=""inline-inclusion"">Inline inclusion</h3>
<p>Token is terry &amp; jack</p>
<h2 id=""code-snippet"">Code Snippet</h2>
<pre><code class=""lang-cs"" name=""code"">using System;

public struct Temperature
{
   private decimal temp;
   private DateTime tempDate;

   public Temperature(decimal temperature, DateTime dateAndTime)
   {
      this.temp = temperature;
      this.tempDate = dateAndTime;
   }

   public decimal Degrees
   { get { return this.temp; } }

   public DateTime Date
   { get { return this.tempDate; } }
}
</code></pre><pre><code class=""lang-cs"" name=""code"">// &lt;snippet1&gt;
using System;

public struct Temperature
{
   private decimal temp;
   private DateTime tempDate;

   public Temperature(decimal temperature, DateTime dateAndTime)
   {
      this.temp = temperature;
      this.tempDate = dateAndTime;
   }

   public decimal Degrees
   { get { return this.temp; } }

   public DateTime Date
   { get { return this.tempDate; } }
}
// &lt;/snippet1&gt;

#region 2
csr
#endregion
</code></pre><pre><code class=""lang-cs"" name=""Main"" title=""This includes the whole file with lines 3-4 highlighted"" highlight-lines=""3-4"">public struct Temperature
{
   private decimal temp;
   private DateTime tempDate;
   public decimal Degrees
   { get { return this.temp; } }

   public DateTime Date
   { get { return this.tempDate; } }
}
</code></pre>
<p><a href=""topic.md"">test link</a></p>
<p><a href=""Topics/topic.md"">test link1</a></p>
<p><xref href=""Microsoft.Build.Tasks"" data-throw-if-not-resolved=""True""></xref></p>
<p><xref href=""Microsoft.Build.Tasks"" data-throw-if-not-resolved=""False""></xref></p>
<p><xref href=""Microsoft.Build.Tasks?text=Tasks"" data-throw-if-not-resolved=""False""></xref></p>
<p><a href=""xref:Microsoft.Build.Tasks"">link_text</a></p>
<p><xref href=""Microsoft.Build.Tasks#Anchor_1"" data-throw-if-not-resolved=""True""></xref></p>
<xref href=""Microsoft.Build.Tasks?alt=ImmutableArray""/>
<p><xref href=""Microsoft.Build.Tasks?alt=ImmutableArray"" data-throw-if-not-resolved=""True""></xref></p>
<a href=""xref:Microsoft.Build.Tasks?displayProperty=fullName""/>
<h2 id=""note-section-video"">Note / Section / Video</h2>
<h3 id=""note-sample"">Note Sample</h3>
<div class=""NOTE"">
<h5>NOTE</h5>
<p>note content</p>
</div>
<div class=""TIP"">
<h5>TIP</h5>
<p>tip content</p>
</div>
<div class=""WARNING"">
<h5>WARNING</h5>
<p>warning content</p>
</div>
<div class=""IMPORTANT"">
<h5>IMPORTANT</h5>
<p>important content</p>
</div>
<div class=""CAUTION"">
<h5>CAUTION</h5>
<p>caution content</p>
</div>
<h3 id=""section-sample"">Section Sample</h3>
<div class=""op_single_selector"">
<ul>
<li><a href=""https://www.google.com"">Google</a></li>
<li><a href=""https://www.bing.com/"">Bing</a></li>
</ul>
</div>
<h3 id=""video-sample"">Video Sample</h3>
<div class=""embeddedvideo""><iframe src=""https://sec.ch9.ms/ch9/4393/7d7c7df7-3f15-4a65-a2f7-3e4d0bea4393/Episode208_mid.mp4"" frameborder=""0"" allowfullscreen=""true""></iframe></div>
<h2 id=""commonmark-sample"">CommonMark sample</h2>
<ul>
<li><h1 id=""foo"">Foo</h1>
</li>
<li><h2 id=""bar"">Bar</h2>
baz</li>
</ul>
";
            var marked = SimpleMarkup(source);
            Assert.Equal(expected.Replace("\r\n", "\n"), marked.Html);
        }
    }
}
