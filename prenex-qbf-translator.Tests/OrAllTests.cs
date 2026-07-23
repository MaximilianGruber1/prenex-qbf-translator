using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using prenex_qbf_translator.Language;

namespace prenex_qbf_translator.Tests
{
    public class OrAllTests
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
            Assert.Throws<ArgumentNullException>(() => new Or(null!));
        }

        [Fact]
        public void Constructor_TooFewOperands_Throws()
        {
            var a = new Variable("a");
            Assert.Throws<ArgumentException>(() => new Or(new IFormula[] { a }));
        }

        [Fact]
        public void ToString_MultipleOperands_JoinWithPipe()
        {
            var a = new Variable("a");
            var b = new Variable("b");
            var c = new Variable("c");
            var or = new Or(new IFormula[] { a, b, c });
            Assert.Equal("(a | b | c)", or.ToString());
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

        [Fact]
        public void Variables_NoDuplicates()
        {
            var a = new Variable("a");
            var b = new Variable("b");
            var or = new Or(new IFormula[] { a, b, a });
            var vars = or.Variables().Select(v => v.Name).OrderBy(n => n).ToArray();
            Assert.Equal(new[] { "a", "b" }, vars);
        }

        [Fact]
        public void FreeVariables_NoDuplicates()
        {
            var a = new Variable("a");
            var b = new Variable("b");
            var or = new Or(new IFormula[] { a, b, a });
            var fvars = or.FreeVariables().Select(v => v.Name).OrderBy(n => n).ToArray();
            Assert.Equal(new[] { "a", "b" }, fvars);
        }

        [Fact]
        public void NBlocks_SumsOperands()
        {
            var f1 = new SimpleFormula("f1", nBlocks: 2);
            var f2 = new SimpleFormula("f2", nBlocks: 3);
            var or = new Or(new IFormula[] { f1, f2 });
            Assert.Equal(5, or.NBlocks());
        }

        [Fact]
        public void NQuantifiedVariables_SumsOperands()
        {
            var f1 = new SimpleFormula("f1", nQuantified: 4);
            var f2 = new SimpleFormula("f2", nQuantified: 1);
            var or = new Or(new IFormula[] { f1, f2 });
            Assert.Equal(5, or.NQuantifiedVariables());
        }

        [Fact]
        public void Length_ComputesCorrectly()
        {
            var f1 = new SimpleFormula("f1", length: 2);
            var f2 = new SimpleFormula("f2", length: 3);
            var or = new Or(new IFormula[] { f1, f2 });
            Assert.Equal(2 * 2 - 1 + 2 + 3, or.Length());
        }

        [Fact]
        public void QuantifierDepth_ReturnsMax()
        {
            var f1 = new SimpleFormula("f1", depth: 1);
            var f2 = new SimpleFormula("f2", depth: 5);
            var or = new Or(new IFormula[] { f1, f2 });
            Assert.Equal(5, or.QuantifierDepth());
        }

        [Fact]
        public void ApplySubstitution_UpdatesOperandsAndReturnsThis()
        {
            var a = new Variable("a");
            var b = new Variable("b");
            var or = new Or(new IFormula[] { a, b });
            var subs = new Substitution(new Dictionary<Variable, IFormula> { { a, new Variable("x") } });
            var ret = or.ApplySubstitution(subs);
            Assert.Same(or, ret);
            Assert.Equal("(x | b)", or.ToString());
        }
    }
}
