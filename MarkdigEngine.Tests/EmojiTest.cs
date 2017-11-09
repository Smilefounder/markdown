using Xunit;

namespace MarkdigEngine.Tests
{
    public class EmojiTest
    {
        [Fact(Skip = "Disable Emoij")]
        public void EmojiTestGeneral()
        {
            //arange
            var content = @"**content :** :smile:";

            var marked = TestUtility.Markup(content, "fake.md");

            // assert
            var expected = @"<p sourceFile=""fake.md"" sourceStartLineNumber=""1"" sourceEndLineNumber=""1""><strong sourceFile=""fake.md"" sourceStartLineNumber=""1"" sourceEndLineNumber=""1"">content :</strong> 😄</p>
".Replace("\r\n", "\n");
            Assert.Equal(expected.Replace("\r\n", "\n"), marked.Html);
        }
    }
}
