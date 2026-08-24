using System;
using System.Collections.Generic;
using System.Text;

namespace prenex_qbf_translator.Language
{
    public interface IFormula
    {
        /// <summary>
        /// Creates a deep copy of the object
        /// </summary>
        /// <returns></returns>
        IFormula Clone();

        IEnumerable<Variable> Variables();

        IFormula ApplySubstitution(Substitution substitution);

        /// <summary>
        /// Checks if the formula does not contain any quantifiers
        /// </summary>
        /// <returns></returns>
        bool IsBoolean();
    }
}
