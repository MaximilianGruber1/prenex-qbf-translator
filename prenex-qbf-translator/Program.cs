using prenex_qbf_translator.Language;
using prenex_qbf_translator.Parsing;
using prenex_qbf_translator.Translator;
using System.Runtime.Intrinsics.Wasm;


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


        var phi2or = new Or(new Variable("x"), new Variable("y"));
        var phi2sub = new Substitution(new Dictionary<Variable, IFormula> {
            { new Variable("psi"), phi2or },
            { new Variable("xi"), phi2or },
            { new Variable("rho"), phi2or } });
        var phi2 = phi.ApplySubstitution(phi2sub);
        TestFormula(phi2);


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


        Parser p = new(new Scanner("#a a"));
        IFormula f = p.Parse();
        Console.WriteLine(f);
        IFormula TExists = new BigTGenerator().GenerateBigTExists(f);
        Console.WriteLine(TExists);

    }

    private static void TestFormula(IFormula phi)
    {
        Console.WriteLine(phi);
        Console.WriteLine("IsBoolean: " + phi.IsBoolean());
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
