namespace prenex_qbf_translator.Language
{
    public class Or : BinaryOperator
    {
        public Or(IFormula left, IFormula right)
        {
            Left = left;
            Right = right;
        }

        public Or(params IFormula[] subformulas)
        {
            ArgumentNullException.ThrowIfNull(subformulas);
            if (subformulas.Length < 2)
                throw new ArgumentException("needs at least two subformulas");

            Left = subformulas[0];
            Right = subformulas.Length == 2
                ? subformulas[1]
                : new Or(subformulas[1..]);
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
