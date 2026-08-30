namespace prenex_qbf_translator.Language
{
    public class Forall : Quantifier
    {

        public Forall(Variable quantifiedVariable, IFormula inner)
        {
            Variable = quantifiedVariable;
            Inner = inner;
        }

        public Forall(IEnumerable<Variable> quantifiedVariables, IFormula inner)
        {
            ArgumentNullException.ThrowIfNull(quantifiedVariables);
            var qvars = quantifiedVariables.ToArray();
            if (qvars.Length == 0)
                throw new ArgumentException("quantifier requires at least one quantified variable");

            Variable = qvars[0];
            Inner = qvars.Length == 1 ?
                inner :
                new Forall(qvars[1..], inner);
        }
    }
}

