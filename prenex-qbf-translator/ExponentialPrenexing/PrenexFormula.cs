using prenex_qbf_translator.Language;

namespace prenex_qbf_translator.ExponentialPrenexing
{
    public class PrenexFormula
    {
        private HashSet<Variable> variables;
        private HashSet<Variable> quantifiedVariables;

        /// <summary>
        /// The formula it represents.
        /// </summary>
        public IFormula Formula { get; private set; }

        public PrenexFormula(Variable variable)
        {
            Formula = variable;
            variables = [variable];
            quantifiedVariables = [];
        }

        private PrenexFormula(PrenexFormula p)
        {
            Formula = p.Formula.DeepCopy();
            variables = [.. p.variables];
            quantifiedVariables = [.. p.quantifiedVariables];
        }

        public void Not()
        {
            SetToDual();
            var oldMatrix = GetMatrix();
            IFormula newMatrix =
                oldMatrix is Not n ?
                n.Inner :
                new Not(GetMatrix());
            ReplaceMatrix(newMatrix);
        }

        public void And(PrenexFormula right)
        {
            RenameVariables(right);

            IFormula newMatrix = new And(this.GetMatrix(), right.GetMatrix());
            right.ReplaceMatrix(newMatrix);
            this.ReplaceMatrix(right.Formula);

            variables.UnionWith(right.variables);
            quantifiedVariables.UnionWith(right.quantifiedVariables);
        }

        public void Or(PrenexFormula right)
        {
            RenameVariables(right);

            IFormula newMatrix = new Or(this.GetMatrix(), right.GetMatrix());
            right.ReplaceMatrix(newMatrix);
            this.ReplaceMatrix(right.Formula);

            variables.UnionWith(right.variables);
            quantifiedVariables.UnionWith(right.quantifiedVariables);
        }

        public void Implies(PrenexFormula right)
        {
            RenameVariables(right);

            this.SetToDual();
            IFormula newMatrix = new Implies(this.GetMatrix(), right.GetMatrix());
            right.ReplaceMatrix(newMatrix);
            this.ReplaceMatrix(right.Formula);

            variables.UnionWith(right.variables);
            quantifiedVariables.UnionWith(right.quantifiedVariables);
        }

        public void IsImpliedBy(PrenexFormula right)
        {
            RenameVariables(right);

            right.SetToDual();
            IFormula newMatrix = new IsImpliedBy(this.GetMatrix(), right.GetMatrix());
            right.ReplaceMatrix(newMatrix);
            this.ReplaceMatrix(right.Formula);

            variables.UnionWith(right.variables);
            quantifiedVariables.UnionWith(right.quantifiedVariables);
        }

        public void Equivalent(PrenexFormula right)
        {
            if (this.quantifiedVariables.Count == 0 && right.quantifiedVariables.Count == 0) // no decomposition of a<->b into a&b|!a&!b needed
            {
                Formula = new Equivalent(this.Formula, right.Formula);

                variables.UnionWith(right.variables);
                quantifiedVariables.UnionWith(right.quantifiedVariables);
            }
            else
            {
                var p1 = this;
                var p2 = right;
                var p3 = new PrenexFormula(this);
                var p4 = new PrenexFormula(right);

                p1.And(p2);
                p3.Not();
                p4.Not();
                p3.And(p4);
                p1.Or(p3);
            }
        }

        public void Forall(Variable qvar)
        {
            Formula = new Forall(qvar, Formula);

            variables.Add(qvar);
            quantifiedVariables.Add(qvar);
        }

        public void Exists(Variable qvar)
        {
            Formula = new Exists(qvar, Formula);

            variables.Add(qvar);
            quantifiedVariables.Add(qvar);
        }



        /// <summary>
        /// Returns the quantifier-free formula inside the quantifiers.
        /// </summary>
        /// <returns></returns>
        private IFormula GetMatrix()
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
        private void ReplaceMatrix(IFormula p)
        {
            if (Formula is Quantifier q)
            {
                // find innermost quantifier
                var cur = q;
                while (cur.Inner is Quantifier qq)
                {
                    cur = qq;
                }
                cur.Inner = p;
            }
            else
            {
                Formula = p;
            }
        }

        /// <summary>
        /// Replaces each quantifier with its dual.
        /// </summary>
        /// <returns></returns>
        private void SetToDual()
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
                return new Exists(q.Variable, q.Inner);
            else
                return new Forall(q.Variable, q.Inner);
        }


        /// <summary>
        /// Renames variables for prenexing binary operators. Renames all variables that are bound in one formula and occur in the other. Only variables of the right formula are renamed.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        private void RenameVariables(PrenexFormula right)
        {
            var toRename = new HashSet<Variable>(this.quantifiedVariables);
            toRename.IntersectWith(right.variables);

            var temp = new HashSet<Variable>(right.quantifiedVariables);
            temp.IntersectWith(this.variables);

            toRename.UnionWith(temp);

            var allVariables = new HashSet<Variable>(this.variables);
            allVariables.UnionWith(right.variables);

            foreach (var v in toRename)
            {
                Variable freshVar = GetAvailableVariable(v, unav: allVariables);
                allVariables.Add(freshVar);
                right.RenameVariable(v, freshVar);
            }

        }

        /// <summary>
        /// Recursively replaces all occurences of a variable by a new variable.
        /// </summary>
        /// <param name="p"></param>
        /// <param name="oldVar"></param>
        /// <param name="newVar"></param>
        
        private void RenameVariable(Variable oldVar, Variable newVar)
        {
            Formula = RenameVariable(Formula, oldVar, newVar);

            variables.Remove(oldVar);
            variables.Add(newVar);
            quantifiedVariables.Remove(oldVar);
            quantifiedVariables.Add(newVar);
        }

        private IFormula RenameVariable(IFormula f, Variable oldVar, Variable newVar)
        {
            if (f is Variable v)
            {
                if (v.Equals(oldVar))
                    return newVar;
                return f;
            }
            else if (f is Not n)
            {
                n.Inner = RenameVariable(n.Inner, oldVar, newVar);
                return f;
            }
            else if (f is BinaryOperator b)
            {
                b.Left = RenameVariable(b.Left, oldVar, newVar);
                b.Right = RenameVariable(b.Right, oldVar, newVar);
                return f;
            }
            else if (f is Quantifier q)
            {
                if (q.Variable.Equals(oldVar))
                    q.Variable = newVar;
                q.Inner = RenameVariable(q.Inner, oldVar, newVar);
                return f;
            }
            else
            {
                throw new NotImplementedException("impossible case");
            }
        }

        /// <summary>
        /// Returns the first of "vp", "vp1", "vp2", ... not appearing in unavailableVariables for a variable "v".
        /// </summary>
        /// <param name="v"></param>
        /// <param name="unav"></param>
        /// <param name="ending"></param>
        /// <returns></returns>
        private Variable GetAvailableVariable(Variable v, IEnumerable<Variable> unav)
        {
            if (!unav.Contains(v))
                return v;

            string ending = "p";
            var minusVariable = new Variable(v + ending);
            if (!unav.Contains(minusVariable))
            {
                return minusVariable;
            }

            int index = 1;
            Variable vMinus;
            do
            {
                vMinus = new Variable(v + ending + index);
                index++;
            }
            while (unav.Contains(vMinus));
            {
                index++;
            }
            return vMinus;
        }


        public override string ToString()
        {
            return Formula.ToString();
        }
    }
}
