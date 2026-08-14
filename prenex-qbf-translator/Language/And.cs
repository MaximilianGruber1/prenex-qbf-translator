using System.Reflection.Metadata.Ecma335;

namespace prenex_qbf_translator.Language
{
    public class And : IBooleanOperator
    {
        public IEnumerable<IFormula> Operands { get; private set; }
        
        public And(IEnumerable<IFormula> operands)
        {
            if (operands == null)
            {
                throw new ArgumentNullException(nameof(operands));
            }
            if (operands.Count() < 2)
            {
                throw new ArgumentException("AND must have at least two operands.");
            }
            Operands = operands;
        }

        public And(IFormula a, IFormula b) : this([a, b]) { }

        public And(IFormula a, IFormula b, IFormula c) : this([a, b, c]) { }

        public And(IFormula a, IFormula b, IFormula c, IFormula d) : this([a, b, c, d]) { }


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
        public IEnumerable<IFormula> Subformulas()
        {
            return Operands;
        }

        public IFormula DeepCopy()
        {
            return new And(Operands.Select(o => o.DeepCopy()));
        }




        public override string ToString()
        {
            return $"{string.Join(" & ", 
                Operands.Select(o => o is Equivalent || o is Implies || o is Or ? $"({o.ToString()})" : o.ToString()))}";
        }

        public IBooleanOperator CreateCopy(IEnumerable<IFormula> subformulas)
        {
            if (subformulas == null)
                throw new ArgumentNullException(nameof(subformulas));
            
            if (subformulas.Count() != Subformulas().Count())
            {
                throw new ArgumentException("The number of subformulas does not match.");
            }
            return new And(subformulas);
        }
    }
}
