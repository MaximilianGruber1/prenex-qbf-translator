using System.Reflection.Metadata.Ecma335;

namespace prenex_qbf_translator.Language
{
    public interface IQuantifier : IFormula
    {
        IEnumerable<Variable> BoundVariables { get; }

        IFormula Inner { get; }

        IQuantifier CreateCopy(IEnumerable<Variable> boundVariables, IFormula subformula);
    }
}
