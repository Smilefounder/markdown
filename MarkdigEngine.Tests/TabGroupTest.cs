using System.Collections.Generic;

using MarkdigEngine.Extensions;

using Microsoft.DocAsCode.Plugins;
using Xunit;

namespace MarkdigEngine.Tests
{
    
    public class TabGroupTest
    {
        [Fact]
        [Trait("Related", "TabGroup")]
        public void Test_General()
        {
            var groupId = "w61hnTEDJ7";
            TestDfmInGeneral(
                @"Tab group test case
# [title-a](#tab/a)
content-a
# [title-b](#tab/b/c)
content-b
- - -",
                $@"<p sourceFile=""Topic.md"" sourceStartLineNumber=""1"" sourceEndLineNumber=""1"">Tab group test case</p>
<div class=""tabGroup"" id=""tabgroup_{groupId}"" sourceFile=""Topic.md"" sourceStartLineNumber=""2"" sourceEndLineNumber=""5"">
<ul role=""tablist"">
<li role=""presentation"">
<a href=""#tabpanel_{groupId}_a"" role=""tab"" aria-controls=""tabpanel_{groupId}_a"" data-tab=""a"" tabindex=""0"" aria-selected=""true"" sourceFile=""Topic.md"" sourceStartLineNumber=""2"" sourceEndLineNumber=""2"">title-a</a>
</li>
<li role=""presentation"">
<a href=""#tabpanel_{groupId}_b_c"" role=""tab"" aria-controls=""tabpanel_{groupId}_b_c"" data-tab=""b"" data-condition=""c"" tabindex=""-1"" sourceFile=""Topic.md"" sourceStartLineNumber=""4"" sourceEndLineNumber=""4"">title-b</a>
</li>
</ul>
<section id=""tabpanel_{groupId}_a"" role=""tabpanel"" data-tab=""a"">
<p sourceFile=""Topic.md"" sourceStartLineNumber=""3"" sourceEndLineNumber=""3"">content-a</p>
</section>
<section id=""tabpanel_{groupId}_b_c"" role=""tabpanel"" data-tab=""b"" data-condition=""c"" aria-hidden=""true"" hidden=""hidden"">
<p sourceFile=""Topic.md"" sourceStartLineNumber=""5"" sourceEndLineNumber=""5"">content-b</p>
</section>
</div>
"
            );
        }

        private static void TestDfmInGeneral(string source, string expected)
        {
            var result = SimpleMarkup(source).Html;
            Assert.Equal(expected.Replace("\r\n", "\n"), result);
        }

        private static MarkupResult SimpleMarkup(string source)
        {
            var parameter = new MarkdownServiceParameters
            {
                BasePath = ".",
                Extensions = new Dictionary<string, object>
                {
                    { LineNumberExtension.EnableSourceInfo, true }
                }
            };
            var service = new MarkdigMarkdownService(parameter);
            return service.Markup(source, "Topic.md");
        }
    }
}
