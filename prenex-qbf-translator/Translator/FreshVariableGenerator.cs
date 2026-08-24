using prenex_qbf_translator.Language;

namespace prenex_qbf_translator.Translator
{
    public class FreshVariableGenerator
    {
        /// <summary>
        /// Generates a fresh variable px that is not in the set of unavailable variables. Used in Fact 4.
        /// </summary>
        /// <param name="unavailableVariables"></param>
        /// <returns></returns>
        public Variable GetP(IEnumerable<Variable> unavailableVariables)
        {
            int index = 1;
            Variable p;
            do
            {
                p = new Variable($"p{index}");
                index++;
            } while (unavailableVariables.Contains(p));
            return p;
        }

        /// <summary>
        /// Generates fresh variables x+ and x- for a given variable x. If the variable x_p and x_m are available, they are returned. Otherwise, it generates x1_p and x1_m, x2_p and x2_m, etc., until it finds a pair that is not in the set of unavailable variables.
        /// </summary>
        /// <param name="baseVariable"></param>
        /// <param name="unavailableVariables"></param>
        /// <returns></returns>
        public Args GetPositiveAndNegative(Variable baseVariable, IEnumerable<Variable> unavailableVariables)
        {
            string plusEnding = "p";
            string minusEnding = "m";
            
            Variable plusVariable = new Variable(baseVariable.Name + plusEnding);
            Variable minusVariable = new Variable(baseVariable.Name + minusEnding);

            if (!unavailableVariables.Contains(plusVariable) && !unavailableVariables.Contains(minusVariable))
            {
                return new Args(plusVariable, minusVariable);
            }

            int index = 1;
            Variable vPlus, vMinus;
            do
            {
                vPlus = new Variable(baseVariable.Name + plusEnding + index);
                vMinus = new Variable(baseVariable.Name + minusEnding + index);
                index++;
            } while (unavailableVariables.Contains(vPlus) || unavailableVariables.Contains(vMinus));
            return new Args(vPlus, vMinus);
        }

        public class Args
        {
            public Variable P { get; }
            public Variable N { get; }
            public Args(Variable p, Variable n)
            {
                P = p;
                N = n;
            }
        }
    }
}
