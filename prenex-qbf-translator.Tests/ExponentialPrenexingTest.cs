
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
            TestFormula("?a a <-> b", "?a #a1 (a & b | !a1 & !b)");
            TestFormula("#a a <-> b", "#a ?a1 (a & b | !a1 & !b)");
            TestFormula("a <-> ?b b", "?b #b1 (a & b | !a & !b1)");
            TestFormula("a <-> #b b", "#b ?b1 (a & b | !a & !b1)");
            TestFormula("?a a <-> ?b b", "?a ?b #a1 #b1 (a & b | !a1 & !b1)");
            TestFormula("?a a <-> #b b", "?a #b #a1 ?b1 (a & b | !a1 & !b1)");
            TestFormula("#a a <-> ?b b", "#a ?b ?a1 #b1 (a & b | !a1 & !b1)");
            TestFormula("#a a <-> #b b", "#a #b ?a1 ?b1 (a & b | !a1 & !b1)");
        }

        [Fact]
        public void NestedQuantifiers()
        {
            TestFormula("!?a ?b #c #d ?e (a <- b <-> c -> (d & e))", "#a #b ?c ?d #e !(a <- b <-> c -> (d & e))");
            TestFormula("(#a #b #c (a&b&c) | ?d ?e ?f ?g (d&e&f&g))", "#a#b#c?d?e?f?g (a&b&c|d&e&f&g)");
            TestFormula("(#a ?b ?c (a&b&c) | ?d ?e #f #g (d&e&f&g))", "#a?b?c?d?e#f#g (a&b&c|d&e&f&g)");
            TestFormula("(#a ?b ?c (a&b&c) -> ?d ?e #f #g (d&e&f&g))", "?a#b#c?d?e#f#g (a&b&c -> d&e&f&g)");
            TestFormula("(#a ?b ?c (a&b&c) <- ?d ?e #f #g (d&e&f&g))", "#a?b?c#d#e?f?g (a&b&c <- d&e&f&g)");
            TestFormula("(#a ?b ?c (a|b|c) <-> ?d ?e #f #g (d|e|f|g))", "#a ?b ?c ?d ?e #f #g  ?a1 #b1 #c1 #d1 #e1 ?f1 ?g1  ((a|b|c) & (d|e|f|g) | !(a1|b1|c1) & !(d1|e1|f1|g1))");
        }

        [Fact]
        public void VariableRenaming()
        {
            TestFormula("?a a & ?a a", "?a ?a1 (a & a1)");
            TestFormula("?v1 v1 | #v1 v1", "?v1 #v2 (v1 | v2)");
            TestFormula("#a a -> ?a a", "?a ?a1 (a -> a1)");
            TestFormula("#a a <- #a a", "#a ?a1 (a <- a1)");
            TestFormula("?a a <-> ?a a", "?a ?a1 #a3 #a2 (a & a1 | !a3 & !a2)");

            TestFormula("((#a a  &  #a a)  &  #a a)  &  #a a", "#a #a1 #a2 #a3 (a & a1 & a2 & a3)");

            TestFormula("?a?b?e (a&b&c&d&e) | #b#c#f (a&b&c&d&f)", "?a ?b ?e #b1 #c1 #f (a & b & c & d & e | a1 & b1 & c1 & d & f)");
        }

        [Fact]
        public void ComplexFormulas()
        {
            TestFormula("#x (a & !#b (!b -> (c | ?d d))  <-  (!c | a | ?f b))", "#x ?b #d #f ((a & !(!b -> c | d))  <-  (!c | a | b1))");
        }

        [Fact]
        public void TwoNestedEquivalences()
        {
            TestFormula(
                "(?a a <-> ?b b) <-> ?c c"
                ,
                "?a ?b #a1 #b1 ?c  #a2 #b2 ?a3 ?b3 #c1" +
                "(" +
                "  (a & b | !a1 & !b1) & c" +
                "  |" +
                "  !(a2 & b2 | !a3 & !b3) & !c1" +
                ")"
                );
            TestFormula(
                "?a a <-> (?b b <-> ?c c)"
                ,
                "?a ?b ?c #b1 #c1  #a1 #b2 #c2 ?b3 ?c3" +
                "(" +
                "  a & (b & c | !b1 & !c1) " +
                "  |" +
                "  !a1 & !(b2 & c2 | !b3 & !c3)" +
                ")"
                );
        }

        [Fact]
        public void ThreeNestedEquivalences()
        {
            TestFormula(
                "(?a a <-> ?b b) <-> (?c c <-> ?d d)"
                ,

                "?a ?b #a1 #b1 ?c ?d #c1 #d1  #a2 #b2 ?a3 ?b3 #c2 #d2 ?c3 ?d3" +
                "(" +
                "  (a & b | !a1 & !b1) &" +
                "  (c & d | !c1 & !d1) |" +
                "  !(a2 & b2 | !a3 & !b3) &" +
                "  !(c2 & d2 | !c3 & !d3)" +
                ")"
                );

            TestFormula(
                "((?a a <-> ?b b) <-> ?c c) <-> ?d d"
                ,

                "?a ?b #a1 #b1 ?c  " +
                "#a2 #b2 ?a3 ?b3 #c1" +
                "?d" +
                "#a4 #b4 ?a5 ?b5 #c2" +
                "?a6 ?b6 #a7 #b7 ?c3" +
                "#d1" +
                "(" +
                "  (" +
                "    (a & b | !a1 & !b1) & c |" +
                "    !(a2 & b2 | !a3 & !b3) & !c1" +
                "  )" +
                "  &" +
                "  d" +
                "  |" +
                "  !" +
                "  (" +
                "    (a4 & b4 | !a5 & !b5) & c2 |" +
                "    !(a6 & b6 | !a7 & !b7) & !c3" +
                "  )" +
                "  &" +
                "  !d1" +
                ")"
                );
        }

        [Fact]
        public void EquivalenceWithoutQuantifiersInSubformulas()
        {
            TestFormula("a <-> b", "a <-> b");
            TestFormula("!#a (a <-> b)", "?a !(a <-> b)");
            TestFormula("#c ?d (c <-> d) & ?e ?f (e <-> f)", "#c ?d ?e ?f ((c <-> d) & (e <-> f))");
        }

        [Fact]
        public void _222_Simplified_Seed0()
        {
            TestFormula(
                "?a ?b #c #d (b <-> d | a & c <-> h | e & !f & g) <-> " +
                "#a #b ?c ?d (c & d & (!a <-> b) <-> (!f <-> !g) & !e & !h)"
                ,

                "?a ?b #c #d " +
                "#a1 #b1 ?c1 ?d1 " +
                "#a3 #b3 ?c3 ?d3 " +
                "?a2 ?b2 #c2 #d2 " +
                "(" +
                "  (b <-> d | a & c <-> h | e & !f & g)  &" +
                "  (c1 & d1 & (!a1 <-> b1) <-> (!f <-> !g) & !e & !h)  |" +
                "  !(b3 <-> d3 | a3 & c3 <-> h | e & !f & g)  &" +
                "  !(c2 & d2 & (!a2 <-> b2) <-> (!f <-> !g) & !e & !h)" +
                ")"
                );
        }

        [Fact]
        public void _213_Simplified_Seed4()
        {
            TestFormula(
                "#a ?b (a <- b <-> !(!c <-> !d)) <-> " +
                "?a #b (a & b <-> !(!c <- d)) <-> " +
                "#a ?b (!(a -> !b) <-> !(!c & !d))"
                ,

                "#a ?b ?a4 #b4 #a1 ?b1 #a3 ?b3 ?a2 #b2" +
                "?a6 #b6 #a5 ?b5 ?a7 #b7 ?a9 #b9 #a8 ?b8" +
                "(" +
                "  (" +
                "    (a <- b <-> !(!c <-> !d))" +
                "    &" +
                "    (" +
                "      (a4 & b4 <-> !(!c <- d)) &" +
                "      (!(a1 -> !b1) <-> !(!c & !d)) |" +
                "      !(a3 & b3 <-> !(!c <- d)) &" +
                "      !(!(a2 -> !b2) <-> !(!c & !d))" +
                "    )" +
                "  )" +
                "  |" +
                "  (" +
                "    !(a6 <- b6 <-> !(!c <-> !d))" +
                "    &" +
                "    !(" +
                "      (a5 & b5 <-> !(!c <- d)) &" +
                "      (!(a7 -> !b7) <-> !(!c & !d)) |" +
                "      !(a9 & b9 <-> !(!c <- d)) &" +
                "      !(!(a8 -> !b8) <-> !(!c & !d))" +
                "    )" + 
                "  )" +
                ")"
                );
        }
    }
}
