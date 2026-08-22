
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
            TestFormula("#a ?b #c (a|b|c)",

                "#a_p #p1 #b_m ?b_p ?p2 ?c_m #c_p #p3 " +
                "    (" +
                "    (p3 <-> a_p | b_p | c_p)  &  (p3 -> (c_p <-> c_m))" +
                "    ->" +
                "    (p2 <-> p3)  &  (!p2 -> (b_p <-> b_m)) &" +
                "    ((p1 <-> p2)  &  (p1 -> (a_p <-> a_m))    ->    p1)" +
                ")");
        }

        [Fact]
        public void MultipleQuantifiersWithMultipleVariablesEach()
        {
            TestFormula(
                "?a?b (a<-b) &" +
                "!?c?d?e (c|d|e) & " +
                "z" +
                " ->" +
                "(" +
                "    #p#q (p -> q) <->" +
                "    !#r#s#t (r <-> s <-> t)" +
                ")"

                ,
                "#a_p #b_p #c_p #d_p #e_p #p_p #q_p #r_p #s_p #t_p #p1 #p2 #p3 #p4" +
                "(" +
                "    (p1 <-> a_p <- b_p) &" +
                "    (p2 <-> c_p | d_p | e_p) &" +
                "    (p3 <-> p_p -> q_p) &" +
                "    (p4 <-> r_p <-> s_p <-> t_p)" +
                "    &" +
                "    (!p1 -> (a_p <-> a_m) & (b_p <-> b_m)) &" +
                "    (!p2 -> (c_p <-> c_m) & (d_p <-> d_m) & (e_p <-> e_m)) &" +
                "    (p3  -> (p_p <-> p_m) & (q_p <-> q_m)) &" +
                "    (p4  -> (r_p <-> r_m) & (s_p <-> s_m) & (t_p <-> t_m))" +
                "    ->" +
                "    (p1 & -p2 & z  ->  (p3 <-> -p4))" +
                ")"
                );
        }

        [Fact]
        public void PaperExampleChangedTo3VariablesPerQuantifier()
        {
            TestFormula(
                "?a?b?c " +
                "(" +
                "    (a|b|c) & " +
                "    ! ?p?q?r (p|q|r)" +
                ")" +
                "&" +
                "! #x#y#z (x|y|z)"
                ,

                "#a_p #b_p #c_p #x_p #y_p #z_p #p1 #p2 #p_m #q_m #r_m" +
                "?p_p ?q_p ?r_p ?p3" +
                "(" +
                "    (p3 <-> p_p|q_p|r_p)" +
                "    &" +
                "    (!p3 -> (p_p <-> p_m) & (q_p <-> q_m) & (r_p <-> r_m))" +
                "    &" +
                "    (" +
                "        (p1 <-> (a_p|b_p|c_p) & !p3) &" +
                "        (p2 <-> (x_p|y_p|z_p))" +
                "        &" +
                "        (!p1 -> (a_p <-> a_m) & (b_p <-> b_m) & (c_p <-> c_m)) &" +
                "        (p2  -> (x_p <-> x_m) & (y_p <-> y_m) & (z_p <-> z_m))" +
                "        ->" +
                "        p1 & !p2" +
                "    )" +
                ")"
                );
        }
    }
}
