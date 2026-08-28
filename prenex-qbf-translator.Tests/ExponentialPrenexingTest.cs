
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
        public void AlreadyPrenexed()
        {
            TestFormula("?a a", "?a a");
            TestFormula("#a !a", "#a !a");
            TestFormula("?a ?b (a & b)", "?a ?b (a & b)");
            TestFormula("?a #b (a | b)", "?a #b (a | b)");
            TestFormula("#a ?b (a -> b)", "#a ?b (a -> b)");
            TestFormula("#a #b (a <- b)", "#a #b (a <- b)");
            TestFormula("#a #b (a <-> b)", "#a #b (a <-> b)");
            TestFormula("#a #b #c ?d ?e #f (a & b <- c <-> d -> e | f)", "#a #b #c ?d ?e #f (a & b <- c <-> d -> e | f)");
        }

        [Fact]
        public void Not()
        {
            TestFormula("!a", "!a");

            TestFormula("!#a a", "?a !a");
            TestFormula("!?a a", "#a !a");

            TestFormula("!?a !a", "#a a");
            TestFormula("!#a !a", "?a a");
        }
            
        [Fact]
        public void And()
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
        public void Or()
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
        public void Implies()
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
        public void IsImpliedBy()
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
        public void Equivalent()
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
        public void NestedQuantifiers()
        {
            TestFormula("!?a ?b #c #d ?e (a <- b <-> c -> (d & e))", "#a #b ?c ?d #e !(a <- b <-> c -> (d & e))");
            TestFormula("(#a #b #c (a&b&c) | ?d ?e ?f ?g (d&e&f&g))", "#a#b#c?d?e?f?g (a&b&c|d&e&f&g)");
            TestFormula("(#a ?b ?c (a&b&c) | ?d ?e #f #g (d&e&f&g))", "#a?b?c?d?e#f#g (a&b&c|d&e&f&g)");
            TestFormula("(#a ?b ?c (a&b&c) -> ?d ?e #f #g (d&e&f&g))", "?a#b#c?d?e#f#g (a&b&c -> d&e&f&g)");
            TestFormula("(#a ?b ?c (a&b&c) <- ?d ?e #f #g (d&e&f&g))", "#a?b?c#d#e?f?g (a&b&c <- d&e&f&g)");
            TestFormula("(#a ?b ?c (a|b|c) <-> ?d ?e #f #g (d|e|f|g))", "#a ?b ?c ?d ?e #f #g  ?ap #bp #cp #dp #ep ?fp ?gp  ((a|b|c) & (d|e|f|g) | !(ap|bp|cp) & !(dp|ep|fp|gp))");
        }

        [Fact]
        public void VariableRenaming()
        {
            TestFormula("?a a & ?a a", "?a ?ap (a & ap)");
            TestFormula("?v1 v1 | #v1 v1", "?v1 #v1p (v1 | v1p)");
            TestFormula("#a a -> ?a a", "?a ?ap (a -> ap)");
            TestFormula("#a a <- #a a", "#a ?ap (a <- ap)");
            TestFormula("?a a <-> ?a a", "?a ?ap #ap1 #app (a & ap | !ap1 & !app)");

            TestFormula("((#a a  &  #a a)  &  #a a)  &  #a a", "#a #ap #ap1 #ap2 (a & ap & ap1 &ap2)");

            TestFormula("?a?b?e (a&b&c&d&e) | #b#c#f (a&b&c&d&f)", "?a ?b ?e #bp #cp #f (a & b & c & d & e | ap & bp & cp & d & f)");
        }

        [Fact]
        public void ComplexFormulas()
        {
            TestFormula("#x (a & !#b (!b -> (c | ?d d))  <-  (!c | a | ?f b))",    "#x ?b #d #f ((a & !(!b -> c | d))  <-  (!c | a | bp))");
        }

        [Fact]
        public void TwoNestedEquivalences()
        {
            TestFormula(
                "(?a a <-> ?b b) <-> ?c c"
                ,
                "?a ?b #ap #bp ?c  #ap1 #bp1 ?app ?bpp #cp" +
                "(" +
                "  (a & b | !ap & !bp) & c" +
                "  |" +
                "  !(ap1 & bp1 | !app & !bpp) & !cp" +
                ")"
                );
            TestFormula(
                "?a a <-> (?b b <-> ?c c)"
                ,
                "?a ?b ?c #bp #cp  #ap #bp1 #cp1 ?bpp ?cpp" +
                "(" +
                "  a & (b & c | !bp & !cp) " +
                "  |" +
                "  !ap & !(bp1 & cp1 | !bpp & !cpp)" +
                ")"
                );
        }

        [Fact]
        public void ThreeNestedEquivalences()
        {
            TestFormula(
                "(?a a <-> ?b b) <-> (?c c <-> ?d d)"
                ,

                "?a ?b #ap #bp ?c ?d #cp #dp  #ap1 #bp1 ?app ?bpp #cp1 #dp1 ?cpp ?dpp" +
                "(" +
                "  (a & b | !ap & !bp) &" +
                "  (c & d | !cp & !dp) |" +
                "  !(ap1 & bp1 | !app & !bpp) &" +
                "  !(cp1 & dp1 | !cpp & !dpp)" +
                ")"
                );

            TestFormula(
                "((?a a <-> ?b b) <-> ?c c) <-> ?d d"
                ,

                "?a ?b #ap #bp ?c  " +
                "#ap1 #bp1 ?app ?bpp #cp" +
                "?d" +
                "#ap2 #bp2 ?app1 ?bpp1 #cp1" +
                "?ap1p ?bp1p #appp #bppp ?cpp" +
                "#dp" +
                "(" +
                "  (" +
                "    (a & b | !ap & !bp) & c |" +
                "    !(ap1 & bp1 | !app & !bpp) & !cp" +
                "  )" +
                "  &" +
                "  d" +
                "  |" +
                "  !" +
                "  (" +
                "    (ap2 & bp2 | !app1 & !bpp1) & cp1 |" +
                "    !(ap1p & bp1p | !appp & !bppp) & !cpp" +
                "  )" +
                "  &" +
                "  !dp" +
                ")"
                );
        }



        [Fact]
        public void HardFormulaTimeTest()
        {
            string s = "a <-> (b <-> (c <-> (d <-> (e <-> (f <-> (g <-> (h <-> (i <-> (j <-> ?x x )))) )))) )";
            var formula = new Parser(s).Parse();
            new ExponentialPrenexer().Prenexed(formula);
        }
    }
}
