namespace prenex_qbf_translator.Language
{
    public class Exists : Quantifier
    {
        public override IEnumerable<Variable> QuantifiedVariables { get; }
        public override IFormula Inner { get; }

        public Exists(IEnumerable<Variable> variables, IFormula inner)
        {
            ArgumentNullException.ThrowIfNull(variables);
            ArgumentNullException.ThrowIfNull(inner);
            if (!variables.Any())
            {
                throw new ArgumentException("Exists must bind at least one variable.");
            }
            foreach (var variable in variables)
            {
                ArgumentNullException.ThrowIfNull(variable);
            }
            QuantifiedVariables = variables;
            Inner = inner;
        }

        public Exists(Variable x, IFormula inner) : this([x], inner) { }
        public Exists(Variable x1, Variable x2, IFormula inner) : this([x1, x2], inner) { }
        public Exists(Variable x1, Variable x2, Variable x3, IFormula inner) : this([x1, x2, x3], inner) { }

        
        public override string ToString()
        {
            return $"?[{string.Join(", ", QuantifiedVariables)}]: " +
                ((Inner is Equivalent || Inner is Implies || Inner is Or || Inner is And) ?
                    $"({Inner})" :
                    Inner.ToString());
        }
    }
}
