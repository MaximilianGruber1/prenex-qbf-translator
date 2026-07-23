using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using prenex_qbf_translator.Language;

namespace prenex_qbf_translator.Tests
{
    public class AndTests
    {
        private class TestFormula : IFormula
        {
            private readonly string _repr;
            private readonly IEnumerable<Variable> _vars;
            private readonly IEnumerable<Variable> _freeVars;
            private readonly int _nBlocks;
            private readonly int _nQuantified;
            private readonly int _length;
            private readonly int _depth;
            private readonly IFormula _negatedReturn;
            private readonly IFormula _applySubstReturn;

            public TestFormula(
                string repr,
                IEnumerable<Variable>? vars = null,
                IEnumerable<Variable>? freeVars = null,
                int nBlocks = 0,
                int nQuantified = 0,
                int length = 1,
                int depth = 0,
                IFormula? negatedReturn = null,
                IFormula? applySubstReturn = null)
            {
                _repr = repr;
                _vars = vars ?? Enumerable.Empty<Variable>();
                _freeVars = freeVars ?? Enumerable.Empty<Variable>();
                _nBlocks = nBlocks;
                _nQuantified = nQuantified;
                _length = length;
                _depth = depth;
                _negatedReturn = negatedReturn ?? new Not(this);
                _applySubstReturn = applySubstReturn ?? this;
            }

            public IFormula Negated() => _negatedReturn;
            public IEnumerable<Variable> Variables() => _vars;
            public IEnumerable<Variable> FreeVariables() => _freeVars;
            public int NBlocks() => _nBlocks;
            public int NQuantifiedVariables() => _nQuantified;
            public int Length() => _length;
            public int QuantifierDepth() => _depth;
            public IFormula ApplySubstitution(Substitution substitution) => _applySubstReturn;
            public override string ToString() => _repr;
        }

        [Fact]
        public void Constructor_NullOperands_Throws()
        {
            Assert.Throws<ArgumentNullException>(() => new And(null!));
        }

        [Fact]
        public void Constructor_TooFewOperands_Throws()
        {
            var a = new Variable("a");
            Assert.Throws<ArgumentException>(() => new And(new IFormula[] { a }));
        }

        [Fact]
        public void ToString_TwoOperands_JoinWithAmpersand()
        {
            var a = new Variable("a");
            var b = new Variable("b");
            var and = new And(new IFormula[] { a, b });
            Assert.Equal("(a & b)", and.ToString());
        }

        [Fact]
        public void ToString_ThreeOperands_JoinWithAmpersand()
        {
            var a = new Variable("a");
            var b = new Variable("b");
            var c = new Variable("c");
            var and = new And(new IFormula[] { a, b, c });
            Assert.Equal("(a & b & c)", and.ToString());
        }

        [Fact]
        public void Negated_TwoOperands_ReturnsOrOfNegatedOperands()
        {
            var a = new Variable("a");
            var b = new Variable("b");
            var and = new And(new IFormula[] { a, b });
            var neg = and.Negated();
            Assert.IsType<Or>(neg);
            Assert.Equal("(~a | ~b)", neg.ToString());
        }

        [Fact]
        public void Negated_ThreeOperands_ReturnsOrOfNegatedOperands()
        {
            var a = new Variable("a");
            var b = new Variable("b");
            var c = new Variable("c");
            var and = new And(new IFormula[] { a, b, c });
            var neg = and.Negated();
            Assert.IsType<Or>(neg);
            Assert.Equal("(~a | ~b | ~c)", neg.ToString());
        }

        [Fact]
        public void Variables_ReturnsDistinctVariables()
        {
            var a = new Variable("a");
            var b = new Variable("b");
            var and = new And(new IFormula[] { a, b, a });
            var vars = and.Variables().Select(v => v.Name).OrderBy(n => n).ToArray();
            Assert.Equal(new[] { "a", "b" }, vars);
        }

        [Fact]
        public void Variables_NoDuplicates_WithMultipleOccurrences()
        {
            var a = new Variable("a");
            var b = new Variable("b");
            var c = new Variable("c");
            var and = new And(new IFormula[] { a, b, a, c, b });
            var vars = and.Variables().Select(v => v.Name).OrderBy(n => n).ToArray();
            Assert.Equal(new[] { "a", "b", "c" }, vars);
        }

        [Fact]
        public void FreeVariables_ReturnsDistinctFreeVariables()
        {
            var a = new Variable("a");
            var b = new Variable("b");
            var and = new And(new IFormula[] { a, b, a });
            var fvars = and.FreeVariables().Select(v => v.Name).OrderBy(n => n).ToArray();
            Assert.Equal(new[] { "a", "b" }, fvars);
        }

        [Fact]
        public void NBlocks_SumsOperandValues()
        {
            var f1 = new TestFormula("f1", nBlocks: 2);
            var f2 = new TestFormula("f2", nBlocks: 3);
            var and = new And(new IFormula[] { f1, f2 });
            Assert.Equal(5, and.NBlocks());
        }

        [Fact]
        public void NQuantifiedVariables_SumsOperandValues()
        {
            var f1 = new TestFormula("f1", nQuantified: 4);
            var f2 = new TestFormula("f2", nQuantified: 1);
            var and = new And(new IFormula[] { f1, f2 });
            Assert.Equal(5, and.NQuantifiedVariables());
        }

        [Fact]
        public void Length_ComputesCorrectly()
        {
            var f1 = new TestFormula("f1", length: 2);
            var f2 = new TestFormula("f2", length: 3);
            var and = new And(new IFormula[] { f1, f2 });
            Assert.Equal(2 * 2 - 1 + 2 + 3, and.Length());
        }

        [Fact]
        public void QuantifierDepth_ReturnsMaxOfOperands()
        {
            var f1 = new TestFormula("f1", depth: 1);
            var f2 = new TestFormula("f2", depth: 4);
            var and = new And(new IFormula[] { f1, f2 });
            Assert.Equal(4, and.QuantifierDepth());
        }

        [Fact]
        public void ApplySubstitution_ReplacesOperandsAndReturnsThis()
        {
            var a = new Variable("a");
            var b = new Variable("b");
            var and = new And(new IFormula[] { a, b });

            var subs = new Substitution(new Dictionary<Variable, IFormula>
            {
                { a, new Variable("x") }
            });

            var returned = and.ApplySubstitution(subs);
            Assert.Same(and, returned);
            Assert.Equal("(x & b)", and.ToString());
        }
    }
}
