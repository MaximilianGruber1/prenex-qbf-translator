using prenex_qbf_translator.Language;
using prenex_qbf_translator.Translator;
using System;
using System.Collections.Generic;
using System.Text;

namespace prenex_qbf_translator.ExponentialPrenexing
{
    public class ExponentialPrenexer
    {
        private readonly FreshVariableGenerator variableGenerator = new();

        public IFormula Prenexed(IFormula f)
        {
            f = f.DeepCopy();
            return PrenexedRecursive(f);
        }
        
        private IFormula PrenexedRecursive(IFormula f)
        {
            if (f is Variable)
            {
                return f;
            }
            else if (f is Quantifier q)
            {
                q.Inner = PrenexedRecursive(q.Inner);
                return q;
            }
            else if (f is BooleanOperator b)
            {
                b.Subformulas = b.Subformulas.Select(PrenexedRecursive);
                return PrenexOneLayer(b);
            }
            else
            {
                throw new NotImplementedException("impossible case");
            }
        }

        private IFormula PrenexOneLayer(BooleanOperator b)
        {
            if (b is Not n) // e.g. '!?x phi' becomes '#x !phi'
            {
                return PullQuantifiersOutOfNot(n);
            }
            else if (b is And a)
            {
                var l = a.Left;
                var r = a.Right;
                if (l is Quantifier)
                {
                    if (r is Quantifier)
                    {

                    }
                    else
                    {
                        
                    }
                }
                else
                {
                    if (r is Quantifier)
                    {

                    }
                    else
                    {
                        return b;
                    }
                }
                
                
            }
            throw new NotImplementedException();
        }


        private IFormula PullQuantifiersOutOfNot(Not negatedPrenexedFormula)
        {
            if (negatedPrenexedFormula.Inner is Quantifier q)
            {
                var n = new Not(q.Inner);
                IFormula newInner = PullQuantifiersOutOfNot(n);
                var newOuter = CreateDual(q);
                newOuter.Inner = newInner;
                return newOuter;
            }
            else // base case
            {
                return SimplifyNotChain(negatedPrenexedFormula); // to avoid multiple negation
            }
        }

        private Quantifier CreateDual(Quantifier q)
        {
            if (q is Forall)
            {
                return new Exists(q.QuantifiedVariables, q.Inner);
            }
            else
            {
                return new Forall(q.QuantifiedVariables, q.Inner);
            }
        }

        private IFormula SimplifyNotChain(IFormula f)
        {
            while (f is Not outerNot && outerNot.Inner is Not innerNot)
            {
                f = innerNot.Inner;
            }
            return f;
        }

        

        /// <summary>
        /// returns the first of v, vx, vx1, vx2, ... not appearing in unavailableVariables for a variable 'v' and some string 'ending'
        /// </summary>
        /// <param name="v"></param>
        /// <param name="unav"></param>
        /// <param name="ending"></param>
        /// <returns></returns>
        private Variable GetAvailableVariable(Variable v, IEnumerable<Variable> unav)
        {
            if (!unav.Contains(v))
                return v;

            string ending = "p";
            var minusVariable = new Variable(v + ending);
            if (!unav.Contains(minusVariable))
            {
                return minusVariable;
            }

            int index = 1;
            Variable vMinus;
            do
            {
                vMinus = new Variable(v + ending + index);
                index++;
            }
            while (unav.Contains(vMinus));
            {
                index++;
            }
            return vMinus;
        }
    }
}
