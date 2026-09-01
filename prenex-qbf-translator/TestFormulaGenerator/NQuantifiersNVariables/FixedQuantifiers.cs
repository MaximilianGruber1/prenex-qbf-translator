using prenex_qbf_translator.Language;

namespace prenex_qbf_translator.TestFormulaGenerator.NQuantifiers
{
    /// <summary>
    /// (?/#)a a <-> ?b b <-> ... <-> ?last last
    /// </summary>
    public class FixedQuantifiers
    {
        public IFormula GenerateTrue(int n) => GenerateFormula(n, true);

        public IFormula GenerateFalse(int n) => GenerateFormula(n, false);


        private IFormula GenerateFormula(int n, bool isTrue)
        {
            if (n < 1) throw new ArgumentException();

            var gen = new VariableGenerator();

            var v = gen.Next();
            IFormula f = isTrue ?
                new Exists(v, v) :
                new Forall(v, v);

            for (int i = 1; i < n; i++)
            {
                var w = gen.Next();
                f = new Equivalent(f, new Exists(w, w));
            }

            return f;
        }
    }
}
