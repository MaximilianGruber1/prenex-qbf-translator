using System;
using System.Collections.Generic;
using Xunit;
using prenex_qbf_translator.Language;

namespace prenex_qbf_translator.Tests
{
    public class SubstitutionAllTests
    {
        [Fact]
        public void Substitution_RejectsNullEntries()
        {
            var dict = new Dictionary<Variable, IFormula>();
            Assert.Throws<ArgumentNullException>(() => new Substitution(null!));
        }

        [Fact]
        public void Substitution_RejectsNullKeyOrValue()
        {
            var withNullValue = new Dictionary<Variable, IFormula> { { new Variable("x"), null! } };
            Assert.Throws<ArgumentNullException>(() => new Substitution(withNullValue));
        }

        [Fact]
        public void ApplySubstitution_ReplacesInAnd()
        {
            var a = new Variable("a");
            var b = new Variable("b");
            var and = new And(new IFormula[] { a, b });
            var subs = new Substitution(new Dictionary<Variable, IFormula> { { a, new Variable("x") } });
            var ret = and.ApplySubstitution(subs);
            Assert.Same(and, ret);
            Assert.Equal("(x & b)", and.ToString());
        }

        [Fact]
        public void ApplySubstitution_OnOr_ReplacesOperands()
        {
            var a = new Variable("a");
            var b = new Variable("b");
            var or = new Or(new IFormula[] { a, b });
            var subs = new Substitution(new Dictionary<Variable, IFormula> { { a, new Variable("x") } });
            var ret = or.ApplySubstitution(subs);
            Assert.Same(or, ret);
            Assert.Equal("(x | b)", or.ToString());
        }

        [Fact]
        public void ApplySubstitution_OnNot_ReplacesInner()
        {
            var a = new Variable("a");
            var not = new Not(a);
            var subs = new Substitution(new Dictionary<Variable, IFormula> { { a, new Variable("x") } });
            var ret = not.ApplySubstitution(subs);
            Assert.Same(not, ret);
            Assert.Equal("~x", not.ToString());
        }

        [Fact]
        public void ApplySubstitution_SubstituteVariableWithComplexFormula()
        {
            var a = new Variable("a");
            var b = new Variable("b");
            // complex formula: (x & ~y)
            var complex = new And(new IFormula[] { new Variable("x"), new Not(new Variable("y")) });
            var or = new Or(new IFormula[] { a, b });
            var subs = new Substitution(new Dictionary<Variable, IFormula> { { a, complex } });
            var ret = or.ApplySubstitution(subs);
            Assert.Same(or, ret);
            Assert.Equal("((x & ~y) | b)", or.ToString());
        }
    }
}
