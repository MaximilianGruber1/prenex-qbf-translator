using System.Collections.Generic;
using Xunit;
using prenex_qbf_translator.Language;

namespace prenex_qbf_translator.Tests
{
    public class SubstitutionTests
    {
        [Fact]
        public void Variable_ApplySubstitution_ReplacesVariable()
        {
            var x = new Variable("x");
            var y = new Variable("y");
            var subst = new Substitution(new Dictionary<Variable, IFormula> { { x, y } });
            var result = x.ApplySubstitution(subst);
            Assert.Equal(y, result);
        }
    }
}
