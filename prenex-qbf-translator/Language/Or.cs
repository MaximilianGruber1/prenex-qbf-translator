namespace prenex_qbf_translator.Language
{
    public class Or : BinaryOperator
    {
        public Or(IFormula left, IFormula right)
        {
            Left = left;
            Right = right;
        }

        public Or(IFormula first, IFormula second, params IFormula[] other)
        {
            ArgumentNullException.ThrowIfNull(other);

            Left = first;
            Right = other.Length == 0
                ? second
                : new Or(second, other[0], other[1..]);
        }


        public override Or DeepCopy()
        {
            return new Or(Left.DeepCopy(), Right.DeepCopy());
        }


        public override string ToString()
        {
            bool NeedsParentheses(IFormula o) => o is Equivalent || o is Implies || o is IsImpliedBy;
            string Format(IFormula formula) => NeedsParentheses(formula) ? $"({formula})" : formula.ToString();

            return $"{Format(Left)} | {Format(Right)}";
        }
    }
}
