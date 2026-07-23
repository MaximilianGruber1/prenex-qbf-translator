namespace prenex_qbf_translator.Language
{
    public class Not : IFormula
    {
        public IFormula Inner { get; private set; }

        public Not(IFormula inner)
        {
            Inner = inner;
        }
        public IFormula Negated()
        {
            return Inner;
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
            return $"~{Inner}";
        }
    }
}
