

namespace prenex_qbf_translator.Language
{
    public class Equivalent : BinaryOperator
    {
        public Equivalent(IFormula left, IFormula right)
        {
            Left = left;
            Right = right;
        }

        public Equivalent(IFormula first, IFormula second, params IFormula[] other)
        {
            ArgumentNullException.ThrowIfNull(other);

            Left = first;
            Right = other.Length == 0
                ? second
                : new Equivalent(second, other[0], other[1..]);
        }


        public override Equivalent DeepCopy()
        {
            return new Equivalent(Left.DeepCopy(), Right.DeepCopy());
        }


        public override string ToString()
        {
            return $"{Left} <-> {Right}";
        }
    }
}
