namespace prenex_qbf_translator.Language
{
    /// <summary>
    /// superclass of !, &, |, ->, <-, <->
    /// </summary>
    public abstract class BooleanOperator : IFormula
    {
        public abstract IEnumerable<IFormula> Subformulas { get; set; }

        public abstract IFormula DeepCopy();
        

        public IEnumerable<Variable> Variables()
        {
            return Subformulas.SelectMany(s => s.Variables());
        }

        public IEnumerable<Variable> FreeVariables()
        {
            return Subformulas.SelectMany(s => s.FreeVariables());
        }

        public bool IsBoolean()
        {
            return Subformulas.All(f => f.IsBoolean());
        }
    }
}
