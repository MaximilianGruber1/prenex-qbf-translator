namespace prenex_qbf_translator.Language
{
    public interface IBooleanOperator : IFormula
    {
        IEnumerable<IFormula> Subformulas();

        IFormula CreateCopy(IEnumerable<IFormula> subformulas);
    }
}
