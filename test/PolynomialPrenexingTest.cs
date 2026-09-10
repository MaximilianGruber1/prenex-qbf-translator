
using Xunit;
using eqprenex.Parsing;
using eqprenex.Language;
using eqprenex.PolynomialPrenexing;
using eqprenex.ExponentialPrenexing;

namespace eqprenex.Tests
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
        public void NoQuantifiers()
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
        public void QuantifierWithSingleVariable()
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
        public void QuantifierWithBooleanOperator()
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
        public void ExampleFromPaper()
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

        // gen 1 3 3 --seed 1
        [Fact]
        public void _133_Seed1()
        {
            TestFormula(
                "#a #b #c (!(!a <-> !(!b <-> !c)) <-> !d | !e | f) <-> " +
                "#a #b #c (a & !b & c <-> !(e & !(!d & f))) <-> " +
                "?a ?b ?c (!(!a & !(b <-> !c)) <-> !(!f <- (d -> !e)))",

                "#ap #bp #cp #a1p #b1p #c1p #a2p #b2p #c2p #p1 #p2 #p3" +
                "(" +
                "  (" +
                "    (p1 <-> (!(!ap <-> !(!bp <-> !cp)) <-> !d | !e | f)) &" +
                "    (p2 <-> (a1p & !b1p & c1p <-> !(e & !(!d & f)))) &" +
                "    (p3 <-> (!(!a2p & !(b2p <-> !c2p)) <-> !(!f <- (d -> !e))))" +
                "  )" +
                "  &" +
                "  (" +
                "    (p1 -> (ap <-> am) & (bp <-> bm) & (cp <-> cm)) &" +
                "    (p2 -> (a1p <-> a1m) & (b1p <-> b1m) & (c1p <-> c1m)) &" +
                "    (!p3 -> (a2p <-> a2m) & (b2p <-> b2m) & (c2p <-> c2m))" +
                "  )" +
                "  ->" +
                "  (" +
                "    p1 <-> p2 <-> p3" +
                "  )" +
                ")"
                );
        }

        // gen 2 2 2 --simplified --seed 0
        [Fact]
        public void _222_Seed0_Simplified()
        {
            TestFormula(
                "?a ?b #c #d (b <-> d | a & c <-> h | e & !f & g) " +
                "<-> " +
                "#a #b ?c ?d (c & d & (!a <-> b) <-> (!f <-> !g) & !e & !h)",
                
                "#ap #bp #a1p #b1p #p1 #p2 #cm #dm #c1m #d1m" +
                "?cp ?dp ?c1p ?d1p ?p3 ?p4" +
                "(" +
                "  (" +
                "    (p3 <-> (bp <-> dp | ap & cp)  <->  h | e & !f & g) &" +
                "    (p4 <-> c1p & d1p & (!a1p <-> b1p)  <->  (!f <-> !g) & !e & !h) &" +
                "    (p3 -> (cp <-> cm) & (dp <-> dm)) &" +
                "    (!p4 -> (c1p <-> c1m) & (d1p <-> d1m))" +
                "  )" +
                "  &" +
                "  (" +
                "    (p1 <-> p3) &" +
                "    (p2 <-> p4) &" +
                "    (!p1 -> (ap <-> am) & (bp <-> bm)) &" +
                "    (p2 -> (a1p <-> a1m) & (b1p <-> b1m))" +
                "    ->" +
                "    (p1 <-> p2)" +
                "  )" +
                ")")
                ;
        }

        // gen 3 1 2 --simplified --seed 2
        [Fact]
        public void _312_Seed2_Simplified()
        {
            TestFormula(
                "?a #b ?c (!b | a | !c <-> !f | (!d <-> e)) <-> " +
                "#a ?b #c (!b & !a & !c <-> !f | d & e)",
                
                "#ap #a1p #p1 #p2 #bm #b1m" +
                "?bp ?b1p ?p3 ?p4 ?cm ?c1m" +
                "#cp #c1p #p5 #p6 (" +
                "(" +
                "  (p5 <-> (!bp | ap | !cp <-> !f | (!d <-> e))) &" +
                "  (p6 <-> (!b1p & !a1p & !c1p <-> !f | d & e)) &" +
                "  (!p5 -> (cp <-> cm)) &" +
                "  (p6 -> (c1p <-> c1m))" +
                ")" +
                "->" +
                "(" +
                "  (p3 <-> p5) &" +
                "  (p4 <-> p6) &" +
                "  (p3 -> (bp <-> bm)) &" +
                "  (!p4 -> (b1p <-> b1m))" +
                ")" +
                "&" +
                "(" +
                "  (" +
                "    (p1 <-> p3) &" +
                "    (p2 <-> p4) &" +
                "    (!p1 -> (ap <-> am)) &" +
                "    (p2 -> (a1p <-> a1m))" +
                "  )" +
                "  ->" +
                "  (p1 <-> p2)" +
                "))"
                );
        }
    }
}
