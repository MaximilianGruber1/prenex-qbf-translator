namespace prenex_qbf_translator.Language
{
    public class Not : Formula
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

        public Not(IFormula inner)
        {
            Inner = inner;
        }
    }
}
