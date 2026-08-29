using prenex_qbf_translator.Language;

namespace prenex_qbf_translator.TestFormulaGenerator
{
    public interface IFormulaGenerator
    {
        /// <summary>
        /// Generates a formula with size n >= 1 following some pattern
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        IFormula GenerateFormula(int n);
    }
}
