using prenex_qbf_translator.ExponentialPrenexing;
using prenex_qbf_translator.Language;
using prenex_qbf_translator.Parsing;
using prenex_qbf_translator.PolynomialPrenexing;
using System.Reflection.Emit;
using System.Runtime.Intrinsics.Wasm;
using System.CommandLine;
using prenex_qbf_translator.TestFormulaGenerator;
using prenex_qbf_translator.TestFormulaGenerator.NQuantifiers;


public partial class Program
{
    

    public static int Main(string[] args)
    {
        var rootCommand = new RootCommand("prenex qbf formulas");

        // pol
        var polCommand = new Command(
            "pol",
            "prenex a formula using the polynomial approach");

        var polInput = new Argument<FileInfo?>("input")
        {
            Description = "input file (default: stdin)",
            Arity = ArgumentArity.ZeroOrOne
        };

        var polOutput = new Option<FileInfo?>("-o")
        {
            Description = "output file (default: stdout)"
        };

        polCommand.Add(polInput);
        polCommand.Add(polOutput);

        polCommand.SetAction(parseResult =>
        {
            var input = parseResult.GetValue(polInput);
            var output = parseResult.GetValue(polOutput);

            RunPol(input, output);

            return 0;
        });

        rootCommand.Add(polCommand);


        // exp
        var expCommand = new Command(
            "exp",
            "prenex a formula using the exponential approach");

        var expInput = new Argument<FileInfo?>("input")
        {
            Description = "input file (default: stdin)",
            Arity = ArgumentArity.ZeroOrOne
        };

        var expOutput = new Option<FileInfo?>("-o")
        {
            Description = "output file (default: stdout)"
        };

        expCommand.Add(expInput);
        expCommand.Add(expOutput);

        expCommand.SetAction(parseResult =>
        {
            var input = parseResult.GetValue(expInput);
            var output = parseResult.GetValue(expOutput);

            RunExp(input, output);

            return 0;
        });

        rootCommand.Add(expCommand);


        // gen
        var genCommand = new Command(
            "gen",
            "generate a test formula");

        var genN = new Argument<int>("n")
        {
            Description = "subformulas"
        };

        var genQ = new Argument<int>("q")
        {
            Description = "quantified variables per subformula"
        };

        var genF = new Argument<int>("f")
        {
            Description = "free variables per subformula"
        };

        var genOutput = new Option<FileInfo?>("-o")
        {
            Description = "output file (default: stdout)"
        };

        genCommand.Add(genN);
        genCommand.Add(genQ);
        genCommand.Add(genF);
        genCommand.Add(genOutput);

        genCommand.SetAction(parseResult =>
        {
            var n = parseResult.GetValue(genN);
            var q = parseResult.GetValue(genQ);
            var f = parseResult.GetValue(genF);
            var output = parseResult.GetValue(genOutput);

            RunGen(n, q, f, output);

            return 0;
        });

        rootCommand.Add(genCommand);

        return rootCommand.Parse(args).Invoke();
    }


    private static void RunPol(FileInfo? input, FileInfo? output)
    {
        using TextReader reader = input is null
            ? Console.In
            : input.OpenText();

        string fileText = reader.ReadToEnd();

        IFormula formula = new Parser(fileText).Parse();
        IFormula prenexedFormula = new PolynomialPrenexer().Prenex(formula);
        string result = prenexedFormula.ToString()!;

        using TextWriter writer = output is null
            ? Console.Out
            : new StreamWriter(output.FullName);

        writer.WriteLine(result);
    }

    private static void RunExp(FileInfo? input, FileInfo? output)
    {
        using TextReader reader = input is null
            ? Console.In
            : input.OpenText();

        string fileText = reader.ReadToEnd();

        IFormula formula = new Parser(fileText).Parse();
        IFormula prenexedFormula = new ExponentialPrenexer().Prenexed(formula);
        string result = prenexedFormula.ToString()!;

        using TextWriter writer = output is null
            ? Console.Out
            : new StreamWriter(output.FullName);

        writer.WriteLine(result);
    }

    private static void RunGen(int n, int q, int f, FileInfo? output)
    {
        IFormula formula = new RandomQuantifiersAndTerms().GenerateFormula2(n, q, f, new Random());
        string fString = formula.ToString()!;

        using TextWriter writer = output is null
            ? Console.Out
            : new StreamWriter(output.FullName);

        writer.WriteLine(fString);
    }
}
