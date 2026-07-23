namespace prenex_qbf_translator.Language
{
    public class FalseConstant : IFormula
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
            return new TrueConstant();
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
            return "false";
        }

        public IEnumerable<Variable> Variables()
        {
            return [];
        }
    }
}
