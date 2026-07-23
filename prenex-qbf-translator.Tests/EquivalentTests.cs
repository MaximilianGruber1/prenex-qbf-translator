using Xunit;
using prenex_qbf_translator.Language;

namespace prenex_qbf_translator.Tests
{
    public class EquivalentTests
    {
        [Fact]
        public void ToString_UseDoubleArrow()
        {
            var a = new Variable("a");
            var b = new Variable("b");
            var eq = new Equivalent(new IFormula[] { a, b });
            Assert.Equal("(a <=> b)", eq.ToString());
        }

        [Fact]
        public void Negated_IsNotOfThis()
        {
            var a = new Variable("a");
            var b = new Variable("b");
            var eq = new Equivalent(new IFormula[] { a, b });
            var neg = eq.Negated();
            Assert.IsType<Not>(neg);
        }
    }
}
