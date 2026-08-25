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
        /// <param name="unavailableVariables">variables that do not occur in the formula but still must not be used</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public Args GetDecomposition(IFormula formula, IEnumerable<Variable>? unavailableVariables = null)
        {
            unavailableVariables ??= [];

            if (formula == null)
                throw new ArgumentException("Formula cannot be null.", nameof(formula));

            formula = formula.DeepCopy();
            unavailableVariables = unavailableVariables.Concat(formula.Variables());

            if (formula is Quantifier)
            {
                var p = variableGenerator.GetP(unavailableVariables);
                return new Args(p, new Substitution((p, formula)));
            }
            else if (formula is BooleanOperator b)
            {
                return DecomposeRecursive(b, unavailableVariables.ToList());
            }
            else // variable
            {
                return new Args(beta: formula, substitution: new Substitution());
            }
        }

        private Args DecomposeRecursive(BooleanOperator formula, List<Variable> unavailableVariables)
        {
            var subformulas = formula.Subformulas;

            List<IFormula> newSubformulas = new();
            Substitution substitution = new();
            foreach (var subformula in subformulas)
            {
                if (subformula is Quantifier)
                {
                    Variable p = variableGenerator.GetP(unavailableVariables);
                    unavailableVariables.Add(p);
                    newSubformulas.Add(p);
                    substitution.Add(p, subformula);
                }
                else if (subformula is BooleanOperator b)
                {
                    var args = DecomposeRecursive(b, unavailableVariables);
                    newSubformulas.Add(args.Beta);
                    substitution.Add(args.Substitution);
                }
                else // variable
                {
                    newSubformulas.Add(subformula);
                }
            }

            formula.Subformulas = newSubformulas;
            return new Args(formula, substitution);
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
