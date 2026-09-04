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
        rootCommand.Add(genCommand);

        var genACommand = new Command(
            "a",
            "with the outermost layer consisting of forall quantifiers");
        genCommand.Add(genACommand);

        var genECommand = new Command(
            "e",
            "with the outermost layer consisting of exists quantifiers");
        genCommand.Add(genECommand);

        var genRCommand = new Command(
            "r",
            "with the outermost layer consisting of randomly mixed quantifiers");
        genCommand.Add(genRCommand);

        var genL = new Argument<int>("l")
        {
            Description = "quantifier layers"
        };
        genCommand.Add(genL);

        var genQ = new Argument<int>("q")
        {
            Description = "quantifiers per layer"
        };
        genCommand.Add(genQ);

        var genS = new Argument<int>("s")
        {
            Description = "subformulas"
        };
        genCommand.Add(genS);

        var genOutput = new Option<FileInfo?>("-o")
        {
            Description = "output file (default: stdout)"
        };
        genCommand.Add(genOutput);

        var genSeed = new Option<int?>("-s")
        {
            Description = "seed"
        };
        genCommand.Add(genSeed);

        genACommand.SetAction(parseResult =>
        {
            int l = parseResult.GetValue(genL);
            int q = parseResult.GetValue(genQ);
            int s = parseResult.GetValue(genS);
            int? seed = parseResult.GetValue(genSeed);
            FileInfo? output = parseResult.GetValue(genOutput);

            RunGen(Layer.Forall, l, q, s, seed, output);

            return 0;
        });

        genECommand.SetAction(parseResult =>
        {
            int l = parseResult.GetValue(genL);
            int q = parseResult.GetValue(genQ);
            int s = parseResult.GetValue(genS);
            int? seed = parseResult.GetValue(genSeed);
            FileInfo? output = parseResult.GetValue(genOutput);

            RunGen(Layer.Exists, l, q, s, seed, output);

            return 0;
        });

        genRCommand.SetAction(parseResult =>
        {
            int l = parseResult.GetValue(genL);
            int q = parseResult.GetValue(genQ);
            int s = parseResult.GetValue(genS);
            int? seed = parseResult.GetValue(genSeed);
            FileInfo? output = parseResult.GetValue(genOutput);

            RunGen(Layer.Random, l, q, s, seed, output);

            return 0;
        });


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

    private static void RunGen(Layer firstLayer, int layers, int quantifiersPerLayer, int subformulas, int? seed, FileInfo? output)
    {
        RandomQuantifiersAndTerms generator = new();
        IFormula formula = generator.GenerateFormula(layers, quantifiersPerLayer, firstLayer, subformulas, seed);
        string fString = formula.ToString()!;

        using TextWriter writer = output is null
            ? Console.Out
            : new StreamWriter(output.FullName);

        writer.WriteLine(fString);
    }
}
