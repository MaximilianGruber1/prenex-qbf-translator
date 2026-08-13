using prenex_qbf_translator.Language;

namespace prenex_qbf_translator.Translator
{
    /// <summary>
    /// Generates T_exists(phi) and T_forall(phi)for a given formula phi according to Definition 5.
    /// </summary>
    public class BigTGenerator
    {
        private readonly SmallTGenerator smallTGenerator = new();


        public IFormula GenerateBigTExists(IFormula formula, IEnumerable<Variable> unavailableVariables)
        {
            return GenerateBigT(formula, false, unavailableVariables);
        }

        public IFormula GenerateBigTForall(IFormula formula, IEnumerable<Variable> unavailableVariables)
        {
            return GenerateBigT(formula, true, unavailableVariables);
        }


        private IFormula GenerateBigT(IFormula formula, bool forall, IEnumerable<Variable> unavailableVariables)
        {
            var unav = new List<Variable>(unavailableVariables);
            unav.AddRange(formula.Variables());
            unav = unav.Distinct().ToList();

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
