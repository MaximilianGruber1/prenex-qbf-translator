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


        private Substitution GetRSubstitution(Substitution sub)
        {
            Dictionary<Variable, IFormula> dic = new();
            foreach (var entry in sub.Dictionary)
            {
                dic[entry.Key] = GetReplacement((IQuantifier)(entry.Value));
            }
            return new Substitution(dic);
        }

        private IFormula GetReplacement(IQuantifier q)
        {
            IEnumerable<Variable> x = q.BoundVariables;
            IFormula phi = q.Inner;
            IEnumerable<Variable> n = smallTGenerator.GetN(phi);
            IFormula T = bigTGenerator.GenerateBigT(phi, q is Forall);

            IEnumerable<Variable> boundVariables = x.Concat(n);

            return q.CreateCopy(boundVariables, T);
        }
    }
}
