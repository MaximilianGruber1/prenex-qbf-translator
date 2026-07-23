namespace prenex_qbf_translator.Language
{
    public class TrueConstant : IFormula
    {
        public IFormula ApplySubstitution(Substitution substitution)
        {
            return this;
        }

        public IEnumerable<Variable> FreeVariables()
        {
            return [];
        }

        public int Length()
        {
            return 1;
        }

        public int NBlocks()
        {
            return 0;
        }

        public IFormula Negated()
        {
            return new FalseConstant();
        }

        public int NQuantifiedVariables()
        {
            return 0;
        }

        public int QuantifierDepth()
        {
            return 0;
        }

        public override string ToString()
        {
            return "true";
        }

        public IEnumerable<Variable> Variables()
        {
            return [];
        }
    }
}
