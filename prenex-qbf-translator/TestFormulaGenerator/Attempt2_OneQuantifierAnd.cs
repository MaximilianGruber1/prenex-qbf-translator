using prenex_qbf_translator.Language;

namespace prenex_qbf_translator.TestFormulaGenerator
{
    public class Attempt2_OneQuantifierAnd : IFormulaGenerator
    {
        public IFormula GenerateFormula(int n)
        {
            var gen = new VariableGenerator();
            var v1 = gen.Next();
            IFormula f = new Exists(v1, v1);

            for (int i = 0; i < n; i++)
            {
                var e = gen.Next();
                var a = gen.Next();

                f = new Equivalent(f, e);
                f = new And(f, a);
            }

            return f;
        }
    }
}
