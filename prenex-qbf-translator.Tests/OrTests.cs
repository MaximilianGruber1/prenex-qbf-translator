using Xunit;
using prenex_qbf_translator.Language;

namespace prenex_qbf_translator.Tests
{
    public class OrTests
    {
        [Fact]
        public void ToString_JoinWithPipe()
        {
            var a = new Variable("a");
            var b = new Variable("b");
            var or = new Or(new IFormula[] { a, b });
            Assert.Equal("(a | b)", or.ToString());
        }

        [Fact]
        public void Negated_ReturnsAndOfNegatedOperands()
        {
            var a = new Variable("a");
            var b = new Variable("b");
            var or = new Or(new IFormula[] { a, b });
            var neg = or.Negated();
            Assert.IsType<And>(neg);
            Assert.Equal("(~a & ~b)", neg.ToString());
        }
    }
}
