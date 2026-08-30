using System;
using System.Collections.Generic;
using Xunit;
using prenex_qbf_translator.Language;
using prenex_qbf_translator.Parsing;
using Xunit.Sdk;
using prenex_qbf_translator.PolynomialPrenexing;

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
        public void AddVariable()
        {
            Substitution sub = new(
                (new Variable("a"), ParseFormula("x & y")),
                (new Variable("b"), ParseFormula("y | z")),
                (new Variable("c"), ParseFormula("x <-> y"))
                );

            Assert.Throws<ArgumentException>(() => sub.Add(new Variable("a"), ParseFormula("x")));
            Assert.Throws<ArgumentException>(() => sub.Add(new Variable("a"), ParseFormula("x & y")));

            sub.Add(new Variable("d"), ParseFormula("x <- z"));
            Assert.Equal(4, sub.Count);
        }

        [Fact]
        public void AddSubstitution()
        {
            Substitution sub = new(
                (new Variable("a"), ParseFormula("x & y")),
                (new Variable("b"), ParseFormula("y | z")),
                (new Variable("c"), ParseFormula("x <-> y"))
                );

            Substitution badSub = new(
                (new Variable("b"), ParseFormula("x | y")),
                (new Variable("d"), ParseFormula("y <- z")),
                (new Variable("e"), ParseFormula("x & !y"))
                );

            Assert.Throws<ArgumentException>(() => sub.Add(badSub));



            Substitution goodSub = new(
                (new Variable("f"), ParseFormula("x | y")),
                (new Variable("d"), ParseFormula("y <- z")),
                (new Variable("e"), ParseFormula("x & !y"))
                );

            sub.Add(goodSub);
            Assert.Equal(6, sub.Count);
        }

        [Fact]
        public void RemoveVariable()
        {
            Substitution sub = new(
                (new Variable("a"), ParseFormula("x & y")),
                (new Variable("b"), ParseFormula("y | z")),
                (new Variable("c"), ParseFormula("x <-> y"))
                );

            sub.Remove(new Variable("c"));

            Assert.Equal(2, sub.Count);

            sub.Remove(new Variable("c"));

            Assert.Equal(2, sub.Count);

            sub.Remove(new Variable("x"));

            Assert.Equal(2, sub.Count);

            sub.Remove(new Variable("a"));

            Assert.Equal(1, sub.Count);

            sub.Remove(new Variable("b"));

            Assert.Equal(0, sub.Count);
        }

        [Fact]
        public void Apply_Empty()
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
        public void Apply_AToA()
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
        public void Apply_AToX()
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
        public void Apply_BToY()
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
        public void Apply_CToZ()
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
        public void Apply_AToXEquivY()
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
        public void Apply_CToXImpliesY()
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
        public void Apply_AB_BC_CA()
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
