

namespace prenex_qbf_translator.Language
{
    public class IsImpliedBy : BinaryOperator
    {
        public IsImpliedBy(IFormula left, IFormula right)
        {
            Left = left;
            Right = right;
        }

        public IsImpliedBy(IFormula first, IFormula second, params IFormula[] other)
        {
            ArgumentNullException.ThrowIfNull(other);

            Left = first;
            Right = other.Length == 0
                ? second
                : new IsImpliedBy(second, other[0], other[1..]);
        }
    }
}
