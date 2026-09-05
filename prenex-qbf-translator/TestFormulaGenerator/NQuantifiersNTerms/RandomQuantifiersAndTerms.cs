using prenex_qbf_translator.Language;
using System.Security.Cryptography;

namespace prenex_qbf_translator.TestFormulaGenerator.NQuantifiers
{
    public class RandomQuantifiersAndTerms
    {
        public IFormula GenerateFormula(int layers, int quantifiersPerLayer, int subformulas, bool firstLayerIsForall, bool firstLayerIsExists, bool simplified, int? seed)
        {
            if (subformulas < 1)
                throw new ArgumentException("only defined for >= 1 subformulas");

            if (firstLayerIsForall && firstLayerIsExists)
                throw new ArgumentException("'forall' and 'exists' cannot be true at the same time");

            Layer firstLayer;
            if (firstLayerIsForall)
                firstLayer = Layer.Forall;
            else if (firstLayerIsExists)
                firstLayer = Layer.Exists;
            else
                firstLayer = Layer.Random;

            Random rng = seed == null ? new() : new(seed.Value);
            VariableGenerator gen = new();

            Variable[] freeVars = GetVariableArray(gen, layers * quantifiersPerLayer);
            Variable[][] qVars = GetVariable2dArray(gen, layers, quantifiersPerLayer);

            if (subformulas == 1)
                return GenerateSubformula(freeVars, qVars, firstLayer, simplified, rng);

            IFormula[] subs = new IFormula[subformulas];
            for (int i = 0; i < subformulas; i++)
            {
                subs[i] = GenerateSubformula(freeVars, qVars, firstLayer, simplified, rng);
            }

            return new Equivalent(subs);
        }

        private Variable[] GetVariableArray(VariableGenerator gen, int n)
        {
            var result = new Variable[n];
            for (int i = 0; i < n; i++)
                result[i] = gen.Next();
            return result;
        }

        private Variable[][] GetVariable2dArray(VariableGenerator gen, int x, int y)
        {
            var result = new Variable[x][];
            for (int xx = 0; xx < x; xx++)
            {
                result[xx] = GetVariableArray(gen, y);
            }

            return result;
        }

        private IFormula GenerateSubformula(Variable[] freeVars, Variable[][] qVars, Layer firstLayer, bool simplified, Random rng)
        {
            IFormula quant = CombineRandomlyToBooleanFormula(qVars.SelectMany(v => v).ToArray(), simplified, rng);
            IFormula free = CombineRandomlyToBooleanFormula(freeVars, simplified, rng);
            IFormula f = new Equivalent(quant, free);
            f = Quantify(f, qVars, firstLayer, rng);

            return f;
        }

        private IFormula CombineRandomlyToBooleanFormula(Variable[] vars, bool simplified, Random rng)
        {
            List<IFormula> formulas = vars.Cast<IFormula>().ToList();

            // negate each variable with 50% chance
            for (int i = 0; i < formulas.Count; i++)
            {
                if (rng.Next(2) == 0)
                {
                    formulas[i] = new Not(formulas[i]);
                }
            }

            // combine randomly with binary operators
            while (formulas.Count > 1)
            {
                (int lIndex, int rIndex) = GetLeftAndRightIndex(formulas.Count, rng);
                var f1 = formulas[lIndex];
                var f2 = formulas[rIndex];

                formulas.RemoveAt(rIndex);
                formulas.RemoveAt(lIndex);

                if (simplified)
                {
                    f1 = rng.Next(3) switch
                    {
                        1 => new Equivalent(f1, f2),
                        2 => new And(f1, f2),
                        _ => new Or(f1, f2)
                    };
                }
                else
                {
                    f1 = rng.Next(3) switch
                    {
                        1 => new Equivalent(f1, f2),
                        2 => new And(f1, f2),
                        _ => rng.Next(3) switch
                        {
                            1 => new Or(f1, f2),
                            2 => new Implies(f1, f2),
                            _ => new IsImpliedBy(f1, f2)
                        }
                    };
                    if (rng.Next(2) == 0) 
                    {
                        f1 = new Not(f1);
                    }
                }
                
                formulas.Add(f1);
            }

            return formulas[0];
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
            return (i2, i1);
        }

        private IFormula Quantify(IFormula f, Variable[][] qVars, Layer firstLayer, Random rng)
        {
            bool layerIsForall;
            if (firstLayer == Layer.Random)
            {
                layerIsForall = rng.Next(2) == 0;
            }
            else
            {
                int layers = qVars.GetLength(0);
                layerIsForall =
                    layers % 2 == 1 && firstLayer == Layer.Forall ||
                    layers % 1 == 0 && firstLayer == Layer.Exists;
            }
            
            for (int i = qVars.GetLength(0) - 1; i >= 0; i--)
            {
                f = QuantifyOneLayer(f, layerIsForall, qVars[i]);
                layerIsForall = !layerIsForall;
            }

            return f;
        }

        private IFormula QuantifyOneLayer(IFormula f, bool isForall, Variable[] qVars)
        {
            if (isForall)
            {
                for (int i = qVars.Length - 1; i >= 0; i--)
                {
                    f = new Forall(qVars[i], f);
                }
            }
            else
            {
                for (int i = qVars.Length - 1; i >= 0; i--)
                {
                    f = new Exists(qVars[i], f);
                }
            }
            
            return f;
        }

        private enum Layer { Forall, Exists, Random }
    }
}
