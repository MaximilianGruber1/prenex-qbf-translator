namespace prenex_qbf_translator.Language
{
    public abstract class BinaryOperator : BooleanOperator
    {
        public abstract IFormula Left { get; set; }
        public abstract IFormula Right { get; set; }
    }
}
