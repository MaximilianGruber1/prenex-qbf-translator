namespace prenex_qbf_translator.Language
{
    public class Not : IFormula
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
    }
}
