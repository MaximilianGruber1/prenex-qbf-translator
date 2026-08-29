using prenex_qbf_translator.Language;

namespace prenex_qbf_translator.Translator
{
    public class FreshVariableGenerator
    {
        private HashSet<Variable> unav;
        private int pIndex = 1;

        public FreshVariableGenerator(HashSet<Variable> unavailableVariables)
        {
            ArgumentNullException.ThrowIfNull(unavailableVariables, nameof(unavailableVariables));

            unav = unavailableVariables;
        }


        /// <summary>
        /// Generates the next fresh variable pn. It is the first of "p1", "p2", "p3", ... that is fresh. Used for Fact 4 (decomposition with outermost quantifiers).
        /// </summary>
        /// <returns></returns>
        public Variable NextP()
        {
            Variable p;
            do
            {
                p = new Variable($"p{pIndex}");
                pIndex++;
            } while (unav.Contains(p));
            unav.Add(p);
            return p;
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
            
            Variable plusVariable = new Variable(variable.Name + plusEnding);
            Variable minusVariable = new Variable(variable.Name + minusEnding);

            if (!unav.Contains(plusVariable) && !unav.Contains(minusVariable))
            {
                unav.Add(plusVariable);
                unav.Add(minusVariable);
                return new PN(plusVariable, minusVariable);
            }

            int index = 1;
            Variable plusVariableWithIndex, minusVariableWithIndex;
            do
            {
                plusVariableWithIndex = new Variable(variable.Name + plusEnding + index);
                minusVariableWithIndex = new Variable(variable.Name + minusEnding + index);
                index++;
            } while (unav.Contains(plusVariableWithIndex) || unav.Contains(minusVariableWithIndex));

            unav.Add(plusVariableWithIndex);
            unav.Add(minusVariableWithIndex);

            return new PN(plusVariableWithIndex, minusVariableWithIndex);
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
