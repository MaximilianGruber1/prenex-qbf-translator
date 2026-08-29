using prenex_qbf_translator.Language;

namespace prenex_qbf_translator.Translator
{
    /// <summary>
    /// Implements the polynomial prenexing approach described in the paper.
    /// </summary>
    public class PolynomialPrenexer
    {
        private readonly SmallTGenerator smallTGenerator = new();

        /// <summary>
        /// Generates T_exists according to Definition 5. T_exists and the initial formula are equisatisfiable. This method is used to prenex a formula for Limboole.
        /// </summary>
        /// <param name="formula"></param>
        /// <returns></returns>
        public IFormula Prenexed(IFormula formula)
        {
            formula = formula.DeepCopy();

            var unavailableVariables = ComputeVariables(formula);
            var varGenerator = new FreshVariableGenerator(unavailableVariables);

            return GenerateBigTRecursive(formula, forall: false, varGenerator).Formula;
        }


        private Result GenerateBigTRecursive(IFormula phi, bool forall, FreshVariableGenerator varGenerator)
        {
            if (IsBoolean(phi))
            {
                return new Result(phi, []);
            }

            var smallTResult = smallTGenerator.GenerateSmallT(phi, forall, varGenerator);
            IFormula psi = smallTResult.Formula;
            IEnumerable<Variable> pPhi = smallTResult.P;
            IEnumerable<Variable> nPhi = smallTResult.N;

            Result TPsiResult = GenerateBigTRecursive(psi, !forall, varGenerator);
            IFormula TPsi = TPsiResult.Formula;
            IEnumerable<Variable> nPsi = TPsiResult.N;

            IEnumerable<Variable> quantifiedVariables = [.. pPhi, .. nPsi];

            IFormula resultFormula = forall ?
                new Exists(quantifiedVariables, TPsi) :
                new Forall(quantifiedVariables, TPsi);

            return new Result(resultFormula, nPhi);
        }

        private HashSet<Variable> ComputeVariables(IFormula phi)
        {
            if (phi is Variable v)
            {
                return [v];
            }
            else if (phi is Quantifier q)
            {
                var vars = ComputeVariables(q.Inner);
                vars.Add(q.Variable);
                return vars;
            }
            else if (phi is Not n)
            {
                return ComputeVariables(n.Inner);
            }
            else if (phi is BinaryOperator b)
            {
                var vars = ComputeVariables(b.Left);
                vars.UnionWith(ComputeVariables(b.Right));
                return vars;
            }
            else
            {
                throw new Exception("impossible case");
            }
        }

        /// <summary>
        /// Checks whether the formula contains any quantifiers
        /// </summary>
        /// <param name="formula"></param>
        /// <returns></returns>
        private bool IsBoolean(IFormula formula)
        {
            if (formula is Variable)
            {
                return true;
            }
            else if (formula is Quantifier)
            {
                return false;
            }
            else if (formula is Not n)
            {
                return IsBoolean(n.Inner);
            }
            else if (formula is BinaryOperator b)
            {
                return IsBoolean(b.Left) && IsBoolean(b.Right);
            }
            else
            {
                throw new Exception("impossible case");
            }
        }


        private class Result(IFormula formula, IEnumerable<Variable> n)
        {
            public IFormula Formula { get; set; } = formula;
            public IEnumerable<Variable> N { get; set; } = n;
        }
    }
}
