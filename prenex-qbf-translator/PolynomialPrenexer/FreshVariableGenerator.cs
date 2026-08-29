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
        /// Generates a fresh variable "pn" that is not in the set of unavailable variables where "n" is the smallest integer >= 1. Used for Fact 4 (decomposition with outermost quantifiers).
        /// </summary>
        /// <param name="unavailableVariables"></param>
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
        /// Generates fresh variables x^+ and x^- that are not in the set of unavailable variables for a given variable x. If "xp" and "xm" are available, they are returned. Otherwise, "xpn" and "xmn" are returned where n is the smallest integer >= 1. Used for Definition 2 (t terms).
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="unavailableVariables"></param>
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
