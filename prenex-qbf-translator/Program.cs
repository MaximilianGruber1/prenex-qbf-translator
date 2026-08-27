using prenex_qbf_translator.Language;
using prenex_qbf_translator.Parsing;
using prenex_qbf_translator.Translator;
using System.Reflection.Emit;
using System.Runtime.Intrinsics.Wasm;


public class Program
{
    

    public static void Main(string[] args)
    {
        //TestFormula("?x (psi & !?x xi) & !#y rho");
        TestFormula("#a#b (a | b)");
    }

    private static void TestFormula(string formula)
    {
        var phi = ParseFormula(formula);
        Console.WriteLine(phi);
        Console.WriteLine("IsBoolean: " + phi.IsBoolean());
        var Args = new OutermostQuantifierDecomposer().GetDecomposition(phi, []);
        Console.WriteLine(Args);
        Console.WriteLine("tExists: " + new SmallTGenerator().GenerateSmallTExists(phi, []));
        Console.WriteLine("tForall: " + new SmallTGenerator().GenerateSmallTForall(phi, []));
        Console.WriteLine("P: " + string.Join(", ", new SmallTGenerator().GetP(phi, [])));
        Console.WriteLine("N: " + string.Join(", ", new SmallTGenerator().GetN(phi, [])));
        IFormula TExists = new BigTGenerator().GenerateBigTExists(phi);
        Console.WriteLine("TExists: " + TExists);
        Console.WriteLine("-----------------------------------------------------------");
    }

    private static IFormula ParseFormula(string s)
    {
        return new Parser(s).Parse();
    }

}
