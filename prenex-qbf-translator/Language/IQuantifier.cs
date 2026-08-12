namespace prenex_qbf_translator.Language
{
    public interface IQuantifier : IFormula
    {
        IEnumerable<Variable> BoundVariables { get; }

        IFormula Inner { get; }
    }
}
