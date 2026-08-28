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

        public HashSet<Variable> Variables()
        {
            var vars = new HashSet<Variable>(Inner.Variables())
            {
                QuantifiedVariable
            };
            return vars;
        }

        public HashSet<Variable> FreeVariables()
        {
            var vars = new HashSet<Variable>(Inner.FreeVariables());
            vars.Remove(QuantifiedVariable);
            return vars;
        }

        public HashSet<Variable> BoundVariables()
        {
            var vars = new HashSet<Variable> (Inner.BoundVariables());
            vars.Add(QuantifiedVariable);
            return vars;
        }

        public bool IsBoolean()
        {
            return false;
        }
    }
}
