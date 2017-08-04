using MarkdigEngine;
using Microsoft.DocAsCode.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace MarkdigEngineTest
{
    public class YamlHeaderTest
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
        public void TestDfm_InvalidYamlHeader_YamlUtilityThrowException()
        {
            var source = @"---
- Jon Schlinkert
- Brian Woodward

---";
            var expected = @"<hr />
<ul>
<li>Jon Schlinkert</li>
<li>Brian Woodward</li>
</ul>
<hr />
";
            var marked = SimpleMarkup(source);
            Assert.Equal(expected.Replace("\r\n", "\n"), marked.Html);
        }


        [Fact]
        [Trait("Related", "DfmMarkdown")]
        public void TestDfmYamlHeader_YamlUtilityReturnNull()
        {
            var source = @"---

### /Unconfigure

---";
            var expected = @"<hr />
<h3 id=""unconfigure"">/Unconfigure</h3>
<hr />
";
            var marked = SimpleMarkup(source);
            Assert.Equal(expected.Replace("\r\n", "\n"), marked.Html);
        }
    }
}
