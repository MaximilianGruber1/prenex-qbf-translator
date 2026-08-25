namespace prenex_qbf_translator.Language
{
    public class Forall : Quantifier
    {
        private IEnumerable<Variable> quantifiedVariables;
        private IFormula inner;

        public override IEnumerable<Variable> QuantifiedVariables
        {
            get => quantifiedVariables;
            set
            {
                ArgumentNullException.ThrowIfNull(value);
                var variables = value.ToArray();
                if (variables.Length == 0)
                {
                    throw new ArgumentException("Quantifier must bind at least one variable.");
                }
                foreach (var variable in variables)
                {
                    ArgumentNullException.ThrowIfNull(variable);
                }
                quantifiedVariables = variables;
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

        public Forall(IEnumerable<Variable> quantifiedVariables, IFormula inner)
        {
            QuantifiedVariables = quantifiedVariables;
            Inner = inner;
        }

        public Forall(Variable x, IFormula inner) : this([x], inner) { }
        public Forall(Variable x1, Variable x2, IFormula inner) : this([x1, x2], inner) { }
        public Forall(Variable x1, Variable x2, Variable x3, IFormula inner) : this([x1, x2, x3], inner) { }



        public override string ToString()
        {
            string symb = "#";
            string variables = string.Join(" ", QuantifiedVariables.Select(v => symb + v.Name));
            string subformula = (Inner is Equivalent || Inner is Implies || Inner is IsImpliedBy || Inner is Or || Inner is And) ?
                    $"({Inner})" :
                    $"{Inner}";
            return $"{variables} {subformula}";
        }
    }
}

