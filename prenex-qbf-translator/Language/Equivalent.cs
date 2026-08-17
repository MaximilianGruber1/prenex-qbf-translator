namespace prenex_qbf_translator.Language
{
    public class Equivalent : BooleanOperator
    {
        public IFormula Left { get; private set; }
        public IFormula Right { get; private set; }

        public override IEnumerable<IFormula> Subformulas => [Left, Right];


        public Equivalent(IFormula left, IFormula right)
        {
            ArgumentNullException.ThrowIfNull(left);
            ArgumentNullException.ThrowIfNull(right);
            Left = left;
            Right = right;
        }

        public Equivalent(IEnumerable<IFormula> subformulas)
        {
            ArgumentNullException.ThrowIfNull(subformulas);
            if (subformulas.Count() != 2)
            {
                throw new ArgumentException("'Equivalent' must be instantiated with exactly 2 subformulas.");
            }
            var left = subformulas.ElementAt(0);
            var right = subformulas.ElementAt(1);
            ArgumentNullException.ThrowIfNull(left);
            ArgumentNullException.ThrowIfNull(right);
            Left = left;
            Right = right;
        }

        public override string ToString()
        {
            List<IFormula> operands = [Left, Right];
            return $"{string.Join(" <=> ",
                operands.Select(o => o is Equivalent ? $"({o})" : o.ToString()))}";
        }
    }
}