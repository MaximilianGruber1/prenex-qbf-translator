using prenex_qbf_translator.Language;

namespace prenex_qbf_translator.PolynomialPrenexing
{
    public class FreshVariableGenerator
    {
        private readonly HashSet<string> unav;
        private int pIndex = 1;

        private readonly Dictionary<string, int> indices = new();

        public FreshVariableGenerator(HashSet<Variable> unavailableVariables)
        {
            ArgumentNullException.ThrowIfNull(unavailableVariables, nameof(unavailableVariables));

            unav = unavailableVariables.Select(v => v.Name).ToHashSet();
        }


        /// <summary>
        /// Generates the next fresh variable pn. It is the first of "p1", "p2", "p3", ... that is fresh. Used for Fact 4 (decomposition with outermost quantifiers).
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
        /// Generates the next pair of fresh variables x_plus and x_minus for a variable x. It is the first of ("xp", "xm"), ("xp1", "xm1"), ("xp2", "xm2"), ... where both variables are fresh. Used for Definition 2 (t terms).
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        public PN NextPositiveAndNegative(Variable variable)
        {
            string plusEnding = "p";
            string minusEnding = "m";

            string namePlus = variable.Name + plusEnding;
            string nameMinus = variable.Name + minusEnding;

            // if plus and minus for this variable are fresh
            if (!unav.Contains(namePlus) && !unav.Contains(nameMinus))
            {
                unav.Add(namePlus);
                unav.Add(nameMinus);
                return new PN(
                    new Variable(namePlus), 
                    new Variable(nameMinus));
            }

            // if not, remove the index and try with indices 1, 2, ...
            string nameWithoutIndex = WithoutIndex(variable.Name);
            
            if (!indices.ContainsKey(nameWithoutIndex))
            {
                indices[nameWithoutIndex] = 1;
            }
            int index = indices[nameWithoutIndex];

            string plus, minus;
            do
            {
                plus = nameWithoutIndex + index + plusEnding;
                minus = nameWithoutIndex + index + minusEnding;
                index++;
            } while (unav.Contains(plus) || unav.Contains(minus));
            indices[nameWithoutIndex] = index;

            return new PN(
                new Variable(plus),
                new Variable(minus));
        }

        private string WithoutIndex(string name)
        {
            int i = name.Length;

            while (i > 1 && char.IsDigit(name[i - 1]))
            {
                i--;
            }

            return name[..i];
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
