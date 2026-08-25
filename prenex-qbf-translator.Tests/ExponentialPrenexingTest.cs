
using Xunit;
using prenex_qbf_translator.Parsing;
using prenex_qbf_translator.Language;
using prenex_qbf_translator.ExponentialPrenexing;

namespace prenex_qbf_translator.Tests
{
    public class ExponentialPrenexingTest
    {
        private void TestFormula(string formula, string prenexedFormula)
        {
            string expected = new Parser(new Scanner(prenexedFormula)).Parse().ToString(); // to standardize (whitespace and parentheses)

            Parser p = new(new Scanner(formula));
            IFormula f = p.Parse();
            IFormula prenexed = new ExponentialPrenexer().Prenexed(f);
            string actual = prenexed.ToString();

            Assert.Equal(expected, actual);
        }


        [Fact]
        public void TestBooleanFormulas() // no quantifiers, no prenexing required
        {
            TestFormula("a", "a");
            TestFormula("!a", "!a");
            TestFormula("a & b", "a&b");
            TestFormula("a | b", "a | b");
            TestFormula("a -> b", "a -> b");
            TestFormula("a <- b", "a <- b");
            TestFormula("a <-> b", "a <-> b");
            TestFormula("!(a & b) | c <- !(d | e) <-> f",
                        "!(a & b) | c <- !(d | e) <-> f");
            TestFormula("a & b & c | d | e & f -> g | h & i <-> j",
                        "a & b & c | d | e & f -> g | h & i <-> j");
        }

        [Fact]
        public void Not()
        {
            TestFormula("!?a a", "#a !a");
            TestFormula("!#a a", "?a !a");

            TestFormula("!?a !a", "#a a");
            TestFormula("!#a !a", "?a a");

            TestFormula("!?a ?b a", "#a #b !a");
            TestFormula("!#a ?b a", "?a #b !a");
            TestFormula("!?a #b a", "#a ?b !a");
            TestFormula("!#a #b a", "?a ?b !a");

            TestFormula("?a !?b a", "?a #b !a");
            TestFormula("#a !?b a", "#a #b !a");
            TestFormula("?a !#b a", "?a ?b !a");
            TestFormula("#a !#b a", "#a ?b !a");

            TestFormula("!?a !?b a", "#a ?b a");
            TestFormula("!#a !?b a", "?a ?b a");
            TestFormula("!?a !#b a", "#a #b a");
            TestFormula("!#a !#b a", "?a #b a");

            TestFormula("!?a ?b !#c #d a", "#a #b #c #d a");
            TestFormula("!#a #b #c !?d ?e ?f !a", "?a ?b ?c ?d ?e ?f !a");
            TestFormula("#a #b ?c !?d ?e !?f ?g !#h !#i #j !a", "#a #b ?c #d #e ?f ?g ?h #i #j !a");
        }



        [Fact]
        public void And_2operands()
        {
            TestFormula("?a a & b", "?a (a & b)");
            TestFormula("#a a & b", "#a (a & b)");
            TestFormula("a & ?b b", "?b (a & b)");
            TestFormula("a & #b b", "#b (a & b)");
            TestFormula("?a a & ?b b", "?a ?b (a & b)");
            TestFormula("?a a & #b b", "?a #b (a & b)");
            TestFormula("#a a & ?b b", "#a ?b (a & b)");
            TestFormula("#a a & #b b", "#a #b (a & b)");

        }

        [Fact]
        public void And_3operandsExists()
        {
            TestFormula("?a a & b & c",  "?a (a & b & c)");
            TestFormula("a & ?b b & c",  "?b (a & b & c)");
            TestFormula("a & b & ? c c",  "?c (a & b & c)");
            TestFormula("?a a & ?b b & c",  "?a ?b (a & b & c)");
            TestFormula("?a a & b & ?c c",  "?a ?c (a & b & c)");
            TestFormula("a & ?b b & ?c c",  "?b ?c (a & b & c)");
            TestFormula("?a a ?b b ?c c",  "?a ?b ?c (a & b & c)");
        }

        [Fact]
        public void And_3operandsForall()
        {
            TestFormula("#a a & b & c",  "#a (a & b & c)");
            TestFormula("a & #b b & c",  "#b (a & b & c)");
            TestFormula("a & b & # c c", "#c (a & b & c)");
            TestFormula("#a a & #b b & c",  "#a #b (a & b & c)");
            TestFormula("#a a & b & #c c",  "#a #c (a & b & c)");
            TestFormula("a & #b b & #c c",  "#b #c (a & b & c)");
            TestFormula("#a a & #b b #c c",  "#a #b #c (a & b & c)");
        }

        [Fact]
        public void And_3operandsMixed()
        {
            TestFormula("?a a & #b b & c",  "?a #b (a & b & c)");
            TestFormula("?a a & b & #c c",  "?a #c (a & b & c)");
            TestFormula("a & ?b b & #c c",  "?b #c (a & b & c)");

            TestFormula("#a a & ?b b & c",  "#a ?b (a & b & c)");
            TestFormula("#a a & b & ?c c",  "#a ?c (a & b & c)");
            TestFormula("a & #b b & ?c c",  "#b ?c (a & b & c)");

            TestFormula("?a a & ?b b & #c c",  "?a #b ?c (a & b & c)");
            TestFormula("?a a & #b b & ?c c",  "?a #b ?c (a & b & c)");
            TestFormula("?a a & #b b & #c c",  "?a #b #c (a & b & c)");
            TestFormula("#a a & ?b b & ?c c",  "#a ?b ?c (a & b & c)");
            TestFormula("#a a & ?b b & #c c",  "#a ?b #c (a & b & c)");
            TestFormula("#a a & #b b & ?c c",  "#a #b ?c (a & b & c)");
        }

        [Fact]
        public void And_ReoccurringVariables()
        {
            TestFormula("?a a & a",  "?ap (ap & a)");
        }
    }
}
