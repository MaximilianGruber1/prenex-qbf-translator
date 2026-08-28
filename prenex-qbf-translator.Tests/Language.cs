
using prenex_qbf_translator.Language;
using prenex_qbf_translator.Parsing;
using System.Linq;
using Xunit;

namespace prenex_qbf_translator.Tests
{
    public class Language
    {
        private void Test(string formula, string vars, string boundVars, string freeVars)
        {
            IFormula f = new Parser(formula).Parse();

            var actualVars = f.Variables();
            string actualVarsString = string.Join("", actualVars.OrderBy(v => v.Name));
            Assert.Equal(vars, actualVarsString);

            var actualBoundVars = f.BoundVariables();
            string actualBoundVarsString = string.Join("", actualBoundVars.OrderBy(v => v.Name));
            Assert.Equal(boundVars, actualBoundVarsString);

            var actualFreeVars = f.FreeVariables();
            string actualFreeVarsString = string.Join("", actualFreeVars.OrderBy(v => v.Name));
            Assert.Equal(freeVars, actualFreeVarsString);
        }

        [Fact]
        public void NoBoundVariables()
        {
            string s =
                "a&b&c&" +
                "(" +
                "  d -> e | !f" +
                ")";

            Test(s, "abcdef", "", "abcdef");
        }

        [Fact]
        public void OneVariableBound()
        {
            string s =
                "#a" +
                "(" +
                "  a -> b | c" +
                ")";

            Test(s, "abc", "a", "bc");
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

            Test(s, "abx", "a", "bx");
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

            Test(s, "ab", "a", "ab");
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

            Test(s, "abcx", "ab", "bcx");
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

            Test(s, "ab", "b", "a");
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

            Test(s, "abc", "b", "abc");
        }

        [Fact]
        public void OperatorsDoNotAffectBinding()
        {
            string s =
                "#a" +
                "(" +
                "  a -> b | !a <-> c <- a" +
                ")";

            Test(s, "abc", "a", "bc");
        }

        [Fact]
        public void MultipleBinders()
        {
            string s =
                "#a#b#c" +
                "(" +
                "  a & b & c & d" +
                ")";

            Test(s, "abcd", "abc", "d");
        }

        [Fact]
        public void BinderWithVariableNeverUsed()
        {
            string s =
                "#a" +
                "(" +
                "  b & c" +
                ")";

            Test(s, "abc", "a", "bc");
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

            Test(s, "abcdxyz", "ac", "bdxyz");
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

            Test(s, "abcdefxyz", "abcd", "acdefxyz");
        }
    }
}
