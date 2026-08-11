using prenex_qbf_translator.Language;
using prenex_qbf_translator.Translator;


public class Program
{
    

    public static void Main(string[] args)
    {
        IFormula phi = 
            new And([
                new Exists([new Variable("p1")],
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

        var Args = OutermostQuantifierDecomposer.Decompose(phi, []);
        Console.WriteLine(Args);
    }

}
