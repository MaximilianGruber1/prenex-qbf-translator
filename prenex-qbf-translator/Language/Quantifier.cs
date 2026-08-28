using System.Reflection.Metadata.Ecma335;

namespace prenex_qbf_translator.Language
{
    /// <summary>
    /// superclass of 'exists' and 'forall'
    /// </summary>
    public abstract class Quantifier : IFormula
    {
        public abstract Variable QuantifiedVariable { get; set; }
        public abstract IFormula Inner { get; set; }

        public abstract IFormula DeepCopy();

        public IEnumerable<Variable> Variables()
        {
            return Inner.Variables().Prepend(QuantifiedVariable).Distinct();
        }

        public IEnumerable<Variable> FreeVariables()
        {
            return Inner.FreeVariables().Where(v => !v.Equals(QuantifiedVariable));
        }

        public IEnumerable<Variable> BoundVariables()
        {
            return Inner.BoundVariables().Prepend(QuantifiedVariable).Distinct();
        }

        public bool IsBoolean()
        {
            return false;
        }
    }
}
