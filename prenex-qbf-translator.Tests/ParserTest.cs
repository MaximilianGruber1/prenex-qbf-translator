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
        public void TestConstants()
        {
            TestSuccess("$true");
            TestSuccess("$false");
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

            TestSuccess("p");
            TestSuccess("P");
            TestSuccess("foo");
            TestSuccess("Foo");
            TestSuccess("FOO");
            TestSuccess("foobar");
            TestSuccess("x1");
            TestSuccess("x2");
            TestSuccess("x10");
            TestSuccess("abc123");
            TestSuccess("a_b");
            TestSuccess("a_");
            TestSuccess("foo_bar");
            TestSuccess("foo_bar_baz");
            TestSuccess("x_1");
            TestSuccess("x1_y2");
            TestSuccess("A1_B2");
            TestSuccess("a123456789");

            TestSuccess("a_");
            TestSuccess("a__");
            TestSuccess("a___");
            TestSuccess("a_1");
            TestSuccess("a_2");
            TestSuccess("a_123");
            TestSuccess("a1_");
            TestSuccess("a1__");
            TestSuccess("a1_2");
            TestSuccess("a1__2");
            TestSuccess("a123_456");
            TestSuccess("a1__23_4____56___");

            TestSuccess("a");
            TestSuccess("Z");
            TestSuccess("aZ");
            TestSuccess("Za");
            TestSuccess("abcXYZ");
            TestSuccess("XYZabc");
            TestSuccess("a1");
            TestSuccess("Z9");
            TestSuccess("a_b_c");
            TestSuccess("A_B_C");
        }

        [Fact]
        public void ParenthesizedExpressions()
        {
            TestSuccess("(a)");
            TestSuccess("((a))");
            TestSuccess("(((a)))");
            TestSuccess("((((a))))");

            TestSuccess("($true)");
            TestSuccess("($false)");
            TestSuccess("(($true))");
            TestSuccess("((($false)))");

            TestSuccess("(a & b)");
            TestSuccess("(a | b)");
            TestSuccess("(a => b)");
            TestSuccess("(a <=> b)");

            TestSuccess("((a & b))");
            TestSuccess("((a | b))");
            TestSuccess("((a => b))");
            TestSuccess("((a <=> b))");

            TestSuccess("(a & (b | c))");
            TestSuccess("((a & b) | c)");
            TestSuccess("(a | (b & c))");
            TestSuccess("((a | b) & c)");
            TestSuccess("(a => (b => c))");
            TestSuccess("((a => b) => c)");
            TestSuccess("(a <=> (b <=> c))");
            TestSuccess("((a <=> b) <=> c)");
        }

        [Fact]
        public void UnaryNegation()
        {
            TestSuccess("~a");
            TestSuccess("~b");
            TestSuccess("~x");
            TestSuccess("~$true");
            TestSuccess("~$false");
            TestSuccess("~(a)");
            TestSuccess("~(a & b)");
            TestSuccess("~(a | b)");
            TestSuccess("~(a => b)");
            TestSuccess("~(a <=> b)");
            TestSuccess("~(![x]:a)");
            TestSuccess("~(?[x]:a)");

            TestSuccess("(~a)");
            TestSuccess("(~$true)");
            TestSuccess("(~(a & b))");
            TestSuccess("(~(a | b))");
            TestSuccess("(~(a => b))");
            TestSuccess("(~(a <=> b))");

            TestSuccess("~~a");
            TestSuccess("~~~a");
            TestSuccess("~~~~a");
            TestSuccess("~~~~~a");
            TestSuccess("~(~a)");
            TestSuccess("~(~(~a))");
            TestSuccess("~~(a & b)");
            TestSuccess("~(~(a | b))");
        }

        [Fact]
        public void Conjunction()
        {
            TestSuccess("a & b");
            TestSuccess("a & c");
            TestSuccess("x & y");
            TestSuccess("$true & $false");
            TestSuccess("$false & $true");

            TestSuccess("a & b & c");
            TestSuccess("a & b & c & d");
            TestSuccess("a & b & c & d & e");

            TestSuccess("a & b & c & d & e & f");
            TestSuccess("a & b & c & d & e & f & g & h");

            TestSuccess("a & $true");
            TestSuccess("$true & a");
            TestSuccess("a & $false");
            TestSuccess("$false & a");
            TestSuccess("a & ~b");
            TestSuccess("~a & b");
            TestSuccess("~a & ~b");
            TestSuccess("~a & $true");
            TestSuccess("$false & ~a");

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
            TestSuccess("$true | $false");
            TestSuccess("$false | $true");

            TestSuccess("a | b | c");
            TestSuccess("a | b | c | d");
            TestSuccess("a | b | c | d | e");

            TestSuccess("a | b | c | d | e | f");
            TestSuccess("a | b | c | d | e | f | g | h");

            TestSuccess("a | $true");
            TestSuccess("$true | a");
            TestSuccess("a | $false");
            TestSuccess("$false | a");
            TestSuccess("a | ~b");
            TestSuccess("~a | b");
            TestSuccess("~a | ~b");

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
            TestSuccess("a => b");
            TestSuccess("$true => a");
            TestSuccess("a => $true");
            TestSuccess("$false => b");
            TestSuccess("b => $false");

            TestSuccess("~a => b");
            TestSuccess("a => ~b");
            TestSuccess("~a => ~b");

            TestSuccess("a & b => c");
            TestSuccess("a => b & c");
            TestSuccess("a | b => c");
            TestSuccess("a => b | c");

            TestSuccess("a & b | c => d");
            TestSuccess("a => b & c | d");
            TestSuccess("a | b & c => d | e");

            TestSuccess("(a => b)");
            TestSuccess("((a => b))");
            TestSuccess("(a & b) => c");
            TestSuccess("a => (b & c)");
            TestSuccess("(a | b) => c");
            TestSuccess("a => (b | c)");
        }

        [Fact]
        public void Equivalence()
        {
            TestSuccess("a <=> b");
            TestSuccess("$true <=> a");
            TestSuccess("a <=> $false");
            TestSuccess("~a <=> b");

            TestSuccess("a & b <=> c");
            TestSuccess("a | b <=> c");
            TestSuccess("a => b <=> c");
            TestSuccess("a <=> b => c");

            TestSuccess("a & b | c <=> d");
            TestSuccess("a => b & c <=> d");
            TestSuccess("a | b => c | d <=> e");

            TestSuccess("(a <=> b)");
            TestSuccess("((a <=> b))");
            TestSuccess("(a => b) <=> c");
            TestSuccess("a <=> (b => c)");
            TestSuccess("(a & b) <=> (c | d)");

            TestSuccess("(a <=> b) <=> c");
            TestSuccess("a <=> (b <=> c)");
            TestSuccess("(a <=> b) <=> (c <=> d)");

            TestFailure("a <=> b <=> c");
            TestFailure("a <=> b <=> c <=> d");
        }

        [Fact]
        public void ImplicationAssociativityAndRestriction()
        {
            TestSuccess("a => b");
            TestSuccess("$true => a");
            TestSuccess("a => $true");
            TestSuccess("$false => b");
            TestSuccess("b => $false");

            TestSuccess("~a => b");
            TestSuccess("a => ~b");
            TestSuccess("~a => ~b");

            TestSuccess("a & b => c");
            TestSuccess("a => b & c");
            TestSuccess("a | b => c");
            TestSuccess("a => b | c");

            TestSuccess("a & b | c => d");
            TestSuccess("a => b & c | d");
            TestSuccess("a | b & c => d | e");

            TestFailure("a => b => c");
            TestFailure("a => b => c => d");

            TestSuccess("(a => b) => c");
            TestSuccess("a => (b => c)");
            TestSuccess("((a => b) => c)");
            TestSuccess("(a => (b => c))");

            TestSuccess("(a => b) => (c => d)");
            TestFailure("(a => b => c)");
        }

        [Fact]
        public void EquivalenceNesting()
        {
            TestSuccess("(a <=> b) <=> c");
            TestSuccess("a <=> (b <=> c)");
            TestSuccess("(a <=> b) <=> (c <=> d)");
            TestSuccess("((a <=> b) <=> c) <=> d");
            TestSuccess("a <=> (b <=> (c <=> d))");

            TestSuccess("a <=> (b => c)");
            TestSuccess("(a => b) <=> c");
            TestSuccess("(a <=> b) => c");
            TestSuccess("a => (b <=> c)");
        }

        [Fact]
        public void QuantifierBasics()
        {
            TestSuccess("![x]:a");
            TestSuccess("![x]:b");
            TestSuccess("![x]:$true");
            TestSuccess("![x]:$false");

            TestSuccess("?[x]:a");
            TestSuccess("?[x]:b");
            TestSuccess("?[x]:$true");
            TestSuccess("?[x]:$false");

            TestSuccess("![x,y]:a");
            TestSuccess("![x,y,z]:a");
            TestSuccess("![x,y,z,w]:a");

            TestSuccess("?[x,y]:a");
            TestSuccess("?[x,y,z]:a");
            TestSuccess("?[x,y,z,w]:a");

            TestSuccess("![x,x]:a");
            TestSuccess("?[x,x]:a");
            TestSuccess("![x,y,x]:a");
            TestSuccess("?[x,y,y]:a");
        }

        [Fact]
        public void QuantifierVariableNames()
        {
            TestSuccess("![x1]:a");
            TestSuccess("![x_1]:a");
            TestSuccess("![foo]:a");
            TestSuccess("![foo_bar]:a");
            TestSuccess("![Foo]:a");
            TestSuccess("![ABC123]:a");
            TestSuccess("![a_b_c]:a");

            TestSuccess("![x1,y2,z3]:a");
            TestSuccess("![foo,bar,baz]:a");
            TestSuccess("![a_1,b_2,c_3]:a");
            TestSuccess("![A,B,C]:a");
        }

        [Fact]
        public void QuantifierNesting()
        {
            TestSuccess("![x]:![y]:a");
            TestSuccess("![x]:?[y]:a");
            TestSuccess("?[x]:![y]:a");
            TestSuccess("?[x]:?[y]:a");

            TestSuccess("![x,y]:?[z]:a");
            TestSuccess("?[x,y]:![z]:a");
            TestSuccess("![x]:?[y,z]:a");
            TestSuccess("?[x]:![y,z]:a");

            TestSuccess("![a]:![b]:![c]:a");
            TestSuccess("![a]:?[b]:![c]:a");
            TestSuccess("?[a]:![b]:?[c]:a");
            TestSuccess("?[a]:?[b]:?[c]:a");
        }

        [Fact]
        public void QuantifiersWithNegation()
        {
            TestSuccess("![x]:~a");
            TestSuccess("?[x]:~a");

            TestSuccess("~![x]:a");
            TestSuccess("~?[x]:a");

            TestSuccess("![x]:~(a & b)");
            TestSuccess("?[x]:~(a | b)");

            TestSuccess("~![x]:(a & b)");
            TestSuccess("~?[x]:(a | b)");

            TestSuccess("![x]:~![y]:a");
            TestSuccess("?[x]:~?[y]:a");
            TestSuccess("![x]:~?[y]:a");
            TestSuccess("?[x]:~![y]:a");
        }

        [Fact]
        public void QuantifierBodyPrecedence()
        {
            TestSuccess("![x]:a & b");
            TestSuccess("![x]:a | b");
            TestSuccess("![x]:a => b");
            TestSuccess("![x]:a <=> b");

            TestSuccess("![x]:~a & b");
            TestSuccess("![x]:~a | b");
            TestSuccess("![x]:~a => b");
            TestSuccess("![x]:~a <=> b");

            TestSuccess("![x]:(a & b)");
            TestSuccess("![x]:(a | b)");
            TestSuccess("![x]:(a => b)");
            TestSuccess("![x]:(a <=> b)");

            TestSuccess("(![x]:a) & b");
            TestSuccess("(![x]:a) | b");
            TestSuccess("(![x]:a) => b");
            TestSuccess("(![x]:a) <=> b");
        }

        [Fact]
        public void QuantifiersWithFullExpressions()
        {
            TestSuccess("![x]:(a & b)");
            TestSuccess("![x]:(a | b)");
            TestSuccess("![x]:(a => b)");
            TestSuccess("![x]:(a <=> b)");

            TestSuccess("?[x]:(a & b)");
            TestSuccess("?[x]:(a | b)");
            TestSuccess("?[x]:(a => b)");
            TestSuccess("?[x]:(a <=> b)");

            TestSuccess("![x]:((a & b) | c)");
            TestSuccess("![x]:(a & (b | c))");
            TestSuccess("![x]:((a => b) & c)");
            TestSuccess("![x]:(a <=> (b | c))");
        }

        [Fact]
        public void FullPrecedenceStack()
        {
            TestSuccess("a & b | c => d <=> e");
            TestSuccess("a | b & c => d <=> e");
            TestSuccess("a => b | c & d <=> e");
            TestSuccess("a <=> b => c | d & e");

            TestSuccess("a & b | c & d => e | f & g <=> h");
            TestSuccess("a | b & c | d => e & f | g <=> h");
            TestSuccess("a & b & c | d | e & f => g | h & i <=> j");

            TestSuccess("~a & b | c => d <=> e");
            TestSuccess("a & ~b | ~c => d <=> ~e");
            TestSuccess("~(a & b) | c => ~(d | e) <=> f");
        }

        [Fact]
        public void QuantifiersMixedWithBinaryOperators()
        {
            TestSuccess("![x]:a & ![y]:b");
            TestSuccess("![x]:a | ?[y]:b");
            TestSuccess("![x]:a => ?[y]:b");
            TestSuccess("![x]:a <=> ?[y]:b");

            TestSuccess("(![x]:a) & (![y]:b)");
            TestSuccess("(![x]:a) | (?[y]:b)");
            TestSuccess("(![x]:a) => (?[y]:b)");
            TestSuccess("(![x]:a) <=> (?[y]:b)");

            TestSuccess("![x]:a & ?[y]:b | c");
            TestSuccess("![x]:a | ?[y]:b & c");
            TestSuccess("![x]:a => ?[y]:b | c");
            TestSuccess("![x]:a <=> ?[y]:(b | c)");
        }

        [Fact]
        public void QuantifierShadowing()
        {
            TestSuccess("![x]:![x]:x");
            TestSuccess("?[x]:?[x]:x");
            TestSuccess("![x]:?[x]:x");
            TestSuccess("?[x]:![x]:x");

            TestSuccess("![x,y]:![y,z]:a");
            TestSuccess("?[x,y]:?[x,z]:b");
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

            TestSuccess("![a]:![b]:![c]:![d]:a");
            TestSuccess("![a]:?[b]:![c]:?[d]:a");
            TestSuccess("?[a]:![b]:?[c]:![d]:a");

            TestSuccess("![a]:(![b]:((a & b) | c))");
            TestSuccess("?[a]:(![b]:((a | b) & c))");
            TestSuccess("![a]:(~(?[b]:(a => b)))");
            TestSuccess("?[a]:(~(![b]:(a <=> b)))");
        }

        [Fact]
        public void FullValidFormulas()
        {
            TestSuccess("![x,y]:~((x & y) | $false)");
            TestSuccess("?[x]:(~x => $true)");
            TestSuccess("![x]:(x <=> (~x | $false))");
            TestSuccess("![x,y]:((x & ~y) => (y | $false))");
            TestSuccess("?[x,y]:(((x | y) & ~(x & y)) <=> (x | y))");

            TestSuccess("![x]:((![y]:(x & y)) => (?[z]:(x | z)))");
            TestSuccess("?[x]:((![y]:(x => y)) <=> (~x | $true))");
            TestSuccess("![a,b,c]:((a & b) | (~b & c) => (a <=> c))");
            TestSuccess("?[x,y,z]:((x | y) & (~x | z) & (~y | ~z))");

            TestSuccess("![x]:(~(![y]:((x & y) => (x | $false))))");
            TestSuccess("?[x]:(~(?[y]:((x | y) <=> (x & $true))))");
        }

        [Fact]
        public void LongFormulas()
        {
            TestSuccess("a & b & c & d & e & f & g & h & i & j");
            TestSuccess("a | b | c | d | e | f | g | h | i | j");
            TestSuccess("a & b | c & d | e & f | g & h | i & j");
            TestSuccess("a | b & c | d & e | f & g | h & i | j");

            TestSuccess("![a,b,c]:((a & b) | (b & c) | (c & a))");
            TestSuccess("![a,b,c]:((a => b) & (b => c) & (c => a))");
        }

        [Fact]
        public void RandomLookingValidFormulas()
        {
            TestSuccess("![foo,bar]:((foo & ~bar) | $false)");
            TestSuccess("?[x1,y_2]:((x1 | y_2) => ~(x1 & y_2))");
            TestSuccess("A1_B2 <=> (~foo | ?[bar_baz]:$true)");
            TestSuccess("![x,y,z]:((x => y) & (y => z) & ~(x <=> z))");
            TestSuccess("?[a1,b2]:(~((a1 | $false) & (b2 => $true)))");
            TestSuccess("![foo_1,Bar2]:((foo_1 & Bar2) | (![x]:x))");
            TestSuccess("((a => b) & (~c | d)) <=> (![x,y]:(x & y))");
            TestSuccess("~(?[x]:(x <=> (![y]:(y => x))))");
        }

        [Fact]
        public void LexicalBoundaryTests()
        {
            TestFailure("$truex");
            TestFailure("$falsex");
            TestFailure("$true_foo");
            TestFailure("$false_foo");

            TestFailure("x$true");
            TestFailure("x$false");
            TestFailure("a$b");
        }

        [Fact]
        public void InvalidVariableNames()
        {
            TestFailure("1");
            TestFailure("123");
            TestFailure("1abc");
            TestFailure("_abc");
            TestFailure("__abc");
            TestFailure("_1");
            TestFailure("1_");
            TestFailure("1_abc");
            TestFailure("-abc");
            TestFailure("a-b");
            TestFailure("a.b");
            TestFailure("a+b");
            TestFailure("a=b");
            TestFailure("a/b");
            TestFailure("a b");
        }

        [Fact]
        public void InvalidConstants()
        {
            TestFailure("$True");
            TestFailure("$False");
            TestFailure("$TRUE");
            TestFailure("$FALSE");
            TestFailure("$tru");
            TestFailure("$fals");
            TestFailure("$truee");
            TestFailure("$falsee");
            TestFailure("$true1");
            TestFailure("$false1");
        }

        [Fact]
        public void InvalidQuantifiers()
        {
            TestFailure("![]:a");
            TestFailure("?[]:a");

            TestFailure("![x:a");
            TestFailure("?[x:a");

            TestFailure("!x]:a");
            TestFailure("?x]:a");

            TestFailure("![x]a");
            TestFailure("?[x]a");

            TestFailure("![x]:");
            TestFailure("?[x]:");

            TestFailure("![x,]:a");
            TestFailure("?[x,]:a");

            TestFailure("![,x]:a");
            TestFailure("?[,x]:a");

            TestFailure("![x,,y]:a");
            TestFailure("?[x,,y]:a");

            TestFailure("![x,y,]:a");
            TestFailure("?[x,y,]:a");
        }

        [Fact]
        public void InvalidQuantifierPunctuation()
        {
            TestFailure("![x,y:a");
            TestFailure("![x,y]a");
            TestFailure("![x,y]::a");
            TestFailure("![x,y]::");
            TestFailure("![x y]:a");
            TestFailure("![x; y]:a");
            TestFailure("![x | y]:a");

            TestFailure("?[x,y:a");
            TestFailure("?[x,y]a");
            TestFailure("?[x,y]::a");
            TestFailure("?[x y]:a");
            TestFailure("?[x; y]:a");
        }

        [Fact]
        public void InvalidOperators()
        {
            TestFailure("=");
            TestFailure("==");
            TestFailure("===");
            TestFailure(">");
            TestFailure("<");
            TestFailure("<=");
            TestFailure(">=");
            TestFailure("<>");
            TestFailure("!=");

            TestFailure("&&");
            TestFailure("||");

            TestFailure("=");
            TestFailure("<");
            TestFailure("<<=>");
            TestFailure("<==");
            TestFailure("==>");
        }

        [Fact]
        public void MissingOperands()
        {
            TestFailure("&");
            TestFailure("|");
            TestFailure("=>");
            TestFailure("<=>");

            TestFailure("a &");
            TestFailure("a |");
            TestFailure("a =>");
            TestFailure("a <=>");

            TestFailure("& a");
            TestFailure("| a");
            TestFailure("=> a");
            TestFailure("<=> a");
        }

        [Fact]
        public void ConsecutiveBinaryOperators()
        {
            TestFailure("a & & b");
            TestFailure("a | | b");
            TestFailure("a & | b");
            TestFailure("a | & b");

            TestFailure("a => => b");
            TestFailure("a <=> <=> b");

            TestFailure("a => | b");
            TestFailure("a | => b");
            TestFailure("a & => b");
            TestFailure("a => & b");
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
            TestFailure("(=>)");
            TestFailure("(<=>)");
        }

        [Fact]
        public void BadNegation()
        {
            TestFailure("~");
            TestFailure("~~");
            TestFailure("~&");
            TestFailure("~|");
            TestFailure("~=>");
            TestFailure("~<=>");
            TestFailure("~)");
            TestFailure("~]");
        }

        [Fact]
        public void WhitespaceTests()
        {
            TestSuccess("a&b");
            TestSuccess("a &b");
            TestSuccess("a& b");
            TestSuccess("a & b");

            TestSuccess("a=>b");
            TestSuccess("a =>b");
            TestSuccess("a=> b");
            TestSuccess("a => b");

            TestSuccess("a<=>b");
            TestSuccess("a <=>b");
            TestSuccess("a<=> b");
            TestSuccess("a <=> b");

            TestSuccess("![x]:a");
            TestSuccess("! [x] : a");
            TestSuccess("! [ x ] : a");
            TestSuccess("![ x,y ] : a");
            TestSuccess("![x , y] : a");

            TestSuccess("~a");
            TestSuccess("~ a");
            TestSuccess("~  a");

            TestSuccess("(a&b)");
            TestSuccess("( a & b )");
        }

        [Fact]
        public void WhitespaceInsideTokens()
        {
            TestFailure("$ true");
            TestFailure("$ t r u e");
            TestFailure("$ f alse");

            TestFailure("< =");
            TestFailure("= >");
            TestFailure("< = >");

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
            TestFailure("x]");
            TestFailure("x)");
            TestFailure("x~");
            TestFailure("x&");
            TestFailure("x|");
            TestFailure("x=>");
            TestFailure("x<=>");

            TestFailure("!x");
            TestFailure("?x");
            TestFailure("![x]");
            TestFailure("?[x]");
        }

        [Fact]
        public void CompleteConsumption()
        {
            TestSuccess("a");
            TestSuccess("a & b");
            TestSuccess("(a)");
            TestSuccess("![x]:a");

            TestFailure("a b");
            TestFailure("a & b c");
            TestFailure("(a) b");
            TestFailure("![x]:a b");
            TestFailure("a <=> b c");

            TestFailure("a)");
            TestFailure("(a))");
            TestFailure("((a)))");
        }
    }
}
