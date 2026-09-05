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

        var genOutput = new Option<FileInfo?>("-o", "--output")
        {
            Description = "output file (default: stdout)"
        };
        genCommand.Add(genOutput);

        var genA = new Option<bool>("-a", "--forall")
        {
            Description = "outermost quantifiers are 'forall'"
        };
        genCommand.Add(genA);
        
        var genE = new Option<bool>("-e", "--exists")
        {
            Description = "outermost quantifiers are 'exists'"
        };
        genCommand.Add(genE);

        var genSimplified = new Option<bool>("-s", "--simplified")
        {
            Description = "only uses the binary operators &, |, <->, and only negates variables"
        };
        genCommand.Add(genSimplified);

        var genSeed = new Option<int?>("--seed")
        {
            Description = "seed"
        };
        genCommand.Add(genSeed);

        genCommand.SetAction(parseResult =>
        {
            int l = parseResult.GetValue(genL);
            int q = parseResult.GetValue(genQ);
            int s = parseResult.GetValue(genS);
            FileInfo? output = parseResult.GetValue(genOutput);
            int? seed = parseResult.GetValue(genSeed);
            bool forall = parseResult.GetValue(genA);
            bool exists = parseResult.GetValue(genE);
            bool simplified = parseResult.GetValue(genSimplified);

            RunGen(l, q, s, output, seed, forall, exists, simplified);

            return 0;
        });


        // neg
        var negCommand = new Command(
            "neg",
            "negate a formula");

        var negInput = new Argument<FileInfo?>("input")
        {
            Description = "input file (default: stdin)",
            Arity = ArgumentArity.ZeroOrOne
        };

        var negOutput = new Option<FileInfo?>("-o")
        {
            Description = "output file (default: stdout)"
        };

        negCommand.Add(negInput);
        negCommand.Add(negOutput);

        negCommand.SetAction(parseResult =>
        {
            var input = parseResult.GetValue(negInput);
            var output = parseResult.GetValue(negOutput);

            RunNeg(input, output);

            return 0;
        });

        rootCommand.Add(negCommand);


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

    private static void RunGen(int layers, int quantifiersPerLayer, int subformulas, FileInfo? output, int? seed, bool forall, bool exists, bool simplified)
    {
        RandomQuantifiersAndTerms generator = new();
        IFormula formula = generator.GenerateFormula(layers, quantifiersPerLayer, subformulas, forall, exists, simplified, seed: seed);
        string fString = formula.ToString()!;

        using TextWriter writer = output is null
            ? Console.Out
            : new StreamWriter(output.FullName);

        writer.WriteLine(fString);
    }

    private static void RunNeg(FileInfo? input, FileInfo? output)
    {
        using TextReader reader = input is null
            ? Console.In
            : input.OpenText();

        string fileText = reader.ReadToEnd();

        IFormula formula = new Parser(fileText).Parse();
        formula = (formula is Not not) ? not.Inner : new Not(formula);
        string result = formula.ToString()!;

        using TextWriter writer = output is null
            ? Console.Out
            : new StreamWriter(output.FullName);

        writer.WriteLine(result);
    }
}
