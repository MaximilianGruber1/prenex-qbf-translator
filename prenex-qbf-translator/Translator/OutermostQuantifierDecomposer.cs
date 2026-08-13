using prenex_qbf_translator.Language;
using System;
using System.Collections.Generic;
using System.Text;

namespace prenex_qbf_translator.Translator
{
    public class OutermostQuantifierDecomposer
    {
        private readonly FreshVariableGenerator variableGenerator = new();


        /// <summary>
        /// Decomposes a formula into beta and a substitution according to Fact 4.
        /// </summary>
        /// <param name="formula"></param>
        /// <param name="unavailableVariables">variables that do not occur in the formula but still can't be used</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public Args Decompose(IFormula formula, IEnumerable<Variable> unavailableVariables = null)
        {
            unavailableVariables ??= [];
            if (formula == null)
                throw new ArgumentException("Formula cannot be null.", nameof(formula));

            var unav = new List<Variable>(unavailableVariables);
            unav.AddRange(formula.Variables());
            unav = unav.Distinct().ToList();
            formula = formula.DeepCopy();
            
            return DecomposeRecursive(formula, unav);
        }

        private Args DecomposeRecursive(IFormula formula, List<Variable> unavailableVariables)
        {
            if (formula is IQuantifier)
                throw new ArgumentException("This method is not supposed to be called with a quantifier");

            if (formula is IBooleanOperator op)
            {
                List<IFormula> newSubformulas = new();
                Dictionary<Variable, IFormula> substitutionDic = [];
                foreach (var subformula in op.Subformulas())
                {
                    if (IsQuantifier(subformula))
                    {
                        Variable p = variableGenerator.GetP(unavailableVariables);
                        unavailableVariables.Add(p);
                        newSubformulas.Add(p);
                        substitutionDic.Add(p, subformula);
                    }
                    else
                    {
                        var args = DecomposeRecursive(subformula, unavailableVariables);
                        newSubformulas.Add(args.Beta);
                        foreach (var kvp in args.Substitution.Dictionary)
                        {
                            substitutionDic.Add(kvp.Key, kvp.Value);
                        }
                    }
                }

                return new Args(op.CreateCopy(newSubformulas), new Substitution(substitutionDic));
            }

            // variable or constant
            return new Args(formula.DeepCopy(), new Substitution([]));


        }





        private bool IsQuantifier(IFormula formula)
        {
            return formula is Forall || formula is Exists;
        }


        public class Args
        {
            public IFormula Beta {  get; set; }
            public Substitution Substitution { get; set; }


            public Args(IFormula beta, Substitution substitution)
            {
                Beta = beta;
                Substitution = substitution;
            }

            public override string ToString()
            {
                return $"Beta: {Beta}, Substitution: {Substitution}";
            }
        }
    }
}
