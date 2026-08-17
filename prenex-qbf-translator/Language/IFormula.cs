using System;
using System.Collections.Generic;
using System.Text;

namespace prenex_qbf_translator.Language
{
    public interface IFormula
    {
        IFormula Clone();

        /**
         * Returns the variables in the formula.
         * A variable is a symbol that can take on different values.
         * For example, in the formula ∀x ∃y P(x,y,z), the variables are x, y, and z.
         */
        IEnumerable<Variable> Variables();

        /**
         * Applies a substitution to the formula.
         * A substitution is a mapping from variables to formulas.
         * For example, if the formula is P(x) and the substitution is {x -> Q(y)}, then the result of applying the substitution is P(Q(y)).
         */
        IFormula ApplySubstitution(Substitution substitution);

        /// <summary>
        /// Checks whether the formula contains any quantifiers
        /// </summary>
        /// <returns></returns>
        bool IsBoolean();
    }
}
