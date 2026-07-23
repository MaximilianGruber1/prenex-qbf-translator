namespace prenex_qbf_translator.Language
{
    public class Or : IFormula
    {
        public IEnumerable<IFormula> Operands { get; private set; }

        public Or(IEnumerable<IFormula> operands)
        {
            if (operands == null)
            {
                throw new ArgumentNullException(nameof(operands));
            }
            if (operands.Count() < 2)
            {
                throw new ArgumentException("OR must have at least two operands.");
            }
            Operands = operands;
        }


        public IFormula Negated()
        {
            var negatedOperands = new List<IFormula>();
            foreach (var operand in Operands)
            {
                negatedOperands.Add(operand.Negated());
            }
            return new And(negatedOperands);
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
            return $"({string.Join(" | ", Operands)})";
        }
    }
}
