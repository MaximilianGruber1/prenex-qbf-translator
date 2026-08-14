namespace prenex_qbf_translator.Language
{
    public interface IBooleanOperator : IFormula
    {
        IEnumerable<IFormula> Subformulas();

        IBooleanOperator CreateCopy(IEnumerable<IFormula> subformulas);
    }
}
