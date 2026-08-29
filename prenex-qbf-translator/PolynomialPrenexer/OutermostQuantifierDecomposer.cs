using prenex_qbf_translator.Language;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace prenex_qbf_translator.Translator
{
    public class OutermostQuantifierDecomposer
    {
        /// <summary>
        /// Decomposes a formula into beta and a substitution according to Fact 4.
        /// </summary>
        /// <param name="formula">is changed and should never be used by the caller after the method call</param>
        /// <param name="varGenerator"></param>
        /// <returns></returns>
        public Result Decompose(IFormula formula, FreshVariableGenerator varGenerator)
        {
            ArgumentNullException.ThrowIfNull(formula);
            ArgumentNullException.ThrowIfNull(varGenerator);

            if (formula is Variable)
            {
                return new Result(beta: formula, substitution: new Substitution());
            }
            if (formula is Quantifier)
            {
                var p = varGenerator.NextP();
                return new Result(p, new Substitution((p, formula)));
            }
            else if (formula is Not n)
            {
                var decomposedInner = Decompose(n.Inner, varGenerator);
                n.Inner = decomposedInner.Beta;
                return new Result(n, decomposedInner.Substitution);
            }
            else if (formula is BinaryOperator b)
            {
                var decomposedLeft = Decompose(b.Left, varGenerator);
                var decomposedRight = Decompose(b.Right, varGenerator);
                var lBeta = decomposedLeft.Beta;
                var rBeta = decomposedRight.Beta;
                var lSub = decomposedLeft.Substitution;
                var rsub = decomposedRight.Substitution;

                b.Left = lBeta;
                b.Right = rBeta;

                var combinedSub = lSub;
                combinedSub.Add(rsub);

                return new Result(b, combinedSub);
            }
            else
            {
                throw new Exception("impossible case");
            }
        }


        public class Result
        {
            public IFormula Beta {  get; set; }
            public Substitution Substitution { get; set; }


            public Result(IFormula beta, Substitution substitution)
            {
                Beta = beta;
                Substitution = substitution;
            }
        }
    }
}
