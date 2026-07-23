using System.Linq;
using Xunit;
using prenex_qbf_translator.Language;

namespace prenex_qbf_translator.Tests
{
    public class QuantifierTests
    {
        [Fact]
        public void Forall_MultipleVariables_ToString()
        {
            var x = new Variable("x");
            var y = new Variable("y");
            var inner = new Variable("p");
            var forall = new Forall(new[] { x, y }, inner);
            Assert.Equal("! x, y: p", forall.ToString());
        }

        [Fact]
        public void Exists_MultipleVariables_ToString()
        {
            var x = new Variable("x");
            var y = new Variable("y");
            var inner = new Variable("p");
            var exists = new Exists(new[] { x, y }, inner);
            Assert.Equal("? x, y: p", exists.ToString());
        }
    }
}
