using prenex_qbf_translator.Language;
using System;
using System.Collections.Generic;
using System.Text;

namespace prenex_qbf_translator.Translator
{
    public static class OutermostQuantifierDecomposer
    {
        /// <summary>
        /// Decomposes a formula into beta and a substitution according to Fact 4.
        /// </summary>
        /// <param name="formula"></param>
        /// <param name="unavailableVariables">variables that do not occur in the formula but still can't be used</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static Args Decompose(IFormula formula, IEnumerable<Variable> unavailableVariables)
        {
            if (formula == null)
                throw new ArgumentException("Formula cannot be null.", nameof(formula));

            var unavailableVariablesList = new List<Variable>(unavailableVariables);
            unavailableVariablesList.AddRange(formula.Variables());
            formula = formula.DeepCopy();
            
            return DecomposeRecursive(formula, unavailableVariablesList);
        }

        private static Args DecomposeRecursive(IFormula formula, List<Variable> unavailableVariables)
        {
            IEnumerable<IFormula> subformulas = formula.Subformulas();

            if (!subformulas.Any())
            {
                return new Args(formula, new Substitution([]));
            }

            List<IFormula> newSubformulas = new();
            Dictionary<Variable, IFormula> substitutionDic = [];
            foreach (var subformula in subformulas)
            {
                if (IsQuantifier(subformula))
                {
                    Variable p = GetFreshP(unavailableVariables);
                    unavailableVariables.Add(p);
                    newSubformulas.Add(p);
                    substitutionDic.Add(p, subformula);
                }
                else
                {
                    var args = DecomposeRecursive(subformula, unavailableVariables);
                    newSubformulas.Add(args.Beta);
                    foreach (var kvp in args.Substitution.Entries)
                    {
                        substitutionDic.Add(kvp.Key, kvp.Value);
                    }
                }
            }

            return new Args(formula.CreateCopy(newSubformulas), new Substitution(substitutionDic));
        }





        private static bool IsQuantifier(IFormula formula)
        {
            return formula is Forall || formula is Exists;
        }

        private static Variable GetFreshP(IEnumerable<Variable> unavailableVariables)
        {
            int index = 1;
            Variable p;
            do
            {
                p = new Variable($"p{index}");
                index++;
            } while (unavailableVariables.Contains(p));
            return p;
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
