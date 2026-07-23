using System;
using Xunit;
using prenex_qbf_translator.Language;

namespace prenex_qbf_translator.Tests
{
    public class VariableAllTests
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

        [Fact]
        public void ApplySubstitution_ReplacesVariable()
        {
            var x = new Variable("x");
            var y = new Variable("y");
            var subst = new Substitution(new System.Collections.Generic.Dictionary<Variable, IFormula> { { x, y } });
            var result = x.ApplySubstitution(subst);
            Assert.Equal(y, result);
        }

        [Fact]
        public void Constructor_NullName_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new Variable(null!));
        }

        [Fact]
        public void Variables_ReturnsSelf()
        {
            var v = new Variable("z");
            var vars = v.Variables();
            Assert.Contains(v, vars);
        }

        [Fact]
        public void FreeVariables_ReturnsSelf()
        {
            var v = new Variable("z");
            var fvars = v.FreeVariables();
            Assert.Contains(v, fvars);
        }

        [Fact]
        public void Equals_SameName_True()
        {
            var a = new Variable("n");
            var b = new Variable("n");
            Assert.True(a.Equals(b));
            Assert.True(a.Equals((object)b));
        }

        [Fact]
        public void Equals_DifferentName_False()
        {
            var a = new Variable("n1");
            var b = new Variable("n2");
            Assert.False(a.Equals(b));
        }

        [Fact]
        public void GetHashCode_SameName_SameHash()
        {
            var a = new Variable("h");
            var b = new Variable("h");
            Assert.Equal(a.GetHashCode(), b.GetHashCode());
        }

        [Fact]
        public void ApplySubstitution_NotPresent_ReturnsThis()
        {
            var x = new Variable("x");
            var subst = new Substitution(new System.Collections.Generic.Dictionary<Variable, IFormula>());
            var result = x.ApplySubstitution(subst);
            Assert.Same(x, result);
        }
    }
}
