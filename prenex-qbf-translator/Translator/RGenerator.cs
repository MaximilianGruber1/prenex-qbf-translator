using prenex_qbf_translator.Language;

namespace prenex_qbf_translator.Translator
{
    public class RGenerator
    {
        private readonly BigTGenerator bigTGenerator = new();
        private readonly SmallTGenerator smallTGenerator = new();
        private readonly OutermostQuantifierDecomposer decomposer = new();

        public IFormula GenerateR(IFormula formula)
        {
            var decomp = decomposer.Decompose(formula);
            IFormula beta = decomp.Beta;
            Substitution substitution = decomp.Substitution;


            throw new NotImplementedException();
        }


        private IFormula GetReplacement(IQuantifier q)
        {
            IEnumerable<Variable> x = q.BoundVariables;
            IFormula phi = q.Inner;
            IEnumerable<Variable> n = smallTGenerator.GetN(phi);

            throw new NotImplementedException();
        }
    }
}
