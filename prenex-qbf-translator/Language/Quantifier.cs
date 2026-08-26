using System.Reflection.Metadata.Ecma335;

namespace prenex_qbf_translator.Language
{
    /// <summary>
    /// superclass of 'exists' and 'forall'
    /// </summary>
    public abstract class Quantifier : IFormula
    {
        public abstract IEnumerable<Variable> QuantifiedVariables { get; set; }
        public abstract IFormula Inner { get; set; }


        public IFormula DeepCopy()
        {
            return (IFormula)Activator.CreateInstance(GetType(), QuantifiedVariables.ToArray(), Inner.DeepCopy())!;
        }

        public IEnumerable<Variable> Variables()
        {
            return QuantifiedVariables.Concat(Inner.Variables()).Distinct();
        }

        public IEnumerable<Variable> FreeVariables()
        {
            return Inner.FreeVariables().Except(QuantifiedVariables);
        }

        public IEnumerable<Variable> BoundVariables()
        {
            return QuantifiedVariables.Concat(Inner.BoundVariables()).Distinct();
        }

        public bool IsBoolean()
        {
            return false;
        }
    }
}
