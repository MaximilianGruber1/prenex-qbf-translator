using prenex_qbf_translator.Language;

namespace prenex_qbf_translator.Translator
{
    /// <summary>
    /// Implements the polynomial prenexing approach described in the paper. Use GenerateBigTExists to prenex a formula for Limboole.
    /// </summary>
    public class BigTGenerator
    {
        private readonly SmallTGenerator smallTGenerator = new();

        /// <summary>
        /// Generates T_exists according to Definition 5.
        /// </summary>
        /// <param name="formula"></param>
        /// <returns></returns>
        public IFormula GenerateBigTExists(IFormula formula)
        {
            return GenerateBigT(formula, false);
        }

        /// <summary>
        /// Generates T_forall according to Definition 5.
        /// </summary>
        /// <param name="formula"></param>
        /// <returns></returns>
        public IFormula GenerateBigTForall(IFormula formula)
        {
            return GenerateBigT(formula, true);
        }


        public IFormula GenerateBigT(IFormula formula, bool forall)
        {
            var unav = formula.Variables().ToList();

            return GenerateBigTRecursive(formula, forall, unav);
        }

        private IFormula GenerateBigTRecursive(IFormula phi, bool forall, List<Variable> unavailableVariables)
        {
            if (phi.IsBoolean())
                return phi;

            IFormula psi = smallTGenerator.GenerateSmallT(phi, forall, unavailableVariables);
            IEnumerable<Variable> pPhi = smallTGenerator.GetP(phi, unavailableVariables);
            IEnumerable<Variable> nPhi = smallTGenerator.GetN(phi, unavailableVariables);
            unavailableVariables.AddRange(pPhi); // add to unavailable variables to avoid variable capture
            unavailableVariables.AddRange(nPhi); // same

            IEnumerable<Variable> nPsi = smallTGenerator.GetN(psi, unavailableVariables);
            IFormula TPsi = GenerateBigTRecursive(psi, !forall, unavailableVariables);

            IEnumerable<Variable> quantifiedVariables = [.. pPhi, .. nPsi];
            if (forall)
                return new Exists(quantifiedVariables, TPsi);
            else
                return new Forall(quantifiedVariables, TPsi);

        }
    }
}
