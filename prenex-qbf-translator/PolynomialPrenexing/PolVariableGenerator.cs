using prenex_qbf_translator.Language;

namespace prenex_qbf_translator.PolynomialPrenexing
{
    public class PolVariableGenerator
    {
        private readonly HashSet<string> unav;
        private int pIndex = 1;

        private readonly Dictionary<string, int> indices = new();

        public PolVariableGenerator(HashSet<Variable> unavailableVariables)
        {
            ArgumentNullException.ThrowIfNull(unavailableVariables, nameof(unavailableVariables));

            unav = unavailableVariables.Select(v => v.Name).ToHashSet();
        }


        /// <summary>
        /// Generates the next fresh variable pi. It is the first of "p1", "p2", "p3", ... that is fresh. Used for Fact 4 (decomposition with outermost quantifiers).
        /// </summary>
        /// <returns></returns>
        public Variable NextP()
        {
            string p;
            do
            {
                p = "p" + pIndex;
                pIndex++;
            } while (unav.Contains(p));
             
            return new Variable(p);
        }

        /// <summary>
        /// Generates the next pair of fresh variables v_plus and v_minus for a variable v. For a variable xi with (possibly empty) index i, it is the first of ("xp", "xm"), ("x1p", "x1m"), ("x2p", "x2m"), ... where both variables are fresh. Used for Definition 2 (t terms).
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        public PN NextPositiveAndNegative(Variable variable)
        {
            string plusEnding = "p";
            string minusEnding = "m";

            string stem = variable.Stem;
            
            if (!indices.ContainsKey(stem))
            {
                indices[stem] = 1;
                string plusName = stem + plusEnding;
                string minusName = stem + minusEnding;

                if (!unav.Contains(plusName) && !unav.Contains(minusName))
                {
                    return new PN(
                        new Variable(plusName),
                        new Variable(minusName));
                }
            }
            int index = indices[stem];

            string plus, minus;
            do
            {
                plus = stem + index + plusEnding;
                minus = stem + index + minusEnding;
                index++;
            } while (unav.Contains(plus) || unav.Contains(minus));
            indices[stem] = index;

            return new PN(
                new Variable(plus),
                new Variable(minus));
        }

        public class PN
        {
            public Variable P { get; }
            public Variable N { get; }
            public PN(Variable p, Variable n)
            {
                P = p;
                N = n;
            }
        }
    }
}
