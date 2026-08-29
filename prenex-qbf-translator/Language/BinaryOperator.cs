namespace prenex_qbf_translator.Language
{
    public abstract class BinaryOperator : BooleanOperator
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

        public IEnumerable<IFormula> Subformulas
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

        public abstract IFormula DeepCopy();

    }
}
