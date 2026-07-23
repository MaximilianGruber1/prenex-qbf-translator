using System.Linq;
using Xunit;
using prenex_qbf_translator.Language;
using System.Collections.Generic;

namespace prenex_qbf_translator.Tests
{
    public class NotAllTests
    {
        private class SimpleFormula : IFormula
        {
            private readonly string _repr;
            private readonly IEnumerable<Variable> _vars;
            private readonly IEnumerable<Variable> _freeVars;
            private readonly int _nBlocks;
            private readonly int _nQuantified;
            private readonly int _length;
            private readonly int _depth;

            public SimpleFormula(string repr, IEnumerable<Variable>? vars = null, IEnumerable<Variable>? freeVars = null, int nBlocks = 0, int nQuantified = 0, int length = 1, int depth = 0)
            {
                _repr = repr;
                _vars = vars ?? Enumerable.Empty<Variable>();
                _freeVars = freeVars ?? Enumerable.Empty<Variable>();
                _nBlocks = nBlocks;
                _nQuantified = nQuantified;
                _length = length;
                _depth = depth;
            }

            public IFormula Negated() => new Not(this);
            public IEnumerable<Variable> Variables() => _vars;
            public IEnumerable<Variable> FreeVariables() => _freeVars;
            public int NBlocks() => _nBlocks;
            public int NQuantifiedVariables() => _nQuantified;
            public int Length() => _length;
            public int QuantifierDepth() => _depth;
            public IFormula ApplySubstitution(Substitution substitution) => this;
            public override string ToString() => _repr;
        }
        [Fact]
        public void ToString_PrefixTilde()
        {
            var a = new Variable("a");
            var not = new Not(a);
            Assert.Equal("~a", not.ToString());
        }

        [Fact]
        public void Negated_ReturnsInner()
        {
            var a = new Variable("a");
            var not = new Not(a);
            var neg = not.Negated();
            Assert.Equal(a, neg);
        }

        [Fact]
        public void DelegatesPropertiesToInner()
        {
            var inner = new Variable("x");
            var not = new Not(inner);
            Assert.Equal(inner.Variables(), not.Variables());
            Assert.Equal(inner.FreeVariables(), not.FreeVariables());
            Assert.Equal(inner.NBlocks(), not.NBlocks());
            Assert.Equal(inner.NQuantifiedVariables(), not.NQuantifiedVariables());
            Assert.Equal(inner.Length(), not.Length());
            Assert.Equal(inner.QuantifierDepth(), not.QuantifierDepth());
        }

        [Fact]
        public void ApplySubstitution_UpdatesInnerAndReturnsThis()
        {
            var a = new Variable("a");
            var b = new Variable("b");
            var not = new Not(a);
            var subs = new Substitution(new Dictionary<Variable, IFormula> { { a, b } });
            var ret = not.ApplySubstitution(subs);
            Assert.Same(not, ret);
            Assert.Equal("~b", not.ToString());
        }

        [Fact]
        public void ToString_FormatsWithInnerComplex()
        {
            var inner = new SimpleFormula("f1");
            var not = new Not(inner);
            Assert.Equal("~f1", not.ToString());
        }

        [Fact]
        public void Negated_OfNot_ReturnsInnerObject()
        {
            var inner = new SimpleFormula("f1");
            var not = new Not(inner);
            var neg = not.Negated();
            Assert.Equal(inner, neg);
        }
    }
}
