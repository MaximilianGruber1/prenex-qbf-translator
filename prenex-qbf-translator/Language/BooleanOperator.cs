namespace prenex_qbf_translator.Language
{
    public abstract class BooleanOperator : IFormula
    {
        public abstract IEnumerable<IFormula> Subformulas { get; }

        

        public IFormula ApplySubstitution(Substitution substitution)
        {
            var subs = Subformulas.Select(s => s.ApplySubstitution(substitution));
            return (IFormula)Activator.CreateInstance(GetType(), subs)!;
        }

        public BooleanOperator CreateCopy(IEnumerable<IFormula> subformulas)
        {
            return (BooleanOperator)Activator.CreateInstance(GetType(), subformulas)!;
        }

        public IFormula Clone()
        {
            var subs = Subformulas.Select(s => s.Clone());
            return (IFormula)Activator.CreateInstance(GetType(), subs)!;
        }

        public IEnumerable<Variable> Variables()
        {
            return Subformulas.SelectMany(s => s.Variables());
        }

        public bool IsBoolean()
        {
            return Subformulas.All(f => f.IsBoolean());
        }
    }
}
