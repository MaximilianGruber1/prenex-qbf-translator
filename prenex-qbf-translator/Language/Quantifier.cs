using System.Reflection.Metadata.Ecma335;

namespace prenex_qbf_translator.Language
{
    /// <summary>
    /// superclass of 'exists' and 'forall'
    /// </summary>
    public abstract class Quantifier : Formula
    {
        private Variable quantifiedVariable;
        private IFormula inner;


        public Variable Variable
        {
            get => quantifiedVariable;
            set
            {
                ArgumentNullException.ThrowIfNull(value);
                quantifiedVariable = value;
            }
        }

        public IFormula Inner
        {
            get => inner;
            set
            {
                ArgumentNullException.ThrowIfNull(value);
                inner = value;
            }
        }
    }
}
