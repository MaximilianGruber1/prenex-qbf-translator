namespace prenex_qbf_translator.Language
{
    public class Not : IBooleanOperator
    {
        public IFormula Inner { get; private set; }

        public Not(IFormula inner)
        {
            Inner = inner;
        }
        public IEnumerable<Variable> Variables()
        {
            return Inner.Variables();
        }

        public IEnumerable<Variable> FreeVariables()
        {
            return Inner.FreeVariables();
        }

        public int NBlocks()
        {
            return Inner.NBlocks();
        }

        public int NQuantifiedVariables()
        {
            return Inner.NQuantifiedVariables();
        }

        public int Length()
        {
            return Inner.Length();
        }

        public int QuantifierDepth()
        {
            return Inner.QuantifierDepth();
        }

        public IFormula ApplySubstitution(Substitution substitution)
        {
            Inner = Inner.ApplySubstitution(substitution);
            return this;
        }



        public override string ToString()
        {
            return Inner is Equivalent || Inner is Implies || Inner is Or || Inner is And ? 
                $"~({Inner})" : 
                $"~{Inner}";
        }

        public IEnumerable<IFormula> Subformulas()
        {
            return [Inner];
        }

        public IFormula DeepCopy()
        {
            return new Not(Inner.DeepCopy());
        }

        public IFormula CreateCopy(IEnumerable<IFormula> subformulas)
        {
            if (subformulas == null)
                throw new ArgumentNullException(nameof(subformulas));
            if (subformulas.Count() != Subformulas().Count())
            {
                throw new ArgumentException("The number of subformulas does not match.");
            }
            return new Not(subformulas.ElementAt(0));
        }
    }
}
