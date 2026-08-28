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
        IFormula DeepCopy();

        HashSet<Variable> Variables();

        HashSet<Variable> FreeVariables();

        HashSet<Variable> BoundVariables();

        /// <summary>
        /// Checks if the formula does not contain any quantifiers
        /// </summary>
        /// <returns></returns>
        bool IsBoolean();
    }
}
