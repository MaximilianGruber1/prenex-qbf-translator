namespace prenex_qbf_translator.Language
{
    public class Exists : IQuantifier
    {
        public IEnumerable<Variable> BoundVariables { get; }
        public IFormula Inner { get; private set; }

        public Exists(IEnumerable<Variable> variables, IFormula inner)
        {
            if (variables == null)
            {
                throw new ArgumentNullException(nameof(variables));
            }
            var list = variables.ToList();
            if (!list.Any())
            {
                throw new ArgumentException("Exists must bind at least one variable.");
            }
            BoundVariables = list;
            Inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        public Exists(Variable x, IFormula inner) : this([x], inner) { }
        public Exists(Variable x1, Variable x2, IFormula inner) : this([x1, x2], inner) { }
        public Exists(Variable x1, Variable x2, Variable x3, IFormula inner) : this([x1, x2, x3], inner) { }

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
            return BoundVariables.Count() + Inner.NQuantifiedVariables();
        }

        public int Length()
        {
            return 1 + BoundVariables.Count() + Inner.Length();
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

            var filtered = new Dictionary<Variable, IFormula>(substitution.Dictionary);
            foreach (var v in BoundVariables)
            {
                if (filtered.ContainsKey(v)) filtered.Remove(v);
            }

            Inner = Inner.ApplySubstitution(new Substitution(filtered));
            return this;
        }

        public IEnumerable<IFormula> Subformulas()
        {
            return [Inner];
        }

        public IFormula DeepCopy()
        {
            return new Exists(BoundVariables.Select(v => (Variable)(v.DeepCopy())), Inner.DeepCopy());
        }


        public override string ToString()
        {
            return $"? [{string.Join(", ", BoundVariables)}] : {Inner}";
        }

        public IQuantifier CreateCopy(IEnumerable<Variable> boundVariables, IFormula subformula)
        {
            return new Exists(boundVariables, subformula);
        }
    }
}
