using Microsoft.DocAsCode.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MarkdigEngine.Tests
{
    public class XrefTest
    {
        [Fact]
        public void XrefTestGeneral()
        {
            //arange
            var content = @"<xref:Microsoft.Build.Tasks>
@Microsoft.Build.Tasks
@""Microsoft.Build.Tasks?text=Tasks""
[link_text](xref:Microsoft.Build.Tasks)
<xref:Microsoft.Build.Tasks#Anchor_1>
<xref href=""Microsoft.Build.Tasks?alt=ImmutableArray""/>
<xref:""Microsoft.Build.Tasks?alt=ImmutableArray"">
<a href=""xref:Microsoft.Build.Tasks?displayProperty=fullName""/>
";
            // act
            var parameter = new MarkdownServiceParameters
            {
                BasePath = ".",
            };
            var service = new MarkdigMarkdownService(parameter);
            var marked = service.Markup(content, "Topic.md");

            // assert
            var expected = @"<p><xref href=""Microsoft.Build.Tasks"" data-throw-if-not-resolved=""True""></xref>
<xref href=""Microsoft.Build.Tasks"" data-throw-if-not-resolved=""False""></xref>
<xref href=""Microsoft.Build.Tasks?text=Tasks"" data-throw-if-not-resolved=""False""></xref>
<a href=""xref:Microsoft.Build.Tasks"">link_text</a>
<xref href=""Microsoft.Build.Tasks#Anchor_1"" data-throw-if-not-resolved=""True""></xref>
<xref href=""Microsoft.Build.Tasks?alt=ImmutableArray""/>
<xref href=""Microsoft.Build.Tasks?alt=ImmutableArray"" data-throw-if-not-resolved=""True""></xref>
<a href=""xref:Microsoft.Build.Tasks?displayProperty=fullName""/></p>
";
            Assert.Equal(expected.Replace("\r\n", "\n"), marked.Html);
        }
    }
}
