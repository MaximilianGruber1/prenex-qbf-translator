namespace prenex_qbf_translator.Language
{
    public class FalseConstant : IFormula
    {
        public IFormula ApplySubstitution(Substitution substitution)
        {
            return this;
        }

        public IFormula CreateCopy(IEnumerable<IFormula> subformulas)
        {
            throw new NotImplementedException();
        }

        public IFormula DeepCopy()
        {
            return new FalseConstant();
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

        public int NQuantifiedVariables()
        {
            return 0;
        }

        public int QuantifierDepth()
        {
            return 0;
        }

        public IEnumerable<IFormula> Subformulas()
        {
            return [];
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
