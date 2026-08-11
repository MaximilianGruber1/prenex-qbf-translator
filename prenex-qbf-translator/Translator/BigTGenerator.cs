using prenex_qbf_translator.Language;

namespace prenex_qbf_translator.Translator
{
    /// <summary>
    /// Generates t_exists(phi), t_forall(phi), N(phi), and P(phi) for a given formula phi according to Definition 2.
    /// </summary>
    public static class BigTGenerator
    {
        public static IFormula GenerateTExists(IFormula formula)
        {
            throw new NotImplementedException();
        }

        public static IFormula GenerateTForall(IFormula formula)
        {
            throw new NotImplementedException();
        }

        public static IEnumerable< Variable> GetN(IFormula formula)
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<Variable> GetP(IFormula formula)
        {
            throw new NotImplementedException();
        }
    }
}
