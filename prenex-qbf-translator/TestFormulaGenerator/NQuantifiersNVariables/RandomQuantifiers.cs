using prenex_qbf_translator.Language;

namespace prenex_qbf_translator.TestFormulaGenerator.NQuantifiers
{
    public class RandomQuantifiers
    {
        public IFormula GenerateTrue(int n, int seed)
        {
            return Generate(n, new Random(seed), isTrue: true);
        }

        public IFormula GenerateTrue(int n)
        {
            return Generate(n, new Random(), isTrue: true);
        }

        public IFormula GenerateFalse(int n, int seed)
        {

            return Generate(n, new Random(seed), isTrue: false);
        }

        public IFormula GenerateFalse(int n)
        {
            return Generate(n, new Random(), isTrue: false);
        }


        private IFormula Generate(int n, Random rng, bool isTrue)
        {
            var vargen = new VariableGenerator();
            Q[] qs = GenerateQuantifiers(n, rng, isTrue);

            var quantifiers = qs.Select(q =>
            {
                var v = vargen.Next();
                if (q == Q.A)
                    return (Quantifier)new Forall(v, v);
                else
                    return (Quantifier)new Exists(v, v);
            }).ToArray();

            return new Equivalent(quantifiers);
        }

        private Q[] GenerateQuantifiers(int n, Random rng, bool isTrue)
        {
            var quantifiers = new Q[n];
            int forallCount = 0;

            for (int i = 1; i < n; i++) // [0] is set at the end depending on isTrue
            {
                quantifiers[i] = rng.Next(2) == 1 ? Q.A : Q.E; // rng.Next(2) == 1 generates random bool

                if (quantifiers[i] == Q.A)
                    forallCount++;
            }

            // even number of foralls <=> formula is true
            Q first;
            if (forallCount % 2 == 0) // even
            {
                if (isTrue)
                    first = Q.E; // let it even
                else
                    first = Q.A; // make it odd
            }
            else // formula is currently false
            {
                if (isTrue)
                    first = Q.A;
                else
                    first = Q.E;
            }
            quantifiers[0] = first;
            return quantifiers;
        }

        private enum Q { E, A }
    }
}
