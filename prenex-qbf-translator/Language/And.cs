

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
    }
}
