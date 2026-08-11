using Xunit;
using prenex_qbf_translator.Language;

namespace prenex_qbf_translator.Tests
{
    public class NotTests
    {
        [Fact]
        public void ToString_PrefixTilde()
        {
            var a = new Variable("a");
            var not = new Not(a);
            Assert.Equal("~a", not.ToString());
        }
    }
}
