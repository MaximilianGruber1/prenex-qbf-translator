using Xunit;
using prenex_qbf_translator.Language;

namespace prenex_qbf_translator.Tests
{
    public class VariableTests
    {
        [Fact]
        public void ToString_ReturnsName()
        {
            var v = new Variable("x");
            Assert.Equal("x", v.ToString());
        }

        [Fact]
        public void Negated_ReturnsNot()
        {
            var v = new Variable("x");
            var n = v.Negated();
            Assert.IsType<Not>(n);
        }
    }
}
