
using prenex_qbf_translator.Language;
using prenex_qbf_translator.Parsing;
using Xunit;

namespace prenex_qbf_translator.Tests
{
    public class LanguageTest
    {


        [Fact]
        public void NoBoundVariables()
        {
            string s =
                "a&b&c&" +
                "(" +
                "  d -> e | !f" +
                ")";

            IFormula f = new Parser(s).Parse();

            Assert.Equal("abcdef", string.Join("", f.Variables()));
            Assert.Equal("", string.Join("", f.BoundVariables()));
            Assert.Equal("abcdef", string.Join("", f.FreeVariables()));
        }

        [Fact]
        public void OneVariableBound()
        {
            string s =
                "#a" +
                "(" +
                "  a -> b | c" +
                ")";

            IFormula f = new Parser(s).Parse();

            Assert.Equal("abc", string.Join("", f.Variables()));
            Assert.Equal("a", string.Join("", f.BoundVariables()));
            Assert.Equal("bc", string.Join("", f.FreeVariables()));
        }

        [Fact]
        public void BoundVariableUsedOnlyInNestedScope()
        {
            string s =
                "x &" +
                "(" +
                "  #a" +
                "  (" +
                "    a & b & x" +
                "  )" +
                ")";

            IFormula f = new Parser(s).Parse();

            Assert.Equal("xab", string.Join("", f.Variables()));
            Assert.Equal("a", string.Join("", f.BoundVariables()));
            Assert.Equal("xb", string.Join("", f.FreeVariables()));
        }

        [Fact]
        public void VariableOccursBothBoundAndFree()
        {
            string s =
                "a&" +
                "(" +
                "  #a" +
                "  (" +
                "    a & b" +
                "  )" +
                ")";

            IFormula f = new Parser(s).Parse();

            Assert.Equal("ab", string.Join("", f.Variables()));
            Assert.Equal("a", string.Join("", f.BoundVariables()));
            Assert.Equal("ab", string.Join("", f.FreeVariables()));
        }

        [Fact]
        public void NestedBinders()
        {
            string s =
                "x&" +
                "(" +
                "  #a" +
                "  (" +
                "    a & b &" +
                "    (" +
                "      #b" +
                "      (" +
                "        a & b & c" +
                "      )" +
                "    )" +
                "  )" +
                ")";

            IFormula f = new Parser(s).Parse();

            Assert.Equal("xabc", string.Join("", f.Variables()));
            Assert.Equal("ab", string.Join("", f.BoundVariables()));
            Assert.Equal("xbc", string.Join("", f.FreeVariables()));
        }

        [Fact]
        public void RepeatedVariableOccurrences()
        {
            string s =
                "a&a&a&" +
                "(" +
                "  #b" +
                "  (" +
                "    b & b & a & a" +
                "  )" +
                ")";

            IFormula f = new Parser(s).Parse();

            Assert.Equal("ab", string.Join("", f.Variables()));
            Assert.Equal("b", string.Join("", f.BoundVariables()));
            Assert.Equal("a", string.Join("", f.FreeVariables()));
        }

        [Fact]
        public void BinderDoesNotBindOutsideItsScope()
        {
            string s =
                "a&" +
                "(" +
                "  #b" +
                "  (" +
                "    b & c" +
                "  )" +
                ")" +
                "&b";

            IFormula f = new Parser(s).Parse();

            Assert.Equal("abc", string.Join("", f.Variables()));
            Assert.Equal("b", string.Join("", f.BoundVariables()));
            Assert.Equal("acb", string.Join("", f.FreeVariables()));
        }

        [Fact]
        public void OperatorsDoNotAffectBinding()
        {
            string s =
                "#a" +
                "(" +
                "  a -> b | !a <-> c <- a" +
                ")";

            IFormula f = new Parser(s).Parse();

            Assert.Equal("abc", string.Join("", f.Variables()));
            Assert.Equal("a", string.Join("", f.BoundVariables()));
            Assert.Equal("bc", string.Join("", f.FreeVariables()));
        }

        [Fact]
        public void MultipleBinders()
        {
            string s =
                "#a#b#c" +
                "(" +
                "  a & b & c & d" +
                ")";

            IFormula f = new Parser(s).Parse();

            Assert.Equal("abcd", string.Join("", f.Variables()));
            Assert.Equal("abc", string.Join("", f.BoundVariables()));
            Assert.Equal("d", string.Join("", f.FreeVariables()));
        }

        [Fact]
        public void BinderWithVariableNeverUsed()
        {
            string s =
                "#a" +
                "(" +
                "  b & c" +
                ")";

            IFormula f = new Parser(s).Parse();

            Assert.Equal("abc", string.Join("", f.Variables()));
            Assert.Equal("a", string.Join("", f.BoundVariables()));
            Assert.Equal("bc", string.Join("", f.FreeVariables()));
        }

        [Fact]
        public void ComplexNestedScopes()
        {
            string s =
                "x&y&" +
                "(" +
                "  #a" +
                "  (" +
                "    a & b & x &" +
                "    (" +
                "      #c" +
                "      (" +
                "        a & c & d & y" +
                "      )" +
                "    )" +
                "  )" +
                ")" +
                "&z";

            IFormula f = new Parser(s).Parse();

            Assert.Equal("xyabcdz", string.Join("", f.Variables()));
            Assert.Equal("ac", string.Join("", f.BoundVariables()));
            Assert.Equal("xybdz", string.Join("", f.FreeVariables()));
        }
        [Fact]

        public void Complex2()
        {
            string s =
                "a&x&y&z&c & #a#b" +
                "(" +
                "  b&c&d&e & !?b?c#d" +
                "  (" +
                "    a -> b | !c <-> e <- f" +
                "  )" +
                ")";
            IFormula f = new Parser(s).Parse();
            Assert.Equal("axyzcbdef", string.Join("", f.Variables()));
            Assert.Equal("abcd", string.Join("", f.BoundVariables()));
            Assert.Equal("axyzcdef", string.Join("", f.FreeVariables()));
        }
    }
}
