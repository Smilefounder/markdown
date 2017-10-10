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
            TestDfmInGeneral(
                @"# [title-a](#tab/a)
content-a
# [title-b](#tab/b)
content-b
- - -",
                @"<!-- todo: tab group -->
");
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
