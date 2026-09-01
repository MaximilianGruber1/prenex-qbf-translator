using prenex_qbf_translator.Language;
using System.Security.Cryptography;

namespace prenex_qbf_translator.TestFormulaGenerator.NQuantifiers
{
    public class RandomQuantifiersAndTerms
    {
        public IFormula GenerateFormula(int subformulas, int variablesPerSubformula, int seed)
        {
            return GenerateFormula(subformulas, variablesPerSubformula, new Random(seed));
        }

        public IFormula GenerateFormula(int equivalences, int variablesPerSubformula)
        {
            return GenerateFormula(equivalences, variablesPerSubformula, new Random());
        }

        private IFormula GenerateFormula(int nSubformulas, int variablesPerSubformula, Random rng)
        {
            VariableGenerator gen = new();

            if (nSubformulas <= 1)
            {
                return GenerateRandomSubformula(variablesPerSubformula, gen, rng);
            }

            IFormula[] subformulas = new IFormula[nSubformulas];
            for (int i = 0; i < nSubformulas; i++)
                subformulas[i] = GenerateRandomSubformula(variablesPerSubformula, gen, rng);
            return new Equivalent(subformulas);
        }

        private IFormula GenerateRandomSubformula(int nVariables, VariableGenerator gen, Random rng)
        {
            List<Variable> vars = new();

            for (int i = 0; i < nVariables; i++)
            {
                vars.Add(gen.Next());
            }

            IFormula f = CombineRandomly(vars, rng);
            f = AddQuantifiersRandomly(f, vars, rng);

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

        private IFormula AddQuantifiersRandomly(IFormula f, List<Variable> variables, Random rng)
        {
            bool isForall = rng.Next(2) == 0;

            for (int i = variables.Count-1; i >= 0; i--)
            {
                if (rng.Next(2) == 0 || i == 0) // ensure at least one quantifier is added
                {
                    f = isForall ? new Forall(variables[i], f) : new Exists(variables[i], f);
                }
            }
            
            foreach (var v in variables)
            {
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
