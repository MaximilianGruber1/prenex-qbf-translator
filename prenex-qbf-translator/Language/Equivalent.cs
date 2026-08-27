

namespace prenex_qbf_translator.Language
{
    public class Equivalent : BinaryOperator
    {
        private IFormula left;
        private IFormula right;

        public override IFormula Left
        {
            get => left;
            set
            {
                ArgumentNullException.ThrowIfNull(value);
                left = value;
            }
        }

        public override IFormula Right
        {
            get => right;
            set
            {
                ArgumentNullException.ThrowIfNull(value);
                right = value;
            }
        }

        public override IEnumerable<IFormula> Subformulas
        {
            get => [left, right];
            set
            {
                ArgumentNullException.ThrowIfNull(value);
                var subformulas = value.ToArray();
                if (subformulas.Length != 2) throw new ArgumentException("needs 2 subformulas");
                Left = subformulas[0];
                Right = subformulas[1];
            }
        }


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
            return new Equivalent(left.DeepCopy(), right.DeepCopy());
        }


        public override string ToString()
        {
            return $"{left} <-> {right}";
        }
    }
}
