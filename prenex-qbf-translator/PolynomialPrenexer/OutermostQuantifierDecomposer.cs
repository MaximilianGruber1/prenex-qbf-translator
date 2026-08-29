using prenex_qbf_translator.Language;
using System;
using System.Collections.Generic;
using System.Text;

namespace prenex_qbf_translator.Translator
{
    public class OutermostQuantifierDecomposer
    {


        /// <summary>
        /// Decomposes a formula into beta and a substitution according to Fact 4.
        /// </summary>
        /// <param name="formula">is changed and should never be used by the caller after the method call</param>4
        /// <param name="unavailableVariables">variables that cannot be used for fresh variables</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public Args Decompose(IFormula formula, FreshVariableGenerator varGenerator)
        {
            ArgumentNullException.ThrowIfNull(formula);

            if (formula is Quantifier)
            {
                var p = varGenerator.NextP();
                return new Args(p, new Substitution((p, formula)));
            }
            else if (formula is BooleanOperator b)
            {
                return DecomposeRecursive(b, varGenerator);
            }
            else // variable
            {
                return new Args(beta: formula, substitution: new Substitution());
            }
        }

        private Args DecomposeRecursive(BooleanOperator formula, FreshVariableGenerator varGenerator)
        {
            var subformulas = formula.Subformulas;

            List<IFormula> newSubformulas = new();
            Substitution substitution = new();
            foreach (var subformula in subformulas)
            {
                if (subformula is Quantifier)
                {
                    Variable p = varGenerator.NextP();
                    newSubformulas.Add(p);
                    substitution.Add(p, subformula);
                }
                else if (subformula is BooleanOperator b)
                {
                    var args = DecomposeRecursive(b, varGenerator);
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
