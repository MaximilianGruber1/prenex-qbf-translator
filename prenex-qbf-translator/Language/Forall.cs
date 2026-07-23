namespace prenex_qbf_translator.Language
{
    public class Forall : IFormula
    {
        public IReadOnlyList<Variable> BoundVariables { get; }
        public IFormula Inner { get; private set; }

        public Forall(IEnumerable<Variable> variables, IFormula inner)
        {
            if (variables == null)
            {
                throw new ArgumentNullException(nameof(variables));
            }
            var list = variables.ToList();
            if (!list.Any())
            {
                throw new ArgumentException("Forall must bind at least one variable.");
            }
            BoundVariables = list;
            Inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        public IFormula Negated()
        {
            return new Exists(BoundVariables, Inner.Negated());
        }

        public IEnumerable<Variable> Variables()
        {
            return Inner.Variables().Concat(BoundVariables).Distinct();
        }

        public IEnumerable<Variable> FreeVariables()
        {
            return Inner.FreeVariables().Where(v => !BoundVariables.Contains(v)).Distinct();
        }

        public int NBlocks()
        {
            return 1 + Inner.NBlocks();
        }

        public int NQuantifiedVariables()
        {
            return BoundVariables.Count + Inner.NQuantifiedVariables();
        }

        public int Length()
        {
            return 1 + BoundVariables.Count + Inner.Length();
        }

        public int QuantifierDepth()
        {
            return 1 + Inner.QuantifierDepth();
        }

        public IFormula ApplySubstitution(Substitution substitution)
        {
            if (substitution == null)
            {
                throw new ArgumentNullException(nameof(substitution));
            }

            var filtered = new Dictionary<Variable, IFormula>(substitution.Entries);
            foreach (var v in BoundVariables)
            {
                if (filtered.ContainsKey(v)) filtered.Remove(v);
            }

            Inner = Inner.ApplySubstitution(new Substitution(filtered));
            return this;
        }


        public override string ToString()
        {
            return $"! {string.Join(", ", BoundVariables)}: {Inner}";
        }
    }
}
