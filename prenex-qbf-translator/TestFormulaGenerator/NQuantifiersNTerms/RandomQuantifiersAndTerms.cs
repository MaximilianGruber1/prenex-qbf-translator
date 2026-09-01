using prenex_qbf_translator.Language;
using System.Security.Cryptography;

namespace prenex_qbf_translator.TestFormulaGenerator.NQuantifiers
{
    public class RandomQuantifiersAndTerms
    {
        public IFormula GenerateFormula(int subformulas, int quantifiedVariablesPerSubformula, int freeVariables, int seed)
        {
            return GenerateFormula(subformulas, quantifiedVariablesPerSubformula, freeVariables, new Random(seed));
        }

        public IFormula GenerateFormula(int subformulas, int quantifiedVariablesPerSubformula, int freeVariables)
        {
            return GenerateFormula(subformulas, quantifiedVariablesPerSubformula, freeVariables, new Random());
        }

        private IFormula GenerateFormula(int nSubf, int nQvars, int nFreeVars, Random rng)
        {
            VariableGenerator gen = new();

            if (nSubf <= 0)
            {
                throw new ArgumentException("only defined for >= 1 subformulas");
            }

            List<Variable> freeVars = GetNVariables(gen, nFreeVars);

            IFormula[] subformulas = new IFormula[nSubf];
            for (int i = 0; i < nSubf; i++)
            {
                List<Variable> qVars = GetNVariables(gen, nQvars);
                subformulas[i] = GenerateRandomSubformula(qVars, freeVars, rng);
            }

            if (nSubf == 1)
                return subformulas[0];
            return new Equivalent(subformulas);
        }

        private List<Variable> GetNVariables(VariableGenerator gen, int n)
        {
            List<Variable> result = [];
            for (int i = 0; i < n; i++)
                result.Add(gen.Next());
            return result;
        }

        private IFormula GenerateRandomSubformula(List<Variable> quantifiedVariables, List<Variable> freeVariables, Random rng)
        {
            IFormula f = CombineRandomly([.. quantifiedVariables, ..freeVariables], rng);
            f = Quantifiy(f, isForall: rng.Next(2) == 0, quantifiedVariables);

            return f;
        }

        /// <summary>
        /// combines a set of formulas to a formula randomly
        /// </summary>
        /// <param name="formulas"></param>
        /// <param name="rng"></param>
        /// <returns></returns>
        private IFormula CombineRandomly(List<Variable> variables, Random rng)
        {
            List<IFormula> formulas = [.. variables.Select(v => (IFormula)v)];

            while (formulas.Count > 1)
            {
                (int index1, int index2) = GetRandomLeftAndRightIndex(formulas.Count, rng);
                var f1 = formulas.ElementAt(index1);
                var f2 = formulas.ElementAt(index2);

                if (rng.Next(6) == 0) // apply Not to f1
                {
                    formulas.Remove(f1);
                    f1 = f1 is Not n ? n.Inner : new Not(f1);
                    formulas.Add(f1);
                }
                else // combine f1 and f2 with a random binary operator
                {
                    formulas.Remove(f1);
                    formulas.Remove(f2);

                    f1 = rng.Next(5) switch
                    {
                        1 => new And(f1, f2),
                        2 => new Or(f1, f2),
                        3 => new Implies(f1, f2),
                        4 => new IsImpliedBy(f1, f2),
                        _ => new Equivalent(f1, f2),
                    };

                    formulas.Add(f1);
                }
            }

            return formulas.First();
        }

        private (int, int) GetRandomLeftAndRightIndex(int n, Random rng)
        {
            if (n < 2) throw new ArgumentException("n must be at least 2");

            int i1 = rng.Next(n);
            int i2 = rng.Next(n);
            while (i2 == i1)
            {
                i2 = rng.Next(n);
            }

            if (i1 < i2)
                return (i1, i2);
            else
                return (i2, i1);
        }

        private IFormula Quantifiy(IFormula f, bool isForall, List<Variable> qVars)
        {
            if (isForall)
            {
                for (int i = qVars.Count - 1; i >= 0; i--)
                {
                    new Forall(qVars[i], f);
                }
            }
            else
            {
                for (int i = qVars.Count - 1; i >= 0; i--)
                {
                    new Exists(qVars[i], f);
                }
            }

            return f;
        }


        private record QuantifiedVariables(QuantifierType type, HashSet<Variable> var);


        private enum QuantifierType
        {
            Forall, Exists
        }
    }

    
}
