using System.Reflection.Metadata.Ecma335;

namespace prenex_qbf_translator.Language
{
    public class And : BooleanOperator
    {
        private readonly List<IFormula> operands;

        public override IEnumerable<IFormula> Subformulas => operands;
        
        public And(IEnumerable<IFormula> operands)
        {
            ArgumentNullException.ThrowIfNull(operands);
            if (operands.Count() < 2)
            {
                throw new ArgumentException("'&' must have at least two operands.");
            }
            foreach (var operand in operands)
            {
                ArgumentNullException.ThrowIfNull(operand);
            }
            this.operands = operands.ToList();
        }

        public And(IFormula a, IFormula b) : this([a, b]) { }

        public And(IFormula a, IFormula b, IFormula c) : this([a, b, c]) { }

        public And(IFormula a, IFormula b, IFormula c, IFormula d) : this([a, b, c, d]) { }



        public override string ToString()
        {
            return $"{string.Join(" & ", 
                operands.Select(o => o is Equivalent || o is Implies || o is IsImpliedBy || o is Or ? $"({o})" : o.ToString()))}";
        }
    }
}
