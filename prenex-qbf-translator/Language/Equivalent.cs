namespace prenex_qbf_translator.Language
{
    public class Equivalent : IFormula
    {
        public IEnumerable<IFormula> Operands { get; private set; }

        public Equivalent(IEnumerable<IFormula> operands)
        {
            if (operands == null)
            {
                throw new ArgumentNullException(nameof(operands));
            }
            if (operands.Count() < 2)
            {
                throw new ArgumentException("EQUIVALENT must have at least two operands.");
            }
            Operands = operands;
        }

        public IFormula Negated()
        {
            // Negation of an equivalence is not trivially an equivalence of negated operands.
            // Use a Not wrapper; more advanced normalization can expand this into AND/OR form if needed.
            return new Not(this);
        }

        public IEnumerable<Variable> Variables()
        {
            return Operands.SelectMany(o => o.Variables()).Distinct();
        }

        public IEnumerable<Variable> FreeVariables()
        {
            return Operands.SelectMany(o => o.FreeVariables()).Distinct();
        }

        public int NBlocks()
        {
            return Operands.Sum(o => o.NBlocks());
        }

        public int NQuantifiedVariables()
        {
            return Operands.Sum(o => o.NQuantifiedVariables());
        }

        public int Length()
        {
            return 2 * Operands.Count() - 1 + Operands.Sum(o => o.Length());
        }

        public int QuantifierDepth()
        {
            return Operands.Max(o => o.QuantifierDepth());
        }

        public IFormula ApplySubstitution(Substitution substitution)
        {
            Operands = Operands.Select(o => o.ApplySubstitution(substitution));
            return this;
        }



        public override string ToString()
        {
            return $"({string.Join(" <=> ", Operands)})";
        }
    }
}
