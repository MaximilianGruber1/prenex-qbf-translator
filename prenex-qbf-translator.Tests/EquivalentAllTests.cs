using System;
using System.Linq;
using Xunit;
using prenex_qbf_translator.Language;
using System.Collections.Generic;

namespace prenex_qbf_translator.Tests
{
    public class EquivalentAllTests
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
        public void Constructor_NullOperands_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new Equivalent(null!));
        }

        [Fact]
        public void Variables_FreeVariables_Distinct()
        {
            var a = new Variable("a");
            var b = new Variable("b");
            var eq = new Equivalent(new IFormula[] { a, b, a });
            var vars = eq.Variables().Select(v => v.Name).OrderBy(n => n).ToArray();
            var fvars = eq.FreeVariables().Select(v => v.Name).OrderBy(n => n).ToArray();
            Assert.Equal(new[] { "a", "b" }, vars);
            Assert.Equal(new[] { "a", "b" }, fvars);
        }

        [Fact]
        public void NBlocks_NQuantified_Length_Depth()
        {
            var f1 = new SimpleFormula("f1", nBlocks: 1, nQuantified: 2, length: 3, depth: 2);
            var f2 = new SimpleFormula("f2", nBlocks: 2, nQuantified: 3, length: 4, depth: 5);
            var eq = new Equivalent(new IFormula[] { f1, f2 });
            Assert.Equal(3, eq.NBlocks());
            Assert.Equal(5, eq.NQuantifiedVariables());
            Assert.Equal(2 * 2 - 1 + 3 + 4, eq.Length());
            Assert.Equal(5, eq.QuantifierDepth());
        }

        [Fact]
        public void ApplySubstitution_ReturnsThisAndUpdatesOperands()
        {
            var a = new Variable("a");
            var b = new Variable("b");
            var eq = new Equivalent(new IFormula[] { a, b });
            var subs = new Substitution(new System.Collections.Generic.Dictionary<Variable, IFormula> { { a, new Variable("x") } });
            var ret = eq.ApplySubstitution(subs);
            Assert.Same(eq, ret);
            Assert.Equal("(x <=> b)", eq.ToString());
        }

        [Fact]
        public void Constructor_TooFewOperands_Throws()
        {
            var a = new Variable("a");
            Assert.Throws<ArgumentException>(() => new Equivalent(new IFormula[] { a }));
        }

        [Fact]
        public void ToString_MultipleOperands_Formats()
        {
            var a = new Variable("a");
            var b = new Variable("b");
            var eq = new Equivalent(new IFormula[] { a, b });
            Assert.Equal("(a <=> b)", eq.ToString());
        }

        [Fact]
        public void Negated_ReturnsNot()
        {
            var a = new Variable("a");
            var b = new Variable("b");
            var eq = new Equivalent(new IFormula[] { a, b });
            var neg = eq.Negated();
            Assert.IsType<Not>(neg);
        }
    }
}
