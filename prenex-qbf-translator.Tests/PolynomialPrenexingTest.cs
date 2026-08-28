
using Xunit;
using prenex_qbf_translator.Parsing;
using prenex_qbf_translator.Language;
using prenex_qbf_translator.Translator;
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
            IFormula TExists = new PolynomialPrenexer().Prenexed(f);
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

                "#xp #yp #p1 #p2 #xm1 ?xp1 ?p3" +
                "(" +
                "(p3 <-> xi) &" +
                "(!p3 -> (xp1 <-> xm1)) &" +
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
                "?a?b?c " +
                "(" +
                "    (a|b|c) & " +
                "    ! ?p?q?r (p|q|r)" +
                ")" +
                "&" +
                "! #x#y#z (x|y|z)"
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
        public void VariableNamingInsanityA_IncludingNesting()
        {
            TestFormula(
                "?a ?ap1 ?app " +
                "(" +
                "    (a|ap1|app) & " +
                "    ! ?a ?am2 ?apm1 (a|am2|apm1)" +
                ")" +
                "&" +
                "! #ap #ap4 #a (ap|ap4|a)"
                ,

                "#ap3 #ap1p #appp #app2 #ap4p #ap5 #p1 #p2 #am6 #am2m #apm1m" +
                "?ap6 ?am2p ?apm1p ?p3" +
                "(" +
                "    (p3 <-> ap6|am2p|apm1p)" +
                "    &" +
                "    (!p3 -> (ap6 <-> am6) & (am2p <-> am2m) & (apm1p <-> apm1m))" +
                "    &" +
                "    (" +
                "        (p1 <-> (ap3|ap1p|appp) & !p3) &" +
                "        (p2 <-> (app2|ap4p|ap5))" +
                "        &" +
                "        (!p1 -> (ap3 <-> am3) & (ap1p <-> ap1m) & (appp <-> appm)) &" +
                "        (p2  -> (app2 <-> apm2) & (ap4p <-> ap4m) & (ap5 <-> am5))" +
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
                "?p ?p1 ?pm (p|p1|pm|p3|pm2) & (p2|pp1)",

                "#pp3 #p1p #pmp #p4" +
                "(" +
                "    (p4 <-> pp3|p1p|pmp|p3|pm2)" +
                "    &" +
                "    (!p4 -> (pp3 <-> pm3) & (p1p <-> p1m) & (pmp <-> pmm))" +
                "    ->" +
                "    p4 & (p2|pp1)" +
                ")"
                );
        }
    }
}
