using prenex_qbf_translator.Language;

namespace prenex_qbf_translator.Translator
{
    /// <summary>
    /// Implements the polynomial prenexing approach described in the paper.
    /// </summary>
    public class PolynomialPrenexer
    {
        private readonly SmallTGenerator smallTGenerator = new();

        /// <summary>
        /// Generates T_exists according to Definition 5. This method is used to prenex a formula for Limboole.
        /// </summary>
        /// <param name="formula"></param>
        /// <returns></returns>
        public IFormula Prenexed(IFormula formula)
        {
            formula = formula.DeepCopy();

            var unav = formula.Variables().ToList();

            return GenerateBigTRecursive(formula, forall: false, unav);
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
