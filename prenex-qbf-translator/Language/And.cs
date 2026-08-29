

namespace prenex_qbf_translator.Language
{
    public class And : BinaryOperator
    {
        public And(IFormula left, IFormula right)
        {
            Left = left;
            Right = right;
        }

        public And(params IFormula[] subformulas)
        {
            ArgumentNullException.ThrowIfNull(subformulas);
            if (subformulas.Length < 2)
                throw new ArgumentException("needs at least two subformulas");

            Left = subformulas[0];
            Right = subformulas.Length == 2
                ? subformulas[1]
                : new And(subformulas[1..]);
        }


        public override And DeepCopy()
        {
            return new And(Left.DeepCopy(), Right.DeepCopy());
        }


        public override string ToString()
        {
            bool NeedsParentheses(IFormula o) => o is Equivalent || o is Implies || o is IsImpliedBy || o is Or;
            string Format(IFormula formula) => NeedsParentheses(formula) ? $"({formula})" : formula.ToString();

            return $"{Format(Left)} & {Format(Right)}";
        }
    }
}
