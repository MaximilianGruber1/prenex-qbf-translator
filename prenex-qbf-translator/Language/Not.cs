namespace prenex_qbf_translator.Language
{
    public class Not : BooleanOperator
    {
        public IFormula Inner { get; private set; }

        public override IEnumerable<IFormula> Subformulas => [Inner];

        public Not(IFormula inner)
        {
            ArgumentNullException.ThrowIfNull(inner);
            Inner = inner;
        }

        public Not(IEnumerable<IFormula> subformulas) // needed for Activator.CreateInstance call in BooleanOperator
        {
            ArgumentNullException.ThrowIfNull(subformulas);
            if (subformulas.Count() != 1)
            {
                throw new ArgumentException("'!' must have exactly one operand.");
            }
            var inner = subformulas.ElementAt(0);
            ArgumentNullException.ThrowIfNull(inner);
            Inner = inner;
        }
        

        public override string ToString()
        {
            return Inner is Equivalent || Inner is Implies || Inner is IsImpliedBy || Inner is Or || Inner is And ? 
                $"!({Inner})" : 
                $"!{Inner}";
        }
    }
}
