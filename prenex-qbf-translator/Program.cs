using prenex_qbf_translator.Language;
using prenex_qbf_translator.Translator;


public class Program
{
    

    public static void Main(string[] args)
    {

        IFormula phi = 
            new And([
                new Exists([new Variable("x")],
                    new And([
                        new Variable("psi"),
                        new Not(
                            new Exists([new Variable("x")], new Variable("xi")))
                    ])
                ),
                new Not(
                    new Forall([new Variable("y")],
                        new Variable("rho")
                    )
                )]
            );
        TestFormula(phi);


        var phi1 =
            new And(
                new Variable("psi"),
                new Not(
                    new Exists(new Variable("x"), new Variable("xi")))
                );
        TestFormula(phi1);



        IFormula booleanFormula = new And([new Variable("a"), new Or([new Variable("b"), new Variable("c")])]);
        TestFormula(booleanFormula);


        Variable x = new Variable("x");
        Variable y = new Variable("y");
        Variable z = new Variable("z");
        IFormula singleQuantifierFormula =
            new Forall(
                x, y, new Or(x, y, z)
                );
        TestFormula(singleQuantifierFormula);

    }

    private static void TestFormula(IFormula phi)
    {
        Console.WriteLine(phi);
        var Args = new OutermostQuantifierDecomposer().Decompose(phi, []);
        Console.WriteLine(Args);
        Console.WriteLine("tExists: " + new SmallTGenerator().GenerateSmallTExists(phi));
        Console.WriteLine("tForall: " + new SmallTGenerator().GenerateSmallTForall(phi));
        Console.WriteLine("P: " + string.Join(", ", new SmallTGenerator().GetP(phi)));
        Console.WriteLine("N: " + string.Join(", ", new SmallTGenerator().GetN(phi)));
        IFormula TExists = new BigTGenerator().GenerateBigTExists(phi);
        Console.WriteLine("TExists: " + TExists);
        IFormula R = new RGenerator().GenerateR(phi);
        Console.WriteLine("R: " + R);
        Console.WriteLine("-----------------------------------------------------------");
    }

}
