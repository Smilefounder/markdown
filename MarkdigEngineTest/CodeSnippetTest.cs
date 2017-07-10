using MarkdigEngine;
using Microsoft.DocAsCode.Plugins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Xunit;

namespace MarkdigEngineTest
{
    public class CodeSnippetTest
    {

        [Fact]
        public void CodeSnippetGeneral()
        {
            //arange
            var content = @"    line for start & end
    // <tag1>
    line1
    // </tag1>
" + " \tline for indent & range";

            File.WriteAllText("Program.cs", content.Replace("\r\n", "\n"));

            // act
            var parameter = new MarkdownServiceParameters
            {
                BasePath = "."
            };
            var service = new MarkdigMarkdownService(parameter);
            var marked = service.Markup("[!code-csharp[name](Program.cs?start=1&end=1&name=tag&range=5-&highlight=1,2-2,4-&dedent=3#tag1)]", "Topic.md");

            // assert
            var expected = @"<pre><code name=""name"" class=""lang-csharp"" highlight-lines=""1,2,4-""> line for start &amp; end
 line1
 line for indent &amp; range
</code></pre>";
            Assert.Equal(expected.Replace("\r\n", "\n"), marked.Html);
        }

        [Fact]
        public void CodeSnippetShouldNotWorkInParagragh()
        {
            // act
            var parameter = new MarkdownServiceParameters
            {
                BasePath = "."
            };
            var service = new MarkdigMarkdownService(parameter);
            var marked = service.Markup("text [!code[test](CodeSnippet.cs)]", "Topic.md");

            // assert
            var expected = @"<p>text [!code<a href=""CodeSnippet.cs"">test</a>]</p>
";
            Assert.Equal(expected.Replace("\r\n", "\n"), marked.Html);
        }

        [Fact]
        public void CodeSnippetTagsShouldMatchCaseInsensitive()
        {
            //arange
            var content = @"// <tag1>
line1
// <tag2>
line2
// </tag2>
line3
// </TAG1>
// <unmatched>
";
            File.WriteAllText("Program.cs", content.Replace("\r\n", "\n"));

            // act
            var parameter = new MarkdownServiceParameters
            {
                BasePath = "."
            };
            var service = new MarkdigMarkdownService(parameter);
            var marked = service.Markup("[!code[tag1](Program.cs#Tag1)]", "Topic.md");

            // assert
            var expected = "<pre><code name=\"tag1\">line1\nline2\nline3\n</code></pre>";
            Assert.Equal(expected.Replace("\r\n", "\n"), marked.Html);
        }

        [Fact]
        public void CodeSnippetTagsShouldSucceedWhenDuplicateWithoutWarning()
        {
            //arange
            var content = @"// <tag1>
line1
// <tag1>
line2
// </tag1>
line3
// </TAG1>
// <tag2>
line4
// </tag2>
";
            File.WriteAllText("Program.cs", content.Replace("\r\n", "\n"));

            // act
            var parameter = new MarkdownServiceParameters
            {
                BasePath = "."
            };
            var service = new MarkdigMarkdownService(parameter);
            var marked = service.Markup("[!code[tag2](Program.cs#Tag2)]", "Topic.md");

            // assert
            var expected = "<pre><code name=\"tag2\">line4\n</code></pre>";
            Assert.Equal(expected.Replace("\r\n", "\n"), marked.Html);
        }

        [Fact]
        public void CodeSnippetTagsShouldSucceedWhenDuplicateWithWarningWhenReferenced()
        {
            //arange
            var content = @"// <tag1>
line1
// <tag1>
line2
// </tag1>
line3
// </TAG1>
// <tag2>
line4
// </tag2>
";
            File.WriteAllText("Program.cs", content.Replace("\r\n", "\n"));

            // act
            var parameter = new MarkdownServiceParameters
            {
                BasePath = "."
            };
            var service = new MarkdigMarkdownService(parameter);
            var result = service.Markup("[!code[tag1](Program.cs#Tag1)]", "Topic.md");

            // assert
            var expected = "<pre><code name=\"tag1\">line2\n</code></pre>";
            Assert.Equal(expected.Replace("\r\n", "\n"), result.Html);
        }

        [Fact]
        public void CodeSnippetTagsShouldSucceedWhenReferencedFileContainsRegionWithoutName()
        {
            // arrange
            var content = @"#region
public class MyClass
#region
{
    #region main
    static void Main()
    {
    }
    #endregion
}
#endregion
#endregion";
            File.WriteAllText("Program.cs", content.Replace("\r\n", "\n"));

            // act
            var parameter = new MarkdownServiceParameters
            {
                BasePath = "."
            };
            var service = new MarkdigMarkdownService(parameter);
            var marked = service.Markup("[!code[MyClass](Program.cs#main)]", "Topic.md");

            // assert
            var expected = @"<pre><code name=""MyClass"">static void Main()
{
}
</code></pre>";
            Assert.Equal(expected.Replace("\r\n", "\n"), marked.Html);
        }

        //[Theory]
        [Trait("Related", "DfmMarkdown")]
        [InlineData(@"> [!div class=""tabbedCodeSnippets"" data-resources=""OutlookServices.Calendar""]
>
>```cs-i
    var outlookClient = await CreateOutlookClientAsync(""Calendar"");
    var events = await outlookClient.Me.Events.Take(10).ExecuteAsync();
            foreach (var calendarEvent in events.CurrentPage)
            {
                System.Diagnostics.Debug.WriteLine(""Event '{0}'."", calendarEvent.Subject);
            }
```
> 
>```javascript-i
outlookClient.me.events.getEvents().fetch().then(function(result) {
        result.currentPage.forEach(function(event) {
        console.log('Event ""' + event.subject + '""')
    });
}, function(error)
    {
        console.log(error);
    });
```")]
        public void TestSectionBlockLevel(string source)
        {
            var parameter = new MarkdownServiceParameters
            {
                BasePath = "."
            };
            var service = new MarkdigMarkdownService(parameter);
            var content = service.Markup(source, "Topic.md");

            // assert
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(content.Html);
            var tabbedCodeNode = xdoc.SelectSingleNode("//div[@class='tabbedCodeSnippets' and @data-resources='OutlookServices.Calendar']");
            Assert.True(tabbedCodeNode != null);
            var csNode = tabbedCodeNode.SelectSingleNode("./pre/code[@class='lang-cs-i']");
            Assert.True(csNode != null);
            var jsNode = tabbedCodeNode.SelectSingleNode("./pre/code[@class='lang-javascript-i']");
            Assert.True(jsNode != null);
        }
    }
}
