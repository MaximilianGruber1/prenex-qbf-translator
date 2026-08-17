using System.Reflection.Metadata.Ecma335;

namespace prenex_qbf_translator.Language
{
    public abstract class Quantifier : IFormula
    {
        public abstract IEnumerable<Variable> QuantifiedVariables { get; }
        public abstract IFormula Inner { get; }

        public IFormula ApplySubstitution(Substitution substitution)
        {
            ArgumentNullException.ThrowIfNull(substitution);
            var filtered = new Dictionary<Variable, IFormula>(substitution.Dictionary);
            foreach (var v in QuantifiedVariables)
            {
                filtered.Remove(v);
            }

            var inner = Inner.ApplySubstitution(new Substitution(filtered));
            return (Quantifier)Activator.CreateInstance(GetType(), QuantifiedVariables.ToList(), inner)!;
        }

        public Quantifier CreateCopy(IEnumerable<Variable> quantifiedVariables, IFormula inner)
        {
            return (Quantifier)Activator.CreateInstance(GetType(), QuantifiedVariables.ToList(), inner)!;
        }

        public IFormula Clone()
        {
            return (IFormula)Activator.CreateInstance(GetType(), QuantifiedVariables.ToList(), Inner.Clone())!;
        }

        public IEnumerable<Variable> Variables()
        {
            return Inner.Variables().Concat(QuantifiedVariables).Distinct();
        }

        public bool IsBoolean()
        {
            return false;
        }
    }
}
