using prenex_qbf_translator.ExponentialPrenexing;
using prenex_qbf_translator.Language;
using prenex_qbf_translator.Parsing;
using prenex_qbf_translator.Translator;
using System.Reflection.Emit;
using System.Runtime.Intrinsics.Wasm;
using System.CommandLine;


public class Program
{
    

    public static int Main(string[] args)
    {
        var rootCommand = new RootCommand("prenex qbf formulas");

        // pol
        var polCommand = new Command("pol", "prenex a formula using the polynomial approach");

        var polInput = new Argument<FileInfo>("input")
        {
            Description = "input file"
        };
        var polOutput = new Option<FileInfo?>("-o")
        {
            Description = "output file"
        };

        polCommand.Add(polInput);
        polCommand.Add(polOutput);

        polCommand.SetAction(parseResult =>
        {
            var input = parseResult.GetValue(polInput);
            var output = parseResult.GetValue(polOutput);

            RunPol(input!, output);

            return 0;
        });

        // exp
        var expCommand = new Command("exp", "prenex a formula using the exponential approach");

        var expInput = new Argument<FileInfo>("input")
        {
            Description = "input file"
        };
        var expOutput = new Option<FileInfo?>("-o")
        {
            Description = "output file"
        };

        expCommand.Add(expInput);
        expCommand.Add(expOutput);

        expCommand.SetAction(parseResult =>
        {
            var input = parseResult.GetValue(expInput);
            var output = parseResult.GetValue(expOutput);

            RunExp(input!, output);

            return 0;
        });

        // root
        rootCommand.Add(polCommand);
        rootCommand.Add(expCommand);

        return rootCommand.Parse(args).Invoke();
    }


    private static void RunPol(FileInfo input, FileInfo? output)
    {
        string fileText = File.ReadAllText(input.FullName);
        IFormula formula = new Parser(fileText).Parse();
        IFormula prenexedFormula = new PolynomialPrenexer().Prenexed(formula);
        string result = prenexedFormula.ToString()!;

        if (output != null)
        {
            File.WriteAllText(output.FullName, result);
        }
        else
        {
            Console.WriteLine(result);
        }
    }

    private static void RunExp(FileInfo input, FileInfo? output)
    {
        string fileText = File.ReadAllText(input.FullName);
        IFormula formula = new Parser(fileText).Parse();
        IFormula prenexedFormula = new ExponentialPrenexer().Prenexed(formula);
        string result = prenexedFormula.ToString()!;

        if (output != null)
        {
            File.WriteAllText(output.FullName, result);
        }
        else
        {
            Console.WriteLine(result);
        }
    }




    

}
