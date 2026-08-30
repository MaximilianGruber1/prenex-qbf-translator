using prenex_qbf_translator.Language;

namespace prenex_qbf_translator.TestFormulaGenerator
{
    public class Attemp0_QuantifiersAtBottom : IFormulaGenerator
    {
        public IFormula GenerateFormula(int n)
        {
            if (n < 1) throw new ArgumentException();

            var gen = new VariableGenerator();

            var v = gen.Next();
            IFormula f = new Forall(v, v);

            for (int i = 1; i < n; i++)
            {
                var w = gen.Next();
                f = new Equivalent(f, new Exists(w, w));
            }

            return f;
        }
    }

}
