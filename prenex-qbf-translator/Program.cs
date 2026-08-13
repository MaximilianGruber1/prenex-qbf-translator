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

        Console.WriteLine(phi);

        var Args = new OutermostQuantifierDecomposer().Decompose(phi, []);
        Console.WriteLine(Args);

        Console.WriteLine();
        var tExistsResult = new SmallTGenerator().GenerateTExists(phi, []);
        var tForallResult = new SmallTGenerator().GenerateTForall(phi, []);
        Console.WriteLine("tExists: " + tExistsResult.Formula);
        Console.WriteLine("tForall: " + tForallResult.Formula);
        Console.WriteLine("P: " + string.Join(", ", tExistsResult.P));
        Console.WriteLine("N: " + string.Join(", ", tExistsResult.N));

    }

}
