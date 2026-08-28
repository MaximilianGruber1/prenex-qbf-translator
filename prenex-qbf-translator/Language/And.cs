

namespace prenex_qbf_translator.Language
{
    public class And : BinaryOperator
    {
        public And(IFormula left, IFormula right)
        {
            Left = left;
            Right = right;
        }

        public And(IFormula first, IFormula second, params IFormula[] other)
        {
            ArgumentNullException.ThrowIfNull(other);

            Left = first;
            Right = other.Length == 0
                ? second
                : new And(second, other[0], other[1..]);
        }


        public override And DeepCopy()
        {
            return new And(left.DeepCopy(), right.DeepCopy());
        }


        public override string ToString()
        {
            bool NeedsParentheses(IFormula o) => o is Equivalent || o is Implies || o is IsImpliedBy || o is Or;
            string Format(IFormula formula) => NeedsParentheses(formula) ? $"({formula})" : formula.ToString();

            return $"{Format(left)} & {Format(right)}";
        }
    }
}
