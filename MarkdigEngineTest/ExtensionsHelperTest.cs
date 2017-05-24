using MarkdigEngine.Extensions;

using Xunit;

namespace MarkdigEngineTest
{
    public class ExtensionsHelperTest
    {
        [Fact]
        public void TestResolveFilePathWithTilde_General()
        {
            var basePath = "C://";
            var tilde = "~/a/b/c.md";
            var resolvedPath = ExtensionsHelper.GetAbsolutePathWithTilde(basePath, tilde);
            var expected = "C:/a/b/c.md";

            Assert.Equal<string>(expected, resolvedPath);
        }

        [Fact]
        public void TestResolveFilePathWithTilde_Complicated()
        {
            var basePath = "C://a/b";
            var tilde = "~/c/d/..\\./../c.md";
            var resolvedPath = ExtensionsHelper.GetAbsolutePathWithTilde(basePath, tilde);
            var expected = "C:/a/b/c.md";

            Assert.Equal<string>(expected, resolvedPath);
        }
    }
}
