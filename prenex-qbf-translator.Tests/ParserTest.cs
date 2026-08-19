
using Xunit;
using System;
using prenex_qbf_translator.Parsing;
using prenex_qbf_translator.Language;
using System.Text.RegularExpressions;

namespace prenex_qbf_translator.Tests
{
    public class ParserTest
    {

        private void TestSuccess(string formula)
        {
            Parser parser = new(new Scanner(formula));
            IFormula f = parser.Parse();
        }

        private void TestFailure(string formula)
        {
            Parser parser = new(new Scanner(formula));
            Assert.Throws<Exception>(parser.Parse);
        }

        private string RemoveAllWhiteSpace(string text)
        {
            return Regex.Replace(text, @"\s+", "");
        }





        [Fact]
        public void TestVariableNames()
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
            TestSuccess("$true");
            TestSuccess("$false");
            TestSuccess("$true1");

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
        public void InvalidVariableNames2()
        {
            TestFailure("a-");
            TestFailure("a--");
            TestFailure("a-b-");
        }

        [Fact]
        public void VariableHyphenRules()
        {
            TestSuccess("a-b");
            TestSuccess("a--b");
            TestSuccess("1-a");
            TestSuccess("a-1");
            TestSuccess("a--b--c");

            TestFailure("a-");
            TestFailure("a--");
            TestFailure("a-b-");
        }

        [Fact]
        public void VariableAllowedCharacters()
        {
            TestSuccess("a");
            TestSuccess("Z");
            TestSuccess("0");
            TestSuccess("9");
            TestSuccess("a-b");
            TestSuccess("a_b");
            TestSuccess("a.b");
            TestSuccess("a[b]");
            TestSuccess("a$b");
            TestSuccess("a@b");

            TestSuccess("a_");
            TestSuccess("a.");
            TestSuccess("a[");
            TestSuccess("a]");
            TestSuccess("a$");
            TestSuccess("a@");

            TestSuccess("_a");
            TestSuccess(".a");
            TestSuccess("[a]");
            TestSuccess("$a");
            TestSuccess("@a");
        }

        [Fact]
        public void VariableSpecialCharacterCombinations()
        {
            TestSuccess("a-b_c.d");
            TestSuccess("a[b]$c@d");
            TestSuccess("a-b_c.d[e]$f@g");
            TestSuccess("123-a_b.c[d]$e@f");
            TestSuccess("a__b..c$$d@@e");
            TestSuccess("[a]");
            TestSuccess("[$]");
            TestSuccess("@a");
            TestSuccess("$a");
            TestSuccess(".a");
            TestSuccess("_a");
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
        public void UnaryNegation()
        {
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

            TestSuccess("(-a)");
            TestSuccess("(-(a & b))");
            TestSuccess("(-(a | b))");
            TestSuccess("(-(a -> b))");
            TestSuccess("(-(a <- b))");
            TestSuccess("(-(a <-> b))");

            TestSuccess("--a");
            TestSuccess("---a");
            TestSuccess("----a");
            TestSuccess("-----a");
            TestSuccess("-(-a)");
            TestSuccess("-(-(-a))");
            TestSuccess("--(a & b)");
            TestSuccess("-(-(a | b))");
        }

        [Fact]
        public void Conjunction()
        {
            TestSuccess("a & b");
            TestSuccess("a & c");
            TestSuccess("x & y");

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
            TestSuccess("a | c");
            TestSuccess("x | y");

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
        }

        [Fact]
        public void Equivalence()
        {
            TestSuccess("a <-> b");
            TestSuccess("$true <-> a");
            TestSuccess("a <-> $false");
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
        }

        [Fact]
        public void ImplicationAssociativityAndRestriction()
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

            TestFailure("a -> b -> c");
            TestFailure("a <- b <- c");
            TestFailure("a -> b <- c");
            TestFailure("a <- b -> c");

            TestSuccess("(a -> b) -> c");
            TestSuccess("(a <- b) -> c");
            TestSuccess("(a -> b) <- c");
            TestSuccess("(a <- b) <- c");

            TestSuccess("a -> (b -> c)");
            TestSuccess("a <- (b <- c)");
            TestSuccess("a -> (b <- c)");
            TestSuccess("a <- (b -> c)");

            TestSuccess("((a -> b) -> c)");
            TestSuccess("((a <- b) <- c)");

            TestFailure("(a -> b -> c)");
            TestFailure("(a <- b <- c)");
        }

        [Fact]
        public void EquivalenceNesting()
        {
            TestSuccess("(a <-> b) <-> c");
            TestSuccess("a <-> (b <-> c)");
            TestSuccess("(a <-> b) <-> (c <-> d)");
            TestSuccess("((a <-> b) <-> c) <-> d");
            TestSuccess("a <-> (b <-> (c <-> d))");

            TestSuccess("a <-> (b -> c)");
            TestSuccess("a <-> (b <- c)");
            TestSuccess("(a -> b) <-> c");
            TestSuccess("(a <- b) <-> c");
            TestSuccess("(a <-> b) -> c");
            TestSuccess("(a <-> b) <- c");
            TestSuccess("a -> (b <-> c)");
            TestSuccess("a <- (b <-> c)");
        }

        [Fact]
        public void QuantifierBasics()
        {
            TestSuccess("#x a");
            TestSuccess("#x b");
            TestSuccess("#x $true");
            TestSuccess("#x $false");

            TestSuccess("?x a");
            TestSuccess("?x b");
            TestSuccess("?x $true");
            TestSuccess("?x $false");

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
        public void QuantifierVariableNames()
        {
            TestSuccess("#x1 a");
            TestSuccess("#x_1 a");
            TestSuccess("#foo a");
            TestSuccess("#foo_bar a");
            TestSuccess("#Foo a");
            TestSuccess("#ABC123 a");
            TestSuccess("#a_b_c a");

            TestSuccess("#x1 #y2 #z3 a");
            TestSuccess("#foo #bar #baz a");
            TestSuccess("#a_1 #b_2 #c_3 a");
            TestSuccess("#A #B #C a");
        }

        [Fact]
        public void QuantifierNesting()
        {
            TestSuccess("#x #y a");
            TestSuccess("#x ?y a");
            TestSuccess("?x #y a");
            TestSuccess("?x ?y a");

            TestSuccess("#x #y ?z a");
            TestSuccess("?x ?y #z a");
            TestSuccess("#x ?y ?z a");
            TestSuccess("?x #y #z a");

            TestSuccess("#a #b #c a");
            TestSuccess("#a ?b #c a");
            TestSuccess("?a #b ?c a");
            TestSuccess("?a ?b ?c a");
        }

        [Fact]
        public void QuantifiersWithNegation()
        {
            TestSuccess("#x -a");
            TestSuccess("?x -a");

            TestSuccess("-#x a");
            TestSuccess("-?x a");

            TestSuccess("#x -(a & b)");
            TestSuccess("?x -(a | b)");

            TestSuccess("-#x (a & b)");
            TestSuccess("-?x (a | b)");

            TestSuccess("#x -#y a");
            TestSuccess("?x -?y a");
            TestSuccess("#x -?y a");
            TestSuccess("?x -#y a");
        }

        [Fact]
        public void QuantifierBodyPrecedence()
        {
            TestSuccess("#x a & b");
            TestSuccess("#x a | b");
            TestSuccess("#x a -> b");
            TestSuccess("#x a <- b");
            TestSuccess("#x a <-> b");

            TestSuccess("#x -a & b");
            TestSuccess("#x -a | b");
            TestSuccess("#x -a -> b");
            TestSuccess("#x -a <- b");
            TestSuccess("#x -a <-> b");

            TestSuccess("#x (a & b)");
            TestSuccess("#x (a | b)");
            TestSuccess("#x (a -> b)");
            TestSuccess("#x (a <- b)");
            TestSuccess("#x (a <-> b)");

            TestSuccess("(#x a) & b");
            TestSuccess("(#x a) | b");
            TestSuccess("(#x a) -> b");
            TestSuccess("(#x a) <- b");
            TestSuccess("(#x a) <-> b");
        }

        [Fact]
        public void QuantifiersWithFullExpressions()
        {
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
        public void FullPrecedenceStack()
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
        public void QuantifiersMixedWithBinaryOperators()
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
        public void QuantifierShadowing()
        {
            TestSuccess("#x #x x");
            TestSuccess("?x ?x x");
            TestSuccess("#x ?x x");
            TestSuccess("?x #x x");

            TestSuccess("#x #y #y #z a");
            TestSuccess("?x ?y ?x ?z b");
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
        public void RandomLookingValidFormulas()
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
        public void InvalidVariableNames()
        {
            TestSuccess("1");
            TestSuccess("123");
            TestSuccess("1abc");
            TestSuccess("_abc");
            TestSuccess("__abc");
            TestSuccess("_1");
            TestSuccess("1_");
            TestSuccess("1_abc");
            TestSuccess("-abc");
            TestSuccess("a-b");
            TestSuccess("a.b");
            TestFailure("a+b");
            TestFailure("a=b");
            TestFailure("a b");
        }


        [Fact]
        public void InvalidOperators()
        {
            TestFailure("a>b");
            TestFailure("a<b");
            TestFailure("a>-b");
            TestFailure("a<>b");
            TestFailure("a!-b");

            TestFailure("a&&b");
            TestFailure("a||b");

            TestFailure("a<ib");
            TestFailure("a<<->b");
            TestFailure("a-->b");

            TestFailure("a<<-b");
            TestFailure("a<-><-b");
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
        }

        [Fact]
        public void ConsecutiveBinaryOperators()
        {
            TestFailure("a & & b");
            TestFailure("a | | b");
            TestFailure("a & | b");
            TestFailure("a | & b");

            TestFailure("a -> -> b");
            TestFailure("a <- <- b");
            TestFailure("a -> <- b");
            TestFailure("a <- -> b");

            TestFailure("a -> | b");
            TestFailure("a | -> b");
            TestFailure("a & -> b");
            TestFailure("a -> & b");

            TestFailure("a <- | b");
            TestFailure("a | <- b");
            TestFailure("a & <- b");
            TestFailure("a <- & b");
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

            TestFailure("((a)");
            TestFailure("(a))");
            TestFailure("((a & b)");
            TestFailure("(a & b))");

            TestFailure("()");
            TestFailure("(())");
            TestFailure("((()))");

            TestFailure("(&)");
            TestFailure("(|)");
            TestFailure("(->)");
            TestFailure("(<-)");
            TestFailure("(<->)");
        }

        [Fact]
        public void BadNegation()
        {
            TestFailure("-");
            TestFailure("-~");
            TestFailure("-&");
            TestFailure("-|");
            TestFailure("-->");
            TestFailure("-<->");
            TestFailure("-<-");
            TestFailure("-)");
        }

        [Fact]
        public void WhitespaceTests()
        {
            TestSuccess("a&b");
            TestSuccess("a &b");
            TestSuccess("a& b");
            TestSuccess("a & b");

            TestFailure("a->b"); // very weird behavior of limboole syntax
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
            TestSuccess("-  a");

            TestSuccess("(a&b)");
            TestSuccess("( a & b )");
        }

        [Fact]
        public void WhitespaceInsideTokens()
        {
            TestFailure("-< -");
            TestFailure("- >");
            TestFailure("< - >");

            TestFailure("< -");
            TestFailure("<  -");

            TestFailure("foo bar");
            TestFailure("foo 123");
            TestFailure("foo _bar");
        }

        [Fact]
        public void EmptyInputAndEOF()
        {
            TestFailure("");
            TestFailure(" ");
            TestFailure("   ");
            TestFailure("\n");
            TestFailure("\t");
            TestFailure(" \n\t ");
        }

        [Fact]
        public void PrefixAndSuffixGarbage()
        {
            TestFailure("x!");
            TestFailure("x?");
            TestFailure("x:");
            TestFailure("x,");
            TestFailure("x)");
            TestFailure("x-");
            TestFailure("x&");
            TestFailure("x|");
            TestFailure("x->");
            TestFailure("x<-");
            TestFailure("x<->");

            TestFailure("#x");
            TestFailure("?x");
            TestFailure("#[x]");
            TestFailure("?[x]");
        }

        [Fact]
        public void CompleteConsumption()
        {
            TestSuccess("a");
            TestSuccess("a & b");
            TestSuccess("(a)");
            TestSuccess("#x a");

            TestFailure("a b");
            TestFailure("a & b c");
            TestFailure("(a) b");
            TestFailure("#x a b");
            TestFailure("a <-> b c");

            TestFailure("a)");
            TestFailure("(a))");
            TestFailure("((a)))");
        }

        [Fact]
        public void QuantifierVariableCharacters()
        {
            TestSuccess("#a a");
            TestSuccess("#1 a");
            TestSuccess("#a-b a");
            TestSuccess("#a_b a");
            TestSuccess("#a.b a");
            TestSuccess("#a[b] a");
            TestSuccess("#a$b a");
            TestSuccess("#a@b a");

            TestSuccess("?a a");
            TestSuccess("?1 a");
            TestSuccess("?a-b a");
            TestSuccess("?a_b a");
            TestSuccess("?a.b a");
            TestSuccess("?a[b] a");
            TestSuccess("?a$b a");
            TestSuccess("?a@b a");

            TestFailure("#a- a");
            TestFailure("?a- a");
            TestFailure("#-a a");
            TestFailure("?-a a");
        }

        [Fact]
        public void DisjunctionOperators()
        {
            TestSuccess("a | b");
            TestSuccess("a / b");
            TestSuccess("a | b | c");
            TestSuccess("a / b / c");
            TestSuccess("a | b / c");
            TestSuccess("a / b | c");

            TestFailure("a || b");
            TestFailure("a // b");
            TestFailure("a | / b");
            TestFailure("a / | b");
        }

        [Fact]
        public void ImplicationOperators()
        {
            TestSuccess("a -> b");
            TestSuccess("a <- b");

            TestSuccess("-a -> b");
            TestSuccess("-a <- b");
            TestSuccess("a -> -b");
            TestSuccess("a <- -b");

            TestFailure("a -> -> b");
            TestFailure("a <- <- b");
            TestFailure("a -> <- b");
            TestFailure("a <- -> b");
        }

        [Fact]
        public void EquivalenceAssociativity()
        {
            TestSuccess("a <-> b");
            TestSuccess("a <-> b <-> c");
            TestSuccess("a <-> b <-> c <-> d");
            TestSuccess("a <-> b <-> c <-> d <-> e");

            TestSuccess("(a <-> b) <-> c");
            TestSuccess("a <-> (b <-> c)");
            TestSuccess("(a <-> b) <-> (c <-> d)");
            TestSuccess("a <-> (b <-> (c <-> d))");
        }

        [Fact]
        public void RecursiveUnaryOperators()
        {
            TestSuccess("-a");
            TestSuccess("!a");
            TestSuccess("--a");
            TestSuccess("!!a");
            TestSuccess("-!a");
            TestSuccess("!-a");
            TestSuccess("-!-a");
            TestSuccess("!-!a");

            TestFailure("-");
            TestFailure("!");
            TestFailure("--");
            TestFailure("!!");
            TestFailure("-&");
            TestFailure("!&");
        }

        [Fact]
        public void LexerParserBoundaries()
        {
            TestSuccess("a-b");
            TestSuccess("a--b");
            TestSuccess("a-b-c");

            TestSuccess("-a");
            TestSuccess("--a");

            TestSuccess("#a a");
            TestSuccess("?a a");

            TestSuccess("#a-b a");
            TestSuccess("?a-b a");

            TestSuccess("a$b");
            TestSuccess("a@b");
            TestSuccess("a[b]");
        }

    }
}
