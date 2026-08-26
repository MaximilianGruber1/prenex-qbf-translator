using System;
using System.Collections.Generic;
using Xunit;
using prenex_qbf_translator.Language;
using prenex_qbf_translator.Parsing;

namespace prenex_qbf_translator.Tests
{
    public class SubstitutionTest
    {
        private void TestAppliction(Substitution sub, string formula, string result)
        {
            var f = ParseFormula(formula);
            string actual = sub.ApplyTo(f).ToString();
            string expected = ParseFormula(result).ToString(); // normalize string
            Assert.Equal(expected, actual);
        }
        private IFormula ParseFormula(string s)
        {
            return new Parser(s).Parse();
        }


        [Fact]
        public void Test()
        {
            Variable a = new("a");
            Variable b = new("b");
            Variable c = new("c");
            Variable x = new("x");
            Variable y = new("y");
            Variable z = new("z");

            Substitution ax = new((a, x));
            Substitution axyz = new(
                (a, ParseFormula("x&y|z")));
            Substitution abcxyz = new(
                (a, x), (b, y), (c, z));
        }

        [Fact]
        public void Empty()
        {
            Substitution empty = new();

            TestAppliction(empty, "a", "a");
            TestAppliction(empty, "!a", "!a");
            TestAppliction(empty, "a&b", "a&b");
            TestAppliction(empty, "a|b", "a|b");
            TestAppliction(empty, "a ->b", "a ->b");
            TestAppliction(empty, "a<-b", "a<-b");
            TestAppliction(empty, "a<->b", "a<->b");
            TestAppliction(empty, "?a (a&b)", "?a (a&b)");
            TestAppliction(empty, "#a (a&b)", "#a (a&b)");
            TestAppliction(empty,
                "a|b|c -> ?a " +
                "(" +
                "    !a|!b|!c <- #b " +
                "    (" +
                "        a|b|c <-> #c " +
                "        (a&b&c)" +
                "    )" +
                ")" +
                "<->" +
                "?a ?b (a|b|c)"
                ,

                "a|b|c -> ?a " +
                "(" +
                "    !a|!b|!c <- #b " +
                "    (" +
                "        a|b|c <-> #c " +
                "        (a&b&c)" +
                "    )" +
                ")" +
                "<->" +
                "?a ?b (a|b|c)"
                );
        }

        [Fact]
        public void AToA()
        {
            Substitution sub = new((
                new Variable("a"),
                ParseFormula("a")));

            TestAppliction(sub, "a", "a");
            TestAppliction(sub, "!a", "!a");
            TestAppliction(sub, "a&b", "a&b");
            TestAppliction(sub, "a|b", "a|b");
            TestAppliction(sub, "a ->b", "a ->b");
            TestAppliction(sub, "a<-b", "a<-b");
            TestAppliction(sub, "a<->b", "a<->b");
            TestAppliction(sub, "?a (a&b)", "?a (a&b)");
            TestAppliction(sub, "#a (a&b)", "#a (a&b)");
            TestAppliction(sub,
                "a|b|c -> ?a " +
                "(" +
                "    !a|!b|!c <- #b " +
                "    (" +
                "        a|b|c <-> #c " +
                "        (a&b&c)" +
                "    )" +
                ")" +
                "<->" +
                "?a ?b (a|b|c)"
                ,

                "a|b|c -> ?a " +
                "(" +
                "    !a|!b|!c <- #b " +
                "    (" +
                "        a|b|c <-> #c " +
                "        (a&b&c)" +
                "    )" +
                ")" +
                "<->" +
                "?a ?b (a|b|c)"
                );
        }

        [Fact]
        public void AToX()
        {
            Substitution sub = new((
                new Variable("a"),
                ParseFormula("x")));

            TestAppliction(sub, "a", "x");
            TestAppliction(sub, "!a", "!x");
            TestAppliction(sub, "a&b", "x&b");
            TestAppliction(sub, "a|b", "x|b");
            TestAppliction(sub, "a ->b", "x ->b");
            TestAppliction(sub, "a<-b", "x<-b");
            TestAppliction(sub, "a<->b", "x<->b");
            TestAppliction(sub, "?a (a&b)", "?a (a&b)");
            TestAppliction(sub, "#a (a&b)", "#a (a&b)");
            TestAppliction(sub,
                "a|b|c -> ?a " +
                "(" +
                "    !a|!b|!c <- #b " +
                "    (" +
                "        a|b|c <-> #c " +
                "        (a&b&c)" +
                "    )" +
                ")" +
                "<->" +
                "?a ?b (a|b|c)"
                ,

                "x|b|c -> ?a " +
                "(" +
                "    !a|!b|!c <- #b " +
                "    (" +
                "        a|b|c <-> #c " +
                "        (a&b&c)" +
                "    )" +
                ")" +
                "<->" +
                "?a ?b (a|b|c)"
                );
        }

        [Fact]
        public void BToY()
        {
            Substitution sub = new((
                new Variable("b"),
                ParseFormula("y")));

            TestAppliction(sub, "a", "a");
            TestAppliction(sub, "!a", "!a");
            TestAppliction(sub, "a&b", "a&y");
            TestAppliction(sub, "a|b", "a|y");
            TestAppliction(sub, "a ->b", "a ->y");
            TestAppliction(sub, "a<-b", "a<-y");
            TestAppliction(sub, "a<->b", "a<->y");
            TestAppliction(sub, "?a (a&b)", "?a (a&y)");
            TestAppliction(sub, "#a (a&b)", "#a (a&y)");
            TestAppliction(sub,
                "a|b|c -> ?a " +
                "(" +
                "    !a|!b|!c <- #b " +
                "    (" +
                "        a|b|c <-> #c " +
                "        (a&b&c)" +
                "    )" +
                ")" +
                "<->" +
                "?a ?b (a|b|c)"
                ,

                "a|y|c -> ?a " +
                "(" +
                "    !a|!y|!c <- #b " +
                "    (" +
                "        a|b|c <-> #c " +
                "        (a&b&c)" +
                "    )" +
                ")" +
                "<->" +
                "?a ?b (a|b|c)"
                );
        }

        [Fact]
        public void CToZ()
        {
            Substitution sub = new((
                new Variable("c"),
                ParseFormula("z")));

            TestAppliction(sub,
                "a|b|c -> ?a " +
                "(" +
                "    !a|!b|!c <- #b " +
                "    (" +
                "        a|b|c <-> #c " +
                "        (a&b&c)" +
                "    )" +
                ")" +
                "<->" +
                "?a ?b (a|b|c)"
                ,

                "a|b|z -> ?a " +
                "(" +
                "    !a|!b|!z <- #b " +
                "    (" +
                "        a|b|z <-> #c " +
                "        (a&b&c)" +
                "    )" +
                ")" +
                "<->" +
                "?a ?b (a|b|z)"
                );
        }

        [Fact]
        public void AToXEquivY()
        {
            Substitution sub = new((
                new Variable("a"),
                ParseFormula("x<->y")));

            TestAppliction(sub, "a", "x<->y");
            TestAppliction(sub, "!a", "!(x<->y)");
            TestAppliction(sub, "a&b", "(x<->y)&b");
            TestAppliction(sub, "a|b", "(x<->y)|b");
            TestAppliction(sub, "a ->b", "(x<->y) ->b");
            TestAppliction(sub, "a<-b", "(x<->y)<-b");
            TestAppliction(sub, "a<->b", "(x<->y)<->b");
            TestAppliction(sub, "?a (a&b)", "?a (a&b)");
            TestAppliction(sub, "#a (a&b)", "#a (a&b)");
            TestAppliction(sub,
                "a|b|c -> ?a " +
                "(" +
                "    !a|!b|!c <- #b " +
                "    (" +
                "        a|b|c <-> #c " +
                "        (a&b&c)" +
                "    )" +
                ")" +
                "<->" +
                "?a ?b (a|b|c)"
                ,

                "(x<->y)|b|c -> ?a " +
                "(" +
                "    !a|!b|!c <- #b " +
                "    (" +
                "        a|b|c <-> #c " +
                "        (a&b&c)" +
                "    )" +
                ")" +
                "<->" +
                "?a ?b (a|b|c)"
                );
        }

        [Fact]
        public void CToXImpliesY()
        {
            Substitution sub = new((
                new Variable("c"),
                ParseFormula("x ->y")));

            TestAppliction(sub,
                "a|b|c -> ?a " +
                "(" +
                "    !a|!b|!c <- #b " +
                "    (" +
                "        a|b|c <-> #c " +
                "        (a&b&c)" +
                "    )" +
                ")" +
                "<->" +
                "?a ?b (a|b|c)"
                ,

                "a|b|(x ->y) -> ?a " +
                "(" +
                "    !a|!b|!(x ->y) <- #b " +
                "    (" +
                "        a|b|(x ->y) <-> #c " +
                "        (a&b&c)" +
                "    )" +
                ")" +
                "<->" +
                "?a ?b (a|b|(x ->y))"
                );
        }

        [Fact]
        public void AB_BC_CA()
        {
            Substitution sub = new(
                (new Variable("a"),
                ParseFormula("b")),
                (new Variable("b"),
                ParseFormula("c")),
                (new Variable("c"),
                ParseFormula("a")));

            TestAppliction(sub, "a", "b");
            TestAppliction(sub, "!a", "!b");
            TestAppliction(sub, "a&b", "b&c");
            TestAppliction(sub, "a|b", "b|c");
            TestAppliction(sub, "a ->b", "b ->c");
            TestAppliction(sub, "a<-b", "b<-c");
            TestAppliction(sub, "a<->b", "b<->c");
            TestAppliction(sub, "?a (a&b)", "?a (a&c)");
            TestAppliction(sub, "#a (a&b)", "#a (a&c)");
            TestAppliction(sub,
                "a|b|c -> ?a " +
                "(" +
                "    !a|!b|!c <- #b " +
                "    (" +
                "        a|b|c <-> #c " +
                "        (a&b&c)" +
                "    )" +
                ")" +
                "<->" +
                "?a ?b (a|b|c)"
                ,

                "b|c|a -> ?a " +
                "(" +
                "    !a|!c|!a <- #b " +
                "    (" +
                "        a|b|a <-> #c " +
                "        (a&b&c)" +
                "    )" +
                ")" +
                "<->" +
                "?a ?b (a|b|a)"
                );
        }
    }
}
