using System;
using Xunit;
using prenex_qbf_translator.Language;

namespace prenex_qbf_translator.Tests
{
    public class ConstantsAllTests
    {
        [Fact]
        public void TrueConstant_ToString_IsTrue()
        {
            var t = new TrueConstant();
            Assert.Equal("true", t.ToString());
        }

        [Fact]
        public void FalseConstant_ToString_IsFalse()
        {
            var f = new FalseConstant();
            Assert.Equal("false", f.ToString());
        }

        [Fact]
        public void True_Negated_IsNotWrap()
        {
            var t = new TrueConstant();
            var neg = new Not(t);
            Assert.IsType<Not>(neg);
            Assert.Equal("~true", neg.ToString());
        }

        [Fact]
        public void False_Negated_IsNotWrap()
        {
            var f = new FalseConstant();
            var neg = new Not(f);
            Assert.IsType<Not>(neg);
            Assert.Equal("~false", neg.ToString());
        }

        [Fact]
        public void Constants_VariablesAndFreeVariables_AreEmpty()
        {
            var t = new TrueConstant();
            var f = new FalseConstant();
            Assert.Empty(t.Variables());
            Assert.Empty(t.FreeVariables());
            Assert.Empty(f.Variables());
            Assert.Empty(f.FreeVariables());
        }

        [Fact]
        public void Constants_LengthAndCounts()
        {
            var t = new TrueConstant();
            var f = new FalseConstant();
            Assert.Equal(1, t.Length());
            Assert.Equal(1, f.Length());
            Assert.Equal(0, t.NBlocks());
            Assert.Equal(0, f.NBlocks());
            Assert.Equal(0, t.NQuantifiedVariables());
        }

        [Fact]
        public void Constants_ApplySubstitution_ReturnsThis()
        {
            var t = new TrueConstant();
            var subs = new Substitution(new System.Collections.Generic.Dictionary<Variable, IFormula>());
            var ret = t.ApplySubstitution(subs);
            Assert.Same(t, ret);
        }
    }
}
