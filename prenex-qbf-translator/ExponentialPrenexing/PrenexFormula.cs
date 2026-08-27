using prenex_qbf_translator.Language;

namespace prenex_qbf_translator.ExponentialPrenexing
{
    public class PrenexFormula
    {
        /// <summary>
        /// The formula it represents.
        /// </summary>
        public IFormula Formula { get; set; }

        /// <summary>
        /// Returns the quantifier-free formula inside the quantifiers.
        /// </summary>
        /// <returns></returns>
        public IFormula GetMatrix()
        {
            var cur = Formula;
            while (cur is Quantifier q)
            {
                cur = q.Inner;
            }
            return cur;
        }

        /// <summary>
        /// Sets the quantifier-free formula inside the quantifiers to a given prenexed formula. The new matrix becomes the matrix of p.
        /// </summary>
        /// <param name="p"></param>
        public void ReplaceMatrix(PrenexFormula p)
        {
            if (Formula is Quantifier q)
            {
                // find innermost quantifier
                var cur = q;
                while (cur.Inner is Quantifier qq)
                {
                    cur = qq;
                }
                cur.Inner = p.Formula;
            }
            else
            {
                Formula = p.Formula;
            }
        }


        public IEnumerable<Variable> Variables() => Formula.Variables();
        public IEnumerable<Variable> BoundVariables() => Formula.BoundVariables();
        public IEnumerable<Variable> FreeVariables() => Formula.FreeVariables();
        public PrenexFormula DeepCopy() => new(Formula.DeepCopy());
        public bool IsBoolean() => Formula.IsBoolean();



        /// <summary>
        /// Does not create a deep copy for performance reasons.
        /// </summary>
        /// <param name="formula"></param>
        /// <exception cref="ArgumentException"></exception>
        public PrenexFormula(IFormula formula)
        {
            Formula = formula;
            if (!GetMatrix().IsBoolean())
                throw new ArgumentException($"parameter '{nameof(formula)}' is not prenexed");
        }


        /// <summary>
        /// Replaces each quantifier with its dual.
        /// </summary>
        /// <returns></returns>
        public void SetToDual()
        {
            Formula = CreateDualRecursive(Formula);
        }

        private IFormula CreateDualRecursive(IFormula formula)
        {
            if (formula is Quantifier q)
            {
                var dual = GetDual(q);
                dual.Inner = CreateDualRecursive(q.Inner);
                return dual;
            }
            else
            {
                return formula;
            }
        }

        private Quantifier GetDual(Quantifier q)
        {
            if (q is Forall)
                return new Exists(q.QuantifiedVariables, q.Inner);
            else
                return new Forall(q.QuantifiedVariables, q.Inner);
        }

        public override string ToString()
        {
            return Formula.ToString();
        }
    }
}
