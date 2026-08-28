namespace prenex_qbf_translator.Language
{
    /// <summary>
    /// superclass of !, &, |, ->, <-, <->
    /// </summary>
    public interface BooleanOperator : IFormula
    {
        IEnumerable<IFormula> Subformulas { get; set; }
    }
}
