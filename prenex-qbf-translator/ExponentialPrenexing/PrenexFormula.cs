using prenex_qbf_translator.FormulaGeneration;
using prenex_qbf_translator.Language;
using System.Text;

namespace prenex_qbf_translator.ExponentialPrenexing
{
    public class PrenexFormula
    {
        private List<Quantifier> prefix;
        private IFormula matrix;

        // store variables by alphabet to make renaming deterministic, which is convenient for testing
        private SortedSet<Variable> variables; 
        private SortedSet<Variable> quantifiedVariables;

        private readonly ExpVariableGenerator variableGenerator;

        private readonly FormulaDuplicator duplicator = new();


        public PrenexFormula(Variable variable, ExpVariableGenerator gen)
        {
            prefix = [];
            matrix = variable;

            variables = [variable];
            quantifiedVariables = [];

            variableGenerator = gen;

            variableGenerator.AddUnavailableVariable(variable);
        }

        private PrenexFormula(PrenexFormula p)
        {
            prefix = p.prefix.Select(q => new Quantifier(q)).ToList();
            matrix = duplicator.Duplicate(p.matrix);

            variables = [.. p.variables];
            quantifiedVariables = [.. p.quantifiedVariables];

            variableGenerator = p.variableGenerator;
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
            var result = matrix;
            for (int i = prefix.Count - 1; i >= 0; i--)
            {
                Quantifier q = prefix[i];
                result = q.IsForall ?
                    new Forall(q.Variable, result) :
                    new Exists(q.Variable, result);
            }
            return result;
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
            var toRename = new SortedSet<Variable>(this.quantifiedVariables);
            toRename.IntersectWith(right.variables);

            var temp = new SortedSet<Variable>(right.quantifiedVariables);
            temp.IntersectWith(this.variables);

            toRename.UnionWith(temp);

            foreach (var v in toRename)
            {
                Variable freshVar = variableGenerator.Next(v);
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

        public override string ToString()
        {
            return ToFormula().ToString()!;
        }


        private class Quantifier
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

            public override string ToString()
            {
                return IsForall ? ("#" + Variable) : ("?" + Variable);
            }
        }
    }
}
