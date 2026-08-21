
using Xunit;
using prenex_qbf_translator.Parsing;
using prenex_qbf_translator.Language;
using prenex_qbf_translator.Translator;

namespace prenex_qbf_translator.Tests
{
    public class PrenexingTest
    {
        private void TestFormula(string formula, string prenexedFormula)
        {
            string expected = new Parser(new Scanner(prenexedFormula)).Parse().ToString(); // to standardize (whitespace and parentheses)
            
            Parser p = new(new Scanner(formula));
            IFormula f = p.Parse();
            IFormula TExists = new BigTGenerator().GenerateBigTExists(f);
            string actual = TExists.ToString();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestBooleanFormulas() // for a boolean formula phi, T_exists(phi) = phi
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
        public void TestQuantifierWithSingleVariable()
        {
            TestFormula(
                "# a a",
                "#a_p #p1 ((p1 <-> a_p) & (p1 -> (a_p <-> a_m)) -> p1)"
                );
            TestFormula(
                "?a a",
                "#a_p #p1 ((p1 <-> a_p) & (!p1 -> (a_p <-> a_m)) -> p1)"
                );
        }

        [Fact]
        public void TestQuantifierWithBooleanOperator()
        {
            TestFormula(
                "#a#b (a | b)",
                "#a_p #b_p #p1 (" +
                "(" +
                "p1 <-> a_p | b_p) & " +
                "(p1 -> (a_p <-> a_m) & (b_p <-> b_m)) " +
                "-> " +
                "p1" +
                ")"
                );
            TestFormula(
                "?a?b (a | b)",
                "#a_p #b_p #p1 " +
                "(" +
                "(p1 <-> a_p | b_p) & " +
                "(!p1 -> (a_p <-> a_m) & (b_p <-> b_m)) " +
                "-> " +
                "p1" +
                ")"
                );
        }

        [Fact]
        public void TestExampleFromPaper()
        {
            TestFormula(
                "?x (psi & !?x xi) & !#y rho",

                "#x_p #y_p #p1 #p2 #x1_m ?x1_p ?p3" +
                "(" +
                "(p3 <-> xi) &" +
                "(!p3 -> (x1_p <-> x1_m)) &" +
                    "(" +
                    "(p1 <-> psi & !p3) &" +
                    "(p2 <-> rho) &" +
                    "(!p1 -> (x_p <-> x_m)) &" +
                    "(p2 -> (y_p <-> y_m))" +
                    "->" +
                    "p1 & !p2" +
                    ")" +

                ")"
                );
        }

        

        [Fact]
        public void TripleNestedQuantifier()
        {

        }
    }
}
