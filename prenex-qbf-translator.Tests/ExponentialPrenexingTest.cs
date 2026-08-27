
using Xunit;
using prenex_qbf_translator.Parsing;
using prenex_qbf_translator.Language;
using prenex_qbf_translator.ExponentialPrenexing;
using System.Diagnostics.Contracts;

namespace prenex_qbf_translator.Tests
{
    public class ExponentialPrenexingTest
    {
        private void TestFormula(string formula, string prenexedFormula)
        {
            string expected = new Parser(prenexedFormula).Parse().ToString(); // to standardize (whitespace and parentheses)

            Parser p = new(formula);
            IFormula f = p.Parse();
            IFormula prenexed = new ExponentialPrenexer().Prenexed(f);
            string actual = prenexed.ToString();

            Assert.Equal(expected, actual);
        }


        [Fact]
        public void ComplexBooleanFormulas() // no quantifiers, no prenexing required
        {
            TestFormula("!(a & b) | c <- !(d | e) <-> f",
                        "!(a & b) | c <- !(d | e) <-> f");
            TestFormula("a & b & c | d | e & f -> g | h & i <-> j",
                        "a & b & c | d | e & f -> g | h & i <-> j");
        }

        [Fact]
        public void Not_Simple()
        {
            TestFormula("!a", "!a");

            TestFormula("!#a a", "?a !a");
            TestFormula("!?a a", "#a !a");

            TestFormula("!?a !a", "#a a");
            TestFormula("!#a !a", "?a a");
        }
            
        [Fact]
        public void And_Simple()
        {
            TestFormula("a & b", "a & b");
            TestFormula("a & b", "a & b");

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
        public void Or_Simple()
        {
            TestFormula("a | b", "a | b");
            TestFormula("a | b", "a | b");

            TestFormula("?a a | b", "?a (a | b)");
            TestFormula("#a a | b", "#a (a | b)");
            TestFormula("a | ?b b", "?b (a | b)");
            TestFormula("a | #b b", "#b (a | b)");
            TestFormula("?a a | ?b b", "?a ?b (a | b)");
            TestFormula("?a a | #b b", "?a #b (a | b)");
            TestFormula("#a a | ?b b", "#a ?b (a | b)");
            TestFormula("#a a | #b b", "#a #b (a | b)");

        }

        [Fact]
        public void Implies_Simple()
        {
            TestFormula("a -> b", "a -> b");
            TestFormula("a -> b", "a -> b");

            TestFormula("?a a -> b", "#a (a -> b)");
            TestFormula("#a a -> b", "?a (a -> b)");
            TestFormula("a -> ?b b", "?b (a -> b)");
            TestFormula("a -> #b b", "#b (a -> b)");
            TestFormula("?a a -> ?b b", "#a ?b (a -> b)");
            TestFormula("?a a -> #b b", "#a #b (a -> b)");
            TestFormula("#a a -> ?b b", "?a ?b (a -> b)");
            TestFormula("#a a -> #b b", "?a #b (a -> b)");
        }

        [Fact]
        public void IsImpliedBy_Simple()
        {
            TestFormula("a <- b", "a <- b");
            TestFormula("a <- b", "a <- b");

            TestFormula("?a a <- b", "?a (a <- b)");
            TestFormula("#a a <- b", "#a (a <- b)");
            TestFormula("a <- ?b b", "#b (a <- b)");
            TestFormula("a <- #b b", "?b (a <- b)");
            TestFormula("?a a <- ?b b", "?a #b (a <- b)");
            TestFormula("?a a <- #b b", "?a ?b (a <- b)");
            TestFormula("#a a <- ?b b", "#a #b (a <- b)");
            TestFormula("#a a <- #b b", "#a ?b (a <- b)");
        }

        [Fact]
        public void Equivalent_Simple()
        {
            TestFormula("a <-> b", "a <-> b");
            TestFormula("a <-> b", "a <-> b");

            TestFormula("?a a <-> b", "?a #ap (a & b | !ap & !b)");
            TestFormula("#a a <-> b", "#a ?ap (a & b | !ap & !b)");
            TestFormula("a <-> ?b b", "?b #bp (a & b | !a & !bp)");
            TestFormula("a <-> #b b", "#b ?bp (a & b | !a & !bp)");
            TestFormula("?a a <-> ?b b", "?a ?b #ap #bp (a & b | !ap & !bp)");
            TestFormula("?a a <-> #b b", "?a #b #ap ?bp (a & b | !ap & !bp)");
            TestFormula("#a a <-> ?b b", "#a ?b ?ap #bp (a & b | !ap & !bp)");
            TestFormula("#a a <-> #b b", "#a #b ?ap ?bp (a & b | !ap & !bp)");
        }

        [Fact]
        public void ReoccuringVariables()
        {
            TestFormula("?a a <-> ?a a", "?a ?ap #ap1 #app (a & ap | !ap1 & !app)");
            TestFormula("?a?b?e (a&b&c&d&e) | #b#c#f (a&b&c&d&f)", "?a ?b ?e #bp #cp #f (a & b & c & d & e | ap & bp & cp & d & f)");
        }

        [Fact]
        public void And_ReoccurringVariables()//some are broken cus renaming changes, change!!!
        {
            TestFormula("?a a & a", "?ap (ap & a)");
            TestFormula("#a a & a", "#ap (ap & a)");
            TestFormula("a & ?a a", "?ap (a & ap)");
            TestFormula("a & #a a", "#ap (a & ap)");
            TestFormula("?a a & ?a a", "?ap ?a (ap & a)");
            TestFormula("#a a & #a a", "#ap #a (ap & a)");
            TestFormula("?a a & #a a", "?ap #a (ap & a)");
            TestFormula("#a a & ?a a", "#ap ?a (ap & a)");
            TestFormula("?a a & ?a a & ?a a", "?ap1 ?ap ?a (ap1 & ap & a)");
            TestFormula("#a a & #a a & #a a", "#ap1 #ap #a (ap1 & ap & a)");
            TestFormula("?a a & ?a a & ?a a & ?a a & ?a a & ?a a", "?ap4 ?ap3 ?ap2 ?ap1 ?ap ?a (ap4 & ap3 & ap2 & ap1 & ap & a)");
            TestFormula("#a a & #a a & #a a & #a a & #a a & #a a", "#ap4 #ap3 #ap2 #ap1 #ap #a (ap4 & ap3 & ap2 & ap1 & ap & a)");
            TestFormula("?a a & #a a & #a a & ?a a & ?a a & #a a", "?ap4 #ap3 #ap2 ?ap1 ?ap #a (ap4 & ap3 & ap2 & ap1 & ap & a)");

            TestFormula("a & #a ?a (a & #a ?a (a & #a ?a a))", "#ap2 ?ap2 #ap1 ?ap1 #ap ?ap (a & ap2 & ap1 &ap)");
            TestFormula("a & #a ((a & ?a a) & (a& #a a))", "#ap1 ?app #ap (a & ap1 & app & ap1 & ap)");
        }
    }
}
