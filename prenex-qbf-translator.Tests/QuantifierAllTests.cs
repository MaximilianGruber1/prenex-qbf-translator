using System;
using System.Linq;
using Xunit;
using prenex_qbf_translator.Language;
using System.Collections.Generic;

namespace prenex_qbf_translator.Tests
{
    public class QuantifierAllTests
    {
        [Fact]
        public void Forall_ToString_MultipleVars()
        {
            var x = new Variable("x");
            var y = new Variable("y");
            var inner = new Variable("p");
            var forall = new Forall(new[] { x, y }, inner);
            Assert.Equal("! x, y: p", forall.ToString());
        }

        [Fact]
        public void Exists_ToString_MultipleVars()
        {
            var x = new Variable("x");
            var y = new Variable("y");
            var inner = new Variable("p");
            var exists = new Exists(new[] { x, y }, inner);
            Assert.Equal("? x, y: p", exists.ToString());
        }

        [Fact]
        public void Quantifier_DelegatesProperties()
        {
            var x = new Variable("x");
            var inner = new Variable("p");
            var forall = new Forall(new[] { x }, inner);
            var vars = forall.Variables().ToList();
            Assert.Contains(inner, vars);
            Assert.Contains(x, vars);
            var fvars = forall.FreeVariables().ToList();
            Assert.DoesNotContain(x, fvars);
            Assert.Contains(inner, fvars);
        }

        [Fact]
        public void Forall_Constructor_Validation()
        {
            var inner = new Variable("p");
            Assert.Throws<ArgumentNullException>(() => new Forall(null!, inner));
            Assert.Throws<ArgumentException>(() => new Forall(Array.Empty<Variable>(), inner));
            Assert.Throws<ArgumentNullException>(() => new Forall(new[] { new Variable("x") }, null!));
        }

        [Fact]
        public void Exists_Constructor_Validation()
        {
            var inner = new Variable("p");
            Assert.Throws<ArgumentNullException>(() => new Exists(null!, inner));
            Assert.Throws<ArgumentException>(() => new Exists(Array.Empty<Variable>(), inner));
            Assert.Throws<ArgumentNullException>(() => new Exists(new[] { new Variable("x") }, null!));
        }

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
        public void Forall_Negated_ReturnsExistsWithNegatedInner()
        {
            var x = new Variable("x");
            var inner = new Variable("p");
            var forall = new Forall(new[] { x }, inner);
            var neg = forall.Negated();
            Assert.IsType<Exists>(neg);
            Assert.Equal("? x: ~p", neg.ToString());
        }

        [Fact]
        public void Forall_NumericProperties()
        {
            var x = new Variable("x");
            var f = new SimpleFormula("f", nBlocks: 2, nQuantified: 3, length: 4, depth: 2);
            var forall = new Forall(new[] { x }, f);
            Assert.Equal(1 + 2, forall.NBlocks());
            Assert.Equal(1 + 3, forall.NQuantifiedVariables());
            Assert.Equal(1 + 1 + 4, forall.Length());
            Assert.Equal(1 + 2, forall.QuantifierDepth());
        }

        [Fact]
        public void Forall_ApplySubstitution_FiltersBoundVariables()
        {
            var x = new Variable("x");
            var y = new Variable("y");
            var inner = new SimpleFormula("f");
            var forall = new Forall(new[] { x }, inner);
            var subs = new Substitution(new System.Collections.Generic.Dictionary<Variable, IFormula> { { x, y }, { y, y } });
            var ret = forall.ApplySubstitution(subs);
            Assert.Same(forall, ret);
        }
    }
}
