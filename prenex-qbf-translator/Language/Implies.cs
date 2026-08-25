

namespace prenex_qbf_translator.Language
{
    public class Implies : BooleanOperator
    {
        private IFormula left;
        private IFormula right;

        public IFormula Left
        {
            get => left;
            set
            {
                ArgumentNullException.ThrowIfNull(value);
                left = value;
            }
        }

        public IFormula Right
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


        public Implies(IFormula left, IFormula right)
        {
            Left = left;
            Right = right;
        }

        public Implies(IFormula first, IFormula second, params IFormula[] other)
        {
            ArgumentNullException.ThrowIfNull(other);

            Left = first;
            Right = other.Length == 0
                ? second
                : new Implies(second, other[0], other[1..]);
        }


        public override IFormula DeepCopy()
        {
            return new Implies(left.DeepCopy(), right.DeepCopy());
        }


        public override string ToString()
        {
            bool NeedsParentheses(IFormula o) => o is Equivalent || o is Implies || o is IsImpliedBy;
            string Format(IFormula formula) => NeedsParentheses(formula) ? $"({formula})" : formula.ToString();

            return $"{Format(left)} -> {Format(right)}";
        }
    }
}
