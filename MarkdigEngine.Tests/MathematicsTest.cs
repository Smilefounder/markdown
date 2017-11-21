using Xunit;

namespace MarkdigEngine.Tests
{
    public class MathematicsTest
    {
        [Fact(Skip ="Disable math support")]
        public void Test_Mathematics_Support_0()
        {
            var source = "$ math inline $";
            var expected = @"<p><span class=""math"">math inline</span></p>
";

            TestUtility.AssertEqual(expected, source, TestUtility.MarkupWithoutSourceInfo);
        }

        [Fact(Skip ="Disable math support")]
        public void Test_Mathematics_Support_1()
        {
            var source = "$ math^0 **inline** $";
            var expected = @"<p><span class=""math"">math^0 **inline**</span></p>
";

            TestUtility.AssertEqual(expected, source, TestUtility.MarkupWithoutSourceInfo);
        }
    }
}
