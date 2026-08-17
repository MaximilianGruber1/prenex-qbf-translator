namespace prenex_qbf_translator.Language
{
    public abstract class TruthConstant : IFormula
    {
        public IFormula ApplySubstitution(Substitution substitution)
        {
            return this;
        }

        public IFormula Clone()
        {
            return this; // immutable, so no problem with returning itself
        }

        public bool IsBoolean()
        {
            return true;
        }

        public IEnumerable<Variable> Variables()
        {
            return [];
        }
    }
}
