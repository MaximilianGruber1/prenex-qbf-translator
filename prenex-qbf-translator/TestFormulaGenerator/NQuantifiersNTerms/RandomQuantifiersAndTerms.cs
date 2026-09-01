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

            List<Variable> qVars = GetNVariables(gen, nQvars);
            List<Variable> freeVars = GetNVariables(gen, nFreeVars);

            IFormula[] subformulas = new IFormula[nSubf];
            for (int i = 0; i < nSubf; i++)
            {
                subformulas[i] = GenerateSubformula(isForall: false, qVars, freeVars, rng);
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

        private IFormula GenerateSubformula(bool isForall, List<Variable> quantifiedVariables, List<Variable> freeVariables, Random rng)
        {
            IFormula f = GenerateBooleanFormula([.. quantifiedVariables, ..freeVariables], rng);
            f = Quantifiy(f, isForall, quantifiedVariables);

            return f;
        }

        private IFormula GenerateBooleanFormula(List<Variable> variables, Random rng)
        {
            List<IFormula> formulas = [.. variables.Select(v => (IFormula)v)];

            while (formulas.Count > 1)
            {
                (int index1, int index2) = GetLeftAndRightIndex(formulas.Count, rng);
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

        private (int, int) GetLeftAndRightIndex(int n, Random rng)
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
                    f = new Forall(qVars[i], f);
                }
            }
            else
            {
                for (int i = qVars.Count - 1; i >= 0; i--)
                {
                    f = new Exists(qVars[i], f);
                }
            }

            return f;
        }


        private record QuantifiedVariables(QuantifierType type, HashSet<Variable> var);


        private enum QuantifierType
        {
            Forall, Exists
        }



        public IFormula GenerateFormula2(int nSubf, int nQvars, int nFreeVars, Random rng)
        {
            VariableGenerator gen = new();

            if (nSubf <= 0)
            {
                throw new ArgumentException("only defined for >= 1 subformulas");
            }

            List<Variable> outerQvars = GetNVariables(gen, nQvars);
            List<Variable> innerQvars = GetNVariables(gen, nQvars);
            List<Variable> freeVars = GetNVariables(gen, nFreeVars);

            IFormula[] subformulas = new IFormula[nSubf];
            for (int i = 0; i < nSubf; i++)
            {
                subformulas[i] = GenerateBooleanFormula([.. outerQvars, .. innerQvars, .. freeVars], rng);
                subformulas[i] = Quantifiy(subformulas[i], isForall: false, innerQvars);
                subformulas[i] = Quantifiy(subformulas[i], isForall: true, outerQvars);
            }

            if (nSubf == 1)
                return subformulas[0];
            return new Equivalent(subformulas);
        }
    }

    
}
