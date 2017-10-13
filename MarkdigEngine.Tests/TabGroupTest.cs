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
                @"# [title-a](#tab/a)
content-a
# [title-b](#tab/b/c)
content-b
- - -",
                $@"<div class=""tabGroup"" id=""tabgroup_{groupId}"">
<ul role=""tablist"">
<li role=""presentation"">
<a href=""#tabpanel_{groupId}_a"" role=""tab"" aria-controls=""tabpanel_{groupId}_a"" data-tab=""a"" tabindex=""0"" aria-selected=""true"">title-a</a>
</li>
<li role=""presentation"">
<a href=""#tabpanel_{groupId}_b_c"" role=""tab"" aria-controls=""tabpanel_{groupId}_b_c"" data-tab=""b"" data-condition=""c"" tabindex=""-1"">title-b</a>
</li>
</ul>
<section id=""tabpanel_{groupId}_a"" role=""tabpanel"" data-tab=""a"">
<p>content-a</p>
</section>
<section id=""tabpanel_{groupId}_b_c"" role=""tabpanel"" data-tab=""b"" data-condition=""c"" aria-hidden=""true"" hidden=""hidden"">
<p>content-b</p>
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
                BasePath = "."
            };
            var service = new MarkdigMarkdownService(parameter);
            return service.Markup(source, "Topic.md");
        }
    }
}
