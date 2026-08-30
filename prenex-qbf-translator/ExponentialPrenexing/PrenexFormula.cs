using prenex_qbf_translator.Language;
using System.Text;

namespace prenex_qbf_translator.ExponentialPrenexing
{
    public class PrenexFormula
    {
        private List<Quantifier> prefix;
        private IFormula matrix;

        private HashSet<Variable> variables;
        private HashSet<Variable> quantifiedVariables;

        public PrenexFormula(Variable variable)
        {
            prefix = [];
            matrix = variable;

            variables = [variable];
            quantifiedVariables = [];
        }

        private PrenexFormula(PrenexFormula p)
        {
            prefix = p.prefix.Select(q => new Quantifier(q)).ToList();
            matrix = p.matrix.DeepCopy();

            variables = [.. p.variables];
            quantifiedVariables = [.. p.quantifiedVariables];
        }

        public void Not()
        {
            SetToDual();
            matrix = 
                matrix is Not n ?
                n.Inner :
                new Not(matrix);
        }

        public void And(PrenexFormula right)
        {
            RenameVariables(right);

            prefix.AddRange(right.prefix);
            matrix = new And(matrix, right.matrix);

            variables.UnionWith(right.variables);
            quantifiedVariables.UnionWith(right.quantifiedVariables);
        }

        public void Or(PrenexFormula right)
        {
            RenameVariables(right);

            prefix.AddRange(right.prefix);
            matrix = new Or(matrix, right.matrix);

            variables.UnionWith(right.variables);
            quantifiedVariables.UnionWith(right.quantifiedVariables);
        }

        public void Implies(PrenexFormula right)
        {
            RenameVariables(right);

            this.SetToDual();
            prefix.AddRange(right.prefix);
            matrix = new Implies(matrix, right.matrix);

            variables.UnionWith(right.variables);
            quantifiedVariables.UnionWith(right.quantifiedVariables);
        }

        public void IsImpliedBy(PrenexFormula right)
        {
            RenameVariables(right);

            right.SetToDual();
            prefix.AddRange(right.prefix);
            matrix = new IsImpliedBy(matrix, right.matrix);

            variables.UnionWith(right.variables);
            quantifiedVariables.UnionWith(right.quantifiedVariables);
        }

        public void Equivalent(PrenexFormula right)
        {
            if (this.prefix.Count == 0 && right.prefix.Count == 0) // no decomposition of a<->b into a&b|!a&!b needed
            {
                matrix = new Equivalent(matrix, right.matrix);

                variables.UnionWith(right.variables);
                quantifiedVariables.UnionWith(right.quantifiedVariables);
            }
            else
            {
                var p2 = right;
                var p3 = new PrenexFormula(this);
                var p4 = new PrenexFormula(right);

                this.And(p2);
                p3.Not();
                p4.Not();
                p3.And(p4);
                this.Or(p3);
            }
        }

        public void Forall(Variable qvar)
        {
            prefix.Insert(0, new Quantifier(isForall: true, qvar));

            variables.Add(qvar);
            quantifiedVariables.Add(qvar);
        }

        public void Exists(Variable qvar)
        {
            prefix.Insert(0, new Quantifier(isForall: false, qvar));

            variables.Add(qvar);
            quantifiedVariables.Add(qvar);
        }

        public IFormula ToFormula()
        {
            var formula = matrix;

            for (int i = prefix.Count - 1; i >= 0; i--)
            {
                var quantifier = prefix[i];
                if (quantifier.IsForall)
                {
                    formula = new Forall(quantifier.Variable, formula);
                }
                else
                {
                    formula = new Exists(quantifier.Variable, formula);
                }
            }

            return formula;
        }

        /// <summary>
        /// Replaces each quantifier with its dual.
        /// </summary>
        /// <returns></returns>
        private void SetToDual()
        {
            foreach (var q in prefix)
            {
                q.IsForall = !q.IsForall;
            }
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
            RenamePrefix(oldVar, newVar);
            matrix = RenameVariableRecursive(matrix, oldVar, newVar);

            variables.Remove(oldVar);
            variables.Add(newVar);
            quantifiedVariables.Remove(oldVar);
            quantifiedVariables.Add(newVar);
        }

        private void RenamePrefix(Variable oldVar, Variable newVar)
        {
            foreach (var q in prefix)
            {
                if (q.Variable.Equals(oldVar))
                {
                    q.Variable = newVar;
                }
            }
        }

        private IFormula RenameVariableRecursive(IFormula f, Variable oldVar, Variable newVar)
        {
            if (f is Variable v)
            {
                if (v.Equals(oldVar))
                    return newVar;
                return f;
            }
            else if (f is Not n)
            {
                n.Inner = RenameVariableRecursive(n.Inner, oldVar, newVar);
                return f;
            }
            else if (f is BinaryOperator b)
            {
                b.Left = RenameVariableRecursive(b.Left, oldVar, newVar);
                b.Right = RenameVariableRecursive(b.Right, oldVar, newVar);
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
            return ToFormula().ToString()!;
        }


        public class Quantifier
        {
            public bool IsForall { get; set; }
            public Variable Variable { get; set; }


            public Quantifier(bool isForall, Variable variable)
            {
                IsForall = isForall;
                Variable = variable;
            }

            public Quantifier(Quantifier q)
            {
                IsForall = q.IsForall;
                Variable = q.Variable;
            }
        }
    }
}
