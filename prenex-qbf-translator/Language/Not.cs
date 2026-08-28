namespace prenex_qbf_translator.Language
{
    public class Not : BooleanOperator
    {
        private IFormula inner;

        public IFormula Inner
        {
            get => inner;
            set
            {
                ArgumentNullException.ThrowIfNull(value);
                inner = value;
            }
        }

        public IEnumerable<IFormula> Subformulas
        {
            get => [inner];
            set
            {
                ArgumentNullException.ThrowIfNull(value);
                var subformulas = value.ToArray();
                if (subformulas.Length != 1) throw new ArgumentException("needs 1 subformula");
                Inner = subformulas[0];
            }
        }

        public Not(IFormula inner)
        {
            Inner = inner;
        }




        public IFormula DeepCopy()
        {
            return new Not(inner.DeepCopy());
        }
        

        public override string ToString()
        {
            return Inner is Equivalent || Inner is Implies || Inner is IsImpliedBy || Inner is Or || Inner is And ? 
                $"!({Inner})" : 
                $"!{Inner}";
        }

        public HashSet<Variable> Variables()
        {
            return Inner.Variables();
        }

        public HashSet<Variable> FreeVariables()
        {
            return Inner.FreeVariables();
        }

        public HashSet<Variable> BoundVariables()
        {
            return Inner.BoundVariables();
        }

        public bool IsBoolean()
        {
            return Inner.IsBoolean();
        }
    }
}
