namespace prenex_qbf_translator.Language
{
    public class Forall : Quantifier
    {
        private Variable quantifiedVariable;
        private IFormula inner;

        public override Variable QuantifiedVariable
        {
            get => quantifiedVariable;
            set
            {
                ArgumentNullException.ThrowIfNull(value);
                quantifiedVariable = value;
            }
        }

        public override IFormula Inner
        {
            get => inner;
            set
            {
                ArgumentNullException.ThrowIfNull(value);
                inner = value;
            }
        }

        public Forall(Variable quantifiedVariable, IFormula inner)
        {
            QuantifiedVariable = quantifiedVariable;
            Inner = inner;
        }

        public Forall(IEnumerable<Variable> quantifiedVariables, IFormula inner)
        {
            ArgumentNullException.ThrowIfNull(quantifiedVariables);
            var qvars = quantifiedVariables.ToArray();
            if (qvars.Length == 0)
                throw new ArgumentException("quantifier requires at least one quantified variable");

            QuantifiedVariable = qvars[0];
            Inner = qvars.Length == 1 ?
                inner :
                new Forall(qvars[1..], inner);
        }


        public override Forall DeepCopy()
        {
            return new Forall(QuantifiedVariable, Inner.DeepCopy());
        }


        public override string ToString()
        {
            string subformula = (Inner is Equivalent || Inner is Implies || Inner is IsImpliedBy || Inner is Or || Inner is And) ?
                    $"({Inner})" :
                    $"{Inner}";
            return $"#{QuantifiedVariable} {subformula}";
        }
    }
}

