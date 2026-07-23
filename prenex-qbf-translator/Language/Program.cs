using prenex_qbf_translator.Language;


public class Program
{
    private class TestFormula : IFormula
    {
        private readonly string _repr;
        private readonly IEnumerable<Variable> _vars;
        private readonly IEnumerable<Variable> _freeVars;
        private readonly int _nBlocks;
        private readonly int _nQuantified;
        private readonly int _length;
        private readonly int _depth;
        private readonly IFormula _applySubstReturn;

        public TestFormula(
            string repr,
            IEnumerable<Variable>? vars = null,
            IEnumerable<Variable>? freeVars = null,
            int nBlocks = 0,
            int nQuantified = 0,
            int length = 1,
            int depth = 0,
            IFormula? applySubstReturn = null)
        {
            _repr = repr;
            _vars = vars ?? Enumerable.Empty<Variable>();
            _freeVars = freeVars ?? Enumerable.Empty<Variable>();
            _nBlocks = nBlocks;
            _nQuantified = nQuantified;
            _length = length;
            _depth = depth;
            _applySubstReturn = applySubstReturn ?? this;
        }

        public IFormula Negated() => new TestFormula($"~{_repr}");
        public IEnumerable<Variable> Variables() => _vars;
        public IEnumerable<Variable> FreeVariables() => _freeVars;
        public int NBlocks() => _nBlocks;
        public int NQuantifiedVariables() => _nQuantified;
        public int Length() => _length;
        public int QuantifierDepth() => _depth;
        public IFormula ApplySubstitution(Substitution substitution) => _applySubstReturn;
        public override string ToString() => _repr;
    }

    public static void Main(string[] args)
    {

        var f1 = new TestFormula("f1", nBlocks: 2);
        var f2 = new TestFormula("f2", nBlocks: 3);
        var and = new And(new IFormula[] { f1, f2 });
        Console.WriteLine(and.Length());
    }

}
