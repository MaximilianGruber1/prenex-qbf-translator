
using Xunit;
using System;
using prenex_qbf_translator.Parsing;
using prenex_qbf_translator.Language;
using System.Text.RegularExpressions;
using System.Diagnostics.Contracts;

namespace prenex_qbf_translator.Tests
{

    public class ParserTest
    {

        private void TestSuccess(string formula)
        {
            TestEqual(formula, formula);
        }

        private void TestFailure(string formula)
        {
            Parser parser = new(formula);
            Assert.Throws<Exception>(parser.Parse);
        }

        private void TestEqual(string formula1, string formula2)
        {
            Parser p1 = new(formula1);
            Parser p2 = new(formula2);
            string w = p1.Parse().ToString();
            string wo = p2.Parse().ToString();
            Assert.Equal(wo, w);
        }




        [Fact]
        public void Test()
        {
            
            string s = new Variable("a").ToString();
        }


        [Fact]
        public void VariableNames()
        {
            TestSuccess("a");
            TestSuccess("b");
            TestSuccess("x");
            TestSuccess("y");
            TestSuccess("z");
            TestSuccess("A");
            TestSuccess("B");
            TestSuccess("X");
            TestSuccess("Y");
            TestSuccess("Z");

            TestSuccess("foo");
            TestSuccess("Foo");
            TestSuccess("FOO");
            TestSuccess("foobar");

            TestSuccess("0");
            TestSuccess("1");
            TestSuccess("9");
            TestSuccess("123");
            TestSuccess("1abc");
            TestSuccess("123abc");

            TestSuccess("a_b");
            TestSuccess("a_");
            TestSuccess("foo_bar");
            TestSuccess("x_1");
            TestSuccess("x1_y2");

            TestSuccess("a-b");
            TestSuccess("foo-bar-baz");
            TestSuccess("a--b");
            TestSuccess("a-1");
            TestSuccess("1-a");

            TestSuccess("a.b");
            TestSuccess("foo.bar.baz");

            TestSuccess("a[b]");
            TestSuccess("foo[bar]");
            TestSuccess("a[");
            TestSuccess("a]");

            TestSuccess("a$b");
            TestSuccess("a$");
            TestSuccess("$a");

            TestSuccess("a@b");
            TestSuccess("a@");
            TestSuccess("@a");

            TestSuccess("_a");
            TestSuccess(".a");
            TestSuccess("[a]");
            TestSuccess("a-b_c.d[e]$f@g");
            TestSuccess("123-a_b.c[d]$e@f");
            TestSuccess("a__b..c$$d@@e");
        }

        [Fact]
        public void VariableHyphens()
        {
            TestFailure("a-");
            TestFailure("a--");
            TestFailure("a-b-");

            TestSuccess("a-b");
            TestSuccess("a--b");
            TestSuccess("1-a");
            TestSuccess("a-1");
            TestSuccess("a--b--c");
        }

        [Fact]
        public void ParenthesizedExpressions()
        {
            TestSuccess("(a)");
            TestSuccess("((a))");
            TestSuccess("(((a)))");
            TestSuccess("((((a))))");

            TestSuccess("(a & b)");
            TestSuccess("(a | b)");
            TestSuccess("(a -> b)");
            TestSuccess("(a <- b)");
            TestSuccess("(a <-> b)");

            TestSuccess("((a & b))");
            TestSuccess("((a | b))");
            TestSuccess("((a -> b))");
            TestSuccess("((a <- b))");
            TestSuccess("((a <-> b))");

            TestSuccess("(a & (b | c))");
            TestSuccess("((a & b) | c)");
            TestSuccess("(a | (b & c))");
            TestSuccess("((a | b) & c)");
            TestSuccess("(a -> (b -> c))");
            TestSuccess("(a <- (b <- c))");
            TestSuccess("((a -> b) -> c)");
            TestSuccess("((a <- b) <- c)");
            TestSuccess("(a <-> (b <-> c))");
            TestSuccess("((a <-> b) <-> c)");
        }

        [Fact]
        public void Negation()
        {
            TestSuccess("!a");
            TestSuccess("!b");
            TestSuccess("!x");
            TestSuccess("!(a)");
            TestSuccess("!(a & b)");
            TestSuccess("!(a | b)");
            TestSuccess("!(a -> b)");
            TestSuccess("!(a <- b)");
            TestSuccess("!(a <-> b)");
            TestSuccess("!(#x a)");
            TestSuccess("!(?x a)");

            TestSuccess("!!a");
            TestSuccess("!!!a");
            TestSuccess("!!!!a");
            TestSuccess("!!!!!a");
            TestSuccess("!(!a)");
            TestSuccess("!(!(!a))");
            TestSuccess("!!(a & b)");
            TestSuccess("!(!(a | b))");

            // same for '-'
            TestSuccess("-a");
            TestSuccess("-b");
            TestSuccess("-x");
            TestSuccess("-(a)");
            TestSuccess("-(a & b)");
            TestSuccess("-(a | b)");
            TestSuccess("-(a -> b)");
            TestSuccess("-(a <- b)");
            TestSuccess("-(a <-> b)");
            TestSuccess("-(#x a)");
            TestSuccess("-(?x a)");

            TestSuccess("--a");
            TestSuccess("---a");
            TestSuccess("----a");
            TestSuccess("-----a");
            TestSuccess("-(-a)");
            TestSuccess("-(-(-a))");
            TestSuccess("--(a & b)");
            TestSuccess("-(-(a | b))");

            // mixed
            TestSuccess("!-a");
            TestSuccess("-!a");
            TestSuccess("---!!-!!!--!a");

        }

        [Fact]
        public void Conjunction()
        {
            TestSuccess("a & b");
            TestSuccess("a & b & c");
            TestSuccess("a & b & c & d");
            TestSuccess("a & b & c & d & e");
            TestSuccess("a & b & c & d & e & f");
            TestSuccess("a & b & c & d & e & f & g & h");

            TestSuccess("a & -b");
            TestSuccess("-a & b");
            TestSuccess("-a & -b");

            TestSuccess("(a) & b");
            TestSuccess("a & (b)");
            TestSuccess("(a) & (b)");
            TestSuccess("(a & b) & c");
            TestSuccess("a & (b & c)");
            TestSuccess("(a & b) & (c & d)");
        }

        [Fact]
        public void Disjunction()
        {
            TestSuccess("a | b");
            TestSuccess("a | b | c");
            TestSuccess("a | b | c | d");
            TestSuccess("a | b | c | d | e");
            TestSuccess("a | b | c | d | e | f");
            TestSuccess("a | b | c | d | e | f | g | h");

            TestSuccess("a | -b");
            TestSuccess("-a | b");
            TestSuccess("-a | -b");

            TestSuccess("(a) | b");
            TestSuccess("a | (b)");
            TestSuccess("(a) | (b)");
            TestSuccess("(a | b) | c");
            TestSuccess("a | (b | c)");
            TestSuccess("(a | b) | (c | d)");

            // same for '/'
            TestSuccess("a / b");
            TestSuccess("a / b / c");
            TestSuccess("a / b / c / d");
            TestSuccess("a / b / c / d / e");
            TestSuccess("a / b / c / d / e / f");
            TestSuccess("a / b / c / d / e / f / g / h");

            TestSuccess("a / -b");
            TestSuccess("-a / b");
            TestSuccess("-a / -b");

            TestSuccess("(a) / b");
            TestSuccess("a / (b)");
            TestSuccess("(a) / (b)");
            TestSuccess("(a / b) / c");
            TestSuccess("a / (b / c)");
            TestSuccess("(a / b) / (c / d)");
        }

        [Fact]
        public void Implication()
        {
            TestSuccess("a -> b");
            TestSuccess("a <- b");

            TestSuccess("-a -> b");
            TestSuccess("-a <- b");
            TestSuccess("a -> -b");
            TestSuccess("a <- -b");
            TestSuccess("-a -> -b");
            TestSuccess("-a <- -b");

            TestSuccess("a & b -> c");
            TestSuccess("a & b <- c");
            TestSuccess("a -> b & c");
            TestSuccess("a <- b & c");
            TestSuccess("a | b -> c");
            TestSuccess("a | b <- c");
            TestSuccess("a -> b | c");
            TestSuccess("a <- b | c");

            TestSuccess("a & b | c -> d");
            TestSuccess("a & b | c <- d");
            TestSuccess("a -> b & c | d");
            TestSuccess("a <- b & c | d");
            TestSuccess("a | b & c -> d | e");
            TestSuccess("a | b & c <- d | e");

            TestSuccess("(a -> b)");
            TestSuccess("(a <- b)");
            TestSuccess("((a -> b))");
            TestSuccess("((a <- b))");
            TestSuccess("(a & b) -> c");
            TestSuccess("(a & b) <- c");
            TestSuccess("a -> (b & c)");
            TestSuccess("a <- (b & c)");
            TestSuccess("(a | b) -> c");
            TestSuccess("(a | b) <- c");
            TestSuccess("a -> (b | c)");
            TestSuccess("a <- (b | c)");

            TestSuccess("(a -> b) -> c");
            TestSuccess("(a <- b) -> c");
            TestSuccess("(a -> b) <- c");
            TestSuccess("(a <- b) <- c");

            TestSuccess("a -> (b -> c)");
            TestSuccess("a <- (b <- c)");
            TestSuccess("a -> (b <- c)");
            TestSuccess("a <- (b -> c)");


            TestFailure("a -> b -> c");
            TestFailure("a <- b <- c");
            TestFailure("a -> b <- c");
            TestFailure("a <- b -> c");
        }

        [Fact]
        public void Equivalence()
        {
            TestSuccess("a <-> b");
            TestSuccess("-a <-> b");

            TestSuccess("a & b <-> c");
            TestSuccess("a | b <-> c");
            TestSuccess("a -> b <-> c");
            TestSuccess("a <- b <-> c");
            TestSuccess("a <-> b -> c");
            TestSuccess("a <-> b <- c");

            TestSuccess("a & b | c <-> d");
            TestSuccess("a -> b & c <-> d");
            TestSuccess("a <- b & c <-> d");
            TestSuccess("a | b -> c | d <-> e");
            TestSuccess("a | b <- c | d <-> e");

            TestSuccess("(a <-> b)");
            TestSuccess("((a <-> b))");
            TestSuccess("(a -> b) <-> c");
            TestSuccess("(a <- b) <-> c");
            TestSuccess("a <-> (b -> c)");
            TestSuccess("a <-> (b <- c)");
            TestSuccess("(a & b) <-> (c | d)");

            TestSuccess("(a <-> b) <-> c");
            TestSuccess("a <-> (b <-> c)");
            TestSuccess("(a <-> b) <-> (c <-> d)");

            TestSuccess("a <-> b <-> c");
            TestSuccess("a <-> b <-> c <-> d");
            TestSuccess("a <-> b <-> c <-> d <-> e <-> f <-> g <-> h <-> i");
        }

        [Fact]
        public void AllBooleanOperators()
        {
            TestSuccess("a & b | c -> d <-> e");
            TestSuccess("a & b | c <- d <-> e");
            TestSuccess("a | b & c -> d <-> e");
            TestSuccess("a | b & c <- d <-> e");
            TestSuccess("a -> b | c & d <-> e");
            TestSuccess("a <- b | c & d <-> e");
            TestSuccess("a <-> b -> c | d & e");
            TestSuccess("a <-> b <- c | d & e");

            TestSuccess("a & b & c | d | e & f -> g | h & i <-> j");
            TestSuccess("a & b & c | d | e & f <- g | h & i <-> j");

            TestSuccess("-a & b | c -> d <-> e");
            TestSuccess("-a & b | c <- d <-> e");
            TestSuccess("a & -b | -c -> d <-> -e");
            TestSuccess("a & -b | -c <- d <-> -e");
            TestSuccess("-(a & b) | c -> -(d | e) <-> f");
            TestSuccess("-(a & b) | c <- -(d | e) <-> f");
        }


        [Fact]
        public void QuantifierBasics()
        {
            TestSuccess("#x a");
            TestSuccess("#x b");

            TestSuccess("?x a");
            TestSuccess("?x b");

            TestSuccess("#x #y a");
            TestSuccess("#x #y #z a");
            TestSuccess("#x #y #z #w a");

            TestSuccess("?x ?y a");
            TestSuccess("?x ?y ?z a");
            TestSuccess("?x ?y ?z ?w a");

            TestSuccess("#x #x a");
            TestSuccess("?x ?x a");
            TestSuccess("#x #y #x a");
            TestSuccess("?x ?y ?y a");
        }

        [Fact]
        public void QuantifierNesting()
        {
            TestSuccess("#x ?y a");
            TestSuccess("?x #y a");

            TestSuccess("#x #y ?z a");
            TestSuccess("?x ?y #z a");
            TestSuccess("#x ?y ?z a");
            TestSuccess("?x #y #z a");
            TestSuccess("#a ?b #c a");
            TestSuccess("?a #b ?c a");
        }

        [Fact]
        public void QuantifiersWithExpressions()
        {
            TestSuccess("#x -a");
            TestSuccess("?x -a");
            TestSuccess("#x (a & b)");
            TestSuccess("#x (a | b)");
            TestSuccess("#x (a -> b)");
            TestSuccess("#x (a <- b)");
            TestSuccess("#x (a <-> b)");

            TestSuccess("?x (a & b)");
            TestSuccess("?x (a | b)");
            TestSuccess("?x (a -> b)");
            TestSuccess("?x (a <- b)");
            TestSuccess("?x (a <-> b)");

            TestSuccess("#x ((a & b) | c)");
            TestSuccess("#x (a & (b | c))");
            TestSuccess("#x ((a -> b) & c)");
            TestSuccess("#x ((a <- b) & c)");
            TestSuccess("#x (a <-> (b | c))");
        }

        [Fact]
        public void QuantifiersInsideFormulas()
        {
            TestSuccess("#x a & #y b");
            TestSuccess("#x a | ?y b");
            TestSuccess("#x a -> ?y b");
            TestSuccess("#x a <- ?y b");
            TestSuccess("#x a <-> ?y b");

            TestSuccess("(#x a) & (#y b)");
            TestSuccess("(#x a) | (?y b)");
            TestSuccess("(#x a) -> (?y b)");
            TestSuccess("(#x a) <- (?y b)");
            TestSuccess("(#x a) <-> (?y b)");

            TestSuccess("#x a & ?y b | c");
            TestSuccess("#x a | ?y b & c");
            TestSuccess("#x a -> ?y b | c");
            TestSuccess("#x a <- ?y b | c");
            TestSuccess("#x a <-> ?y (b | c)");
        }

        [Fact]
        public void DeeplyNestedExpressions()
        {
            TestSuccess("(a)");
            TestSuccess("((a))");
            TestSuccess("(((a)))");
            TestSuccess("((((a))))");
            TestSuccess("(((((a)))))");
            TestSuccess("((((((a))))))");

            TestSuccess("(a & (b | (c & (d | e))))");
            TestSuccess("((a & b) | ((c & d) | e))");
            TestSuccess("(a | (b & (c | (d & e))))");

            TestSuccess("#a #b #c #d a");
            TestSuccess("#a ?b #c ?d a");
            TestSuccess("?a #b ?c #d a");

            TestSuccess("#a (#b ((a & b) | c))");
            TestSuccess("?a (#b ((a | b) & c))");
            TestSuccess("#a (#b ((a -> b) <- c))");
            TestSuccess("?a (-(#b (a <-> b)))");
            TestSuccess("#a (-(?b (a -> b)))");
            TestSuccess("#a (-(?b (a <- b)))");
        }

        [Fact]
        public void FullValidFormulas()
        {
            TestSuccess("#x #y -((x & y) | w)");
            TestSuccess("?x (-x -> w)");
            TestSuccess("?x (-x <- w)");
            TestSuccess("#x (x <-> (-x | w))");
            TestSuccess("#x #y ((x & -y) -> (y | w))");
            TestSuccess("#x #y ((x & -y) <- (y | w))");
            TestSuccess("?x ?y (((x | y) & -(x & y)) <-> (x | y))");

            TestSuccess("#x ((#y (x & y)) -> (?z (x | z)))");
            TestSuccess("#x ((#y (x & y)) <- (?z (x | z)))");
            TestSuccess("?x ((#y (x -> y)) <-> (-x | w))");
            TestSuccess("?x ((#y (x <- y)) <-> (-x | w))");
            TestSuccess("#a #b #c ((a & b) | (-b & c) -> (a <-> c))");
            TestSuccess("#a #b #c ((a & b) | (-b & c) <- (a <-> c))");
            TestSuccess("?x ?y ?z ((x | y) & (-x | z) & (-y | -z))");

            TestSuccess("#x (-(#y ((x & y) -> (x | w))))");
            TestSuccess("#x (-(#y ((x & y) <- (x | w))))");
            TestSuccess("?x (-(?y ((x | y) <-> (x & w))))");
        }

        [Fact]
        public void LongFormulas()
        {
            TestSuccess("a & b & c & d & e & f & g & h & i & j");
            TestSuccess("a | b | c | d | e | f | g | h | i | j");
            TestSuccess("a & b | c & d | e & f | g & h | i & j");
            TestSuccess("a | b & c | d & e | f & g | h & i | j");

            TestSuccess("#a #b #c ((a & b) | (b & c) | (c & a))");
            TestSuccess("#a #b #c ((a -> b) & (b -> c) & (c -> a))");
            TestSuccess("#a #b #c ((a <- b) & (b <- c) & (c <- a))");
        }

        [Fact]
        public void RandomLongFormulas()
        {
            TestSuccess("#foo #bar ((foo & -bar) | w)");
            TestSuccess("?x1 ?y_2 ((x1 | y_2) -> -(x1 & y_2))");
            TestSuccess("?x1 ?y_2 ((x1 | y_2) <- -(x1 & y_2))");
            TestSuccess("A1_B2 <-> (-foo | ?bar_baz w)");
            TestSuccess("#x #y #z ((x -> y) & (y -> z) & -(x <-> z))");
            TestSuccess("#x #y #z ((x <- y) & (y <- z) & -(x <-> z))");
            TestSuccess("?a1 ?b2 (-((a1 | w) -> (b2 -> w)))");
            TestSuccess("?a1 ?b2 (-((a1 | w) <- (b2 <- w)))");
            TestSuccess("#foo_1 #Bar2 ((foo_1 & Bar2) | (#x x))");
            TestSuccess("((a -> b) & (-c | d)) <-> (#x #y (x & y))");
            TestSuccess("((a <- b) & (-c | d)) <-> (#x #y (x & y))");
            TestSuccess("-(?x (x <-> (#y (y -> x))))");
            TestSuccess("-(?x (x <-> (#y (y <- x))))");
        }


        [Fact]
        public void InvalidOperators()
        {
            TestFailure("a && b");
            TestFailure("a || b");
            TestFailure("a // b");
            TestFailure("a => b");
            TestFailure("a <= b");
            TestFailure("a <=> b");
            TestFailure("a = b");
            TestFailure("a == b");
            TestFailure("a != b");
            TestFailure("a <> b");
            TestFailure("a ^ b");

        }

        [Fact]
        public void MissingOperands()
        {
            TestFailure("&");
            TestFailure("|");
            TestFailure("->");
            TestFailure("<-");
            TestFailure("<->");

            TestFailure("a &");
            TestFailure("a |");
            TestFailure("a ->");
            TestFailure("a <-");
            TestFailure("a <->");

            TestFailure("& a");
            TestFailure("| a");
            TestFailure("-> a");
            TestFailure("<- a");
            TestFailure("<-> a");

            TestFailure("!");
            TestFailure("#");
            TestFailure("?");
        }

        [Fact]
        public void ConsecutiveBinaryOperators()
        {
            TestFailure("a & & b");
            TestFailure("a | | b");
            TestFailure("a -> -> b");
            TestFailure("a <- <- b");
            TestFailure("a <-> <-> b");

            TestFailure("a -> | b");
            TestFailure("a <-> <- b");
            TestFailure("a & -> b");
            TestFailure("a | -> b");
            TestFailure("a <- <-> b");
            TestFailure("a ->  &b");
        }

        [Fact]
        public void BadParentheses()
        {
            TestFailure("(");
            TestFailure(")");
            TestFailure("(a");
            TestFailure("a)");
            TestFailure("((a)");
            TestFailure("(a))");
            TestFailure("((((((a)))))");
            TestFailure("(((((a))))))");

            TestFailure("()");
            TestFailure("(())");
            TestFailure("((((()))))");
        }

        [Fact]
        public void BadNegation()
        {
            TestFailure("!!");
            TestFailure("!&");
            TestFailure("!|");
            TestFailure("!->");
            TestFailure("!<->");
            TestFailure("!<-");
        }

        [Fact]
        public void WhitespaceTests()
        {
            TestSuccess("a&b");
            TestSuccess("a &b");
            TestSuccess("a& b");
            TestSuccess("a & b");

            TestSuccess("a|b");
            TestSuccess("a &b");
            TestSuccess("a& b");
            TestSuccess("a & b");

            TestFailure("a->b"); // very weird behavior of limboole parser; reads 'a-' as invalid variable name
            TestSuccess("a ->b");
            TestFailure("a-> b"); // also here
            TestSuccess("a -> b");

            TestSuccess("a<-b");
            TestSuccess("a <-b");
            TestSuccess("a<- b");
            TestSuccess("a <- b");

            TestSuccess("a<->b");
            TestSuccess("a <->b");
            TestSuccess("a<-> b");
            TestSuccess("a <-> b");

            TestSuccess("#x  a");

            TestSuccess("-a");
            TestSuccess("- a");
            TestSuccess("- a");

            TestSuccess("( a & b )");

            TestSuccess("-           a");
            TestSuccess("a\n\n\t  &  \t   \n   b");
        }

        [Fact]
        public void WhitespaceInsideOperators()
        {
            TestFailure("a < - b");
            TestFailure("a - > b");
            TestFailure("a < - > b");
            TestFailure("a <- > b");
            TestFailure("a < -> b");
        }

        [Fact]
        public void EmptyInput()
        {
            TestFailure("");
            TestFailure(" ");
            TestFailure("   ");
            TestFailure("\n");
            TestFailure("\t");
            TestFailure(" \n\t ");
        }

        [Fact]
        public void CompleteConsumption()
        {
            TestFailure("a b");
            TestFailure("(a) b");
            TestFailure("a & b c");
            TestFailure("a <-> b c");
            TestFailure("#x a b");

            TestFailure("x !");
            TestFailure("x ?");
            TestFailure("x #");
        }


        // tests using TestEqual directly

        [Fact]
        public void OperatorPrecedence()
        {
            TestEqual(
                "(((a & b) | c) -> d) <-> e",
                "a & b | c -> d <-> e"
                );
            TestEqual(
                "(((a & b) | c) <- d) <-> e",
                "a & b | c <- d <-> e"
                );

            TestEqual(
                "((a | (b & c)) -> d) <-> e",
                "a | b & c -> d <-> e"
                );
            TestEqual(
                "((a | (b & c)) <- d) <-> e",
                "a | b & c <- d <-> e"
                );

            TestEqual(
                "(a -> (b | (c & d))) <-> e",
                "a -> b | c & d <-> e"
                );
            TestEqual(
                "(a <- (b | (c & d))) <-> e",
                "a <- b | c & d <-> e"
                );

            TestEqual(
                "a <-> (b -> (c | (d & e)))",
                "a <-> b -> c | d & e"
                );
            TestEqual(
                "a <-> (b <- (c | (d & e)))",
                "a <-> b <- c | d & e"
                );

            TestEqual(
                "((((!a) & (!b) & (!c)) | (!d) | ((!e) & (!f))) -> ((!g) | ((!h) & (!i)))) <-> (!j)",
                "!a & !b & !c | !d | !e & !f -> !g | !h & !i <-> !j"
                );
            TestEqual(
                "((((!a) & (!b) & (!c)) | (!d) | ((!e) & (!f))) <- ((!g) | ((!h) & (!i)))) <-> (!j)",
                "!a & !b & !c | !d | !e & !f <- !g | !h & !i <-> !j"
                );
        }

        [Fact]
        public void QuantifierPrecedence()
        {
            TestEqual("#a a & b", "(#a a) & b");
            TestEqual("#a a | b", "(#a a) | b");
            TestEqual("#a a -> b", "(#a a) -> b");
            TestEqual("#a a <- b", "(#a a) <- b");
            TestEqual("#a a <-> b", "(#a a) <-> b");

            TestEqual("?a a & b", "(?a a) & b");
            TestEqual("?a a | b", "(?a a) | b");
            TestEqual("?a a -> b", "(?a a) -> b");
            TestEqual("?a a <- b", "(?a a) <- b");
            TestEqual("?a a <-> b", "(?a a) <-> b");
        }
    }
}
