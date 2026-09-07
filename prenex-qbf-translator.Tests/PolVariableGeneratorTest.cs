using prenex_qbf_translator.PolynomialPrenexing;
using prenex_qbf_translator.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using Xunit;

namespace prenex_qbf_translator.Tests
{
    public class PolVariableGeneratorTest
    {
        private void TestNextP(List<string> unav, params string[] generatedNames)
        {
            var gen = new PolVariableGenerator(unav.Select(v => new Variable(v)).ToHashSet());

            for (int i = 0; i < generatedNames.Length; i++)
            {
                Variable v = gen.NextP();
                Assert.Equal(generatedNames[i], v.Name);
            }
        }

        private void TestNextPositiveAndNegative(string name, List<string> unav, params (string, string)[] expectedNames)
        {
            var gen = new PolVariableGenerator(unav.Select(v => new Variable(v)).ToHashSet());

            for (int i = 0; i < expectedNames.Length; i++)
            {
                var actual = gen.NextPositiveAndNegative(new Variable(name));
                (string expectedPlus, string expectedMinus) = expectedNames[i];

                Assert.Equal(expectedPlus, actual.P.Name);
                Assert.Equal(expectedMinus, actual.N.Name);
            }
        }


        [Fact]
        public void NextP()
        {
            TestNextP(unav: [], "p1", "p2", "p3", "p4");
            TestNextP(unav: ["p2"], "p1", "p3", "p4");
            TestNextP(unav: ["p1"], "p2", "p3", "p4");
            TestNextP(unav: ["p2", "p3", "p7", "p8", "p11"], "p1", "p4", "p5", "p6", "p9", "p10", "p12", "p13");

            TestNextP(unav: ["p0", "P1"], "p1", "p2", "p3", "p4");
        }

        [Fact]
        public void BasicSequence()
        {
            TestNextPositiveAndNegative("a", [], ("ap", "am"), ("a1p", "a1m"), ("a2p", "a2m"), ("a3p", "a3m"));
            TestNextPositiveAndNegative("a1", [], ("ap", "am"), ("a1p", "a1m"), ("a2p", "a2m"), ("a3p", "a3m"));
            TestNextPositiveAndNegative("a2", [], ("ap", "am"), ("a1p", "a1m"), ("a2p", "a2m"), ("a3p", "a3m"));
            TestNextPositiveAndNegative("a123", [], ("ap", "am"), ("a1p", "a1m"), ("a2p", "a2m"), ("a3p", "a3m"));
        }

        [Fact]
        public void NextPositiveAndNegative_TakenPlusMinus()
        {
            TestNextPositiveAndNegative("a", ["ap"],
                ("a1p", "a1m"), ("a2p", "a2m"));
            TestNextPositiveAndNegative("a", ["am"],
                ("a1p", "a1m"), ("a2p", "a2m"));
            TestNextPositiveAndNegative("a", ["ap", "am"],
                ("a1p", "a1m"), ("a2p", "a2m"));

            TestNextPositiveAndNegative("a", ["a1p"],
                ("ap", "am"), ("a2p", "a2m"), ("a3p", "a3m"));
            TestNextPositiveAndNegative("a", ["a2m"],
                ("ap", "am"), ("a1p", "a1m"), ("a3p", "a3m"));
            TestNextPositiveAndNegative("a", ["a3p", "a3m"],
                ("ap", "am"), ("a1p", "a1m"), ("a2p", "a2m"), ("a4p", "a4m"));

            TestNextPositiveAndNegative("a", ["ap", "a2m", "a4p", "a4m"],
                ("a1p", "a1m"), ("a3p", "a3m"), ("a5p", "a5m"));
        }

        [Fact]
        public void LongVariables()
        {
            TestNextPositiveAndNegative("var", [], ("varp", "varm"), ("var1p", "var1m"), ("var2p", "var2m"));
            TestNextPositiveAndNegative("var2", [], ("varp", "varm"), ("var1p", "var1m"), ("var2p", "var2m"));
            TestNextPositiveAndNegative("var345", [], ("varp", "varm"), ("var1p", "var1m"), ("var2p", "var2m"));
        }

        [Fact]
        public void VariablesStartingWithDigit()
        {
            TestNextPositiveAndNegative("1", [], ("1p", "1m"), ("11p", "11m"), ("12p", "12m"));
            TestNextPositiveAndNegative("5", [], ("5p", "5m"), ("51p", "51m"), ("52p", "52m"));
            TestNextPositiveAndNegative("13", [], ("1p", "1m"), ("11p", "11m"), ("12p", "12m"), ("13p", "13m"));
        }
    }
}
