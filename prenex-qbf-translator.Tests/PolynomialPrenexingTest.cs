
using Xunit;
using prenex_qbf_translator.Parsing;
using prenex_qbf_translator.Language;
using prenex_qbf_translator.PolynomialPrenexing;
using prenex_qbf_translator.ExponentialPrenexing;

namespace prenex_qbf_translator.Tests
{
    public class PolynomialPrenexingTest
    {
        private void TestFormula(string formula, string prenexedFormula)
        {
            string expected = new Parser(prenexedFormula).Parse().ToString(); // to standardize (whitespace and parentheses)

            Parser p = new(formula);
            IFormula f = p.Parse();
            IFormula TExists = new PolynomialPrenexer().Prenex(f);
            string actual = TExists.ToString();

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestBooleanFormulas() // for a boolean formula phi, Texists(phi) = phi
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
                "#ap #p1 ((p1 <-> ap) & (p1 -> (ap <-> am)) -> p1)"
                );
            TestFormula(
                "?a a",
                "#ap #p1 ((p1 <-> ap) & (!p1 -> (ap <-> am)) -> p1)"
                );
        }

        [Fact]
        public void TestQuantifierWithBooleanOperator()
        {
            TestFormula(
                "#a#b (a | b)",
                "#ap #bp #p1 (" +
                "(" +
                "p1 <-> ap | bp) & " +
                "(p1 -> (ap <-> am) & (bp <-> bm)) " +
                "-> " +
                "p1" +
                ")"
                );
            TestFormula(
                "?a?b (a | b)",
                "#ap #bp #p1 " +
                "(" +
                "(p1 <-> ap | bp) & " +
                "(!p1 -> (ap <-> am) & (bp <-> bm)) " +
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

                "#xp #yp #p1 #p2 #x1m ?x1p ?p3" +
                "(" +
                "(p3 <-> xi) &" +
                "(!p3 -> (x1p <-> x1m)) &" +
                    "(" +
                    "(p1 <-> psi & !p3) &" +
                    "(p2 <-> rho) &" +
                    "(!p1 -> (xp <-> xm)) &" +
                    "(p2 -> (yp <-> ym))" +
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

                "#ap #p1 #bm ?bp ?p2 ?cm #cp #p3 " +
                "    (" +
                "    (p3 <-> ap | bp | cp)  &  (p3 -> (cp <-> cm))" +
                "    ->" +
                "    (p2 <-> p3)  &  (!p2 -> (bp <-> bm)) &" +
                "    ((p1 <-> p2)  &  (p1 -> (ap <-> am))    ->    p1)" +
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
                "#ap #bp #cp #dp #ep #pp #qp #rp #sp #tp #p1 #p2 #p3 #p4" +
                "(" +
                "    (p1 <-> ap <- bp) &" +
                "    (p2 <-> cp | dp | ep) &" +
                "    (p3 <-> pp -> qp) &" +
                "    (p4 <-> rp <-> sp <-> tp)" +
                "    &" +
                "    (!p1 -> (ap <-> am) & (bp <-> bm)) &" +
                "    (!p2 -> (cp <-> cm) & (dp <-> dm) & (ep <-> em)) &" +
                "    (p3  -> (pp <-> pm) & (qp <-> qm)) &" +
                "    (p4  -> (rp <-> rm) & (sp <-> sm) & (tp <-> tm))" +
                "    ->" +
                "    (p1 & -p2 & z  ->  (p3 <-> -p4))" +
                ")"
                );
        }

        [Fact]
        public void PaperExampleChangedTo3VariablesPerQuantifier()
        {
            TestFormula(
                "?a ?b ?c " +
                "(" +
                "    (a|b|c) & " +
                "    ! ?p ?q ?r (p|q|r)" +
                ")" +
                "&" +
                "! #x #y #z (x|y|z)"
                ,

                "#ap #bp #cp #xp #yp #zp #p1 #p2 #pm #qm #rm" +
                "?pp ?qp ?rp ?p3" +
                "(" +
                "    (p3 <-> pp|qp|rp)" +
                "    &" +
                "    (!p3 -> (pp <-> pm) & (qp <-> qm) & (rp <-> rm))" +
                "    &" +
                "    (" +
                "        (p1 <-> (ap|bp|cp) & !p3) &" +
                "        (p2 <-> (xp|yp|zp))" +
                "        &" +
                "        (!p1 -> (ap <-> am) & (bp <-> bm) & (cp <-> cm)) &" +
                "        (p2  -> (xp <-> xm) & (yp <-> ym) & (zp <-> zm))" +
                "        ->" +
                "        p1 & !p2" +
                "    )" +
                ")"
                );
        }

        [Fact]
        public void VariableNamingInsanityA()
        {
            TestFormula(
                "?a1 ?a12345 ?a2 " +
                "(" +
                "    (a1|a12345|a2) & " +
                "    ! ?a ?a1 ?a3 (a|a1|a3)" +
                ")" +
                "&" +
                "! #a #a1 #a6 (a|a1|a6)"
                ,

                "#ap #a1p #a2p #a3p #a4p #a5p #p1 #p2 #a6m #a7m #a8m" +
                "?a6p ?a7p ?a8p ?p3" +
                "(" +
                "    (p3 <-> a6p|a7p|a8p)" +
                "    &" +
                "    (!p3 -> (a6p <-> a6m) & (a7p <-> a7m) & (a8p <-> a8m))" +
                "    &" +
                "    (" +
                "        (p1 <-> (ap|a1p|a2p) & !p3) &" +
                "        (p2 <-> (a3p|a4p|a5p))" +
                "        &" +
                "        (!p1 -> (ap <-> am) & (a1p <-> a1m) & (a2p <-> a2m)) &" +
                "        (p2  -> (a3p <-> a3m) & (a4p <-> a4m) & (a5p <-> a5m))" +
                "        ->" +
                "        p1 & !p2" +
                "    )" +
                ")"
                );
        }

        [Fact]
        public void OneQuantifierManyVariables()
        {
            TestFormula(
                "?a?b?c(a|b|c|d|e|f|g|h|i) & (x|y|z)",

                "#ap #bp #cp #p1" +
                "(" +
                "    (p1 <-> ap|bp|cp|d|e|f|g|h|i)" +
                "    &" +
                "    (!p1 -> (ap <-> am) & (bp <-> bm) & (cp <-> cm))" +
                "    ->" +
                "    p1 & (x|y|z)" +
                ")"
                );
        }

        [Fact]
        public void VariableNamingInsanityP_NoNesting()
        {
            TestFormula(
                "?p ?p1 ?pm (p|p1|pm|p3|p2m) & (p2|p1p)",

                "#p3p #p4p #pmp #p4" +
                "(" +
                "    (p4 <-> p3p|p4p|pmp|p3|p2m)" +
                "    &" +
                "    (!p4 -> (p3p <-> p3m) & (p4p <-> p4m) & (pmp <-> pmm))" +
                "    ->" +
                "    p4 & (p2|p1p)" +
                ")"
                );
        }
    }
}
