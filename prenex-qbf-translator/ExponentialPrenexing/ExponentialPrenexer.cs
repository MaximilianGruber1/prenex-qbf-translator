using prenex_qbf_translator.Language;
using prenex_qbf_translator.PolynomialPrenexing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace prenex_qbf_translator.ExponentialPrenexing
{
    public class ExponentialPrenexer
    {
        /// <summary>
        /// Prenexes a formula by shifting quantifiers, renaming variables and replacing 'a <-> b' by 'a & b | !a & !b'.
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public IFormula Prenexed(IFormula f)
        {
            return PrenexRecursive(f).ToFormula();
        }

        /// <summary>
        /// No deep copy for performance, breaks f.
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private PrenexFormula PrenexRecursive(IFormula f)
        {
            if (f is Variable v)
            {
                return new PrenexFormula(v);
            }
            else if (f is Exists e)
            {
                PrenexFormula prenexInner = PrenexRecursive(e.Inner);
                prenexInner.Exists(e.Variable);
                return prenexInner;
            }
            else if (f is Forall a)
            {
                PrenexFormula prenexInner = PrenexRecursive(a.Inner);
                prenexInner.Forall(a.Variable);
                return prenexInner;
            }
            if (f is Not n)
            {
                PrenexFormula prenexInner = PrenexRecursive(n.Inner);
                prenexInner.Not();
                return prenexInner;
            }
            else if (f is BinaryOperator b)
            {
                PrenexFormula prenexLeft = PrenexRecursive(b.Left);
                PrenexFormula prenexRight = PrenexRecursive(b.Right);

                if (b is And)
                {
                    prenexLeft.And(prenexRight);
                }
                else if (b is Or)
                {
                    prenexLeft.Or(prenexRight);
                }
                else if (b is Implies)
                {
                    prenexLeft.Implies(prenexRight);
                }
                else if (b is IsImpliedBy)
                {
                    prenexLeft.IsImpliedBy(prenexRight);
                }
                else if (b is Equivalent)
                {
                    prenexLeft.Equivalent(prenexRight);
                }
                else
                {
                    throw new Exception("impossible case");
                }

                return prenexLeft;
            }
            else
            {
                throw new Exception("impossible case");
            }
        }



        

        

        

        


        
    }
}
