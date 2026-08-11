namespace prenex_qbf_translator.Language
{
    public class Implies : IFormula
    {
        public IFormula Left { get; private set; }
        public IFormula Right { get; private set; }
        public Implies(IFormula left, IFormula right)
        {
            if (left == null)
            {
                throw new ArgumentNullException(nameof(left));
            }
            if (right == null)
            {
                throw new ArgumentNullException(nameof(right));
            }
            Left = left;
            Right = right;
        }
        public IEnumerable<Variable> Variables()
        {
            return Left.Variables().Concat(Right.Variables()).Distinct();
        }
        public IEnumerable<Variable> FreeVariables()
        {
            return Left.FreeVariables().Concat(Right.FreeVariables()).Distinct();
        }
        public int NBlocks()
        {
            return Left.NBlocks() + Right.NBlocks();
        }
        public int NQuantifiedVariables()
        {
            return Left.NQuantifiedVariables() + Right.NQuantifiedVariables();
        }
        public int Length()
        {
            return 1 + Left.Length() + Right.Length();
        }
        public int QuantifierDepth()
        {
            return Math.Max(Left.QuantifierDepth(), Right.QuantifierDepth());
        }
        public IFormula ApplySubstitution(Substitution substitution)
        {
            Left = Left.ApplySubstitution(substitution);
            Right = Right.ApplySubstitution(substitution);
            return this;
        }

        public IEnumerable<IFormula> Subformulas()
        {
            return [Left, Right];
        }

        public IFormula DeepCopy()
        {
            return new Implies(Left.DeepCopy(), Right.DeepCopy());
        }

        public IFormula CreateCopy(IEnumerable<IFormula> subformulas)
        {
            if (subformulas == null)
                throw new ArgumentNullException(nameof(subformulas));
            if (subformulas.Count() != Subformulas().Count())
            {
                throw new ArgumentException("The number of subformulas does not match.");
            }
            return new Implies(subformulas.ElementAt(0), subformulas.ElementAt(1));
        }
    }
}