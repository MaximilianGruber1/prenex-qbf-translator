using eqprenex.Language;
using eqprenex.PolynomialPrenexing;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace eqprenex.ExponentialPrenexing
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
            ExpVariableGenerator gen = new();

            return PrenexRecursive(f, gen).ToFormula();
        }

        /// <summary>
        /// No deep copy for performance, breaks f.
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private PrenexFormula PrenexRecursive(IFormula f, ExpVariableGenerator gen)
        {
            if (f is Variable v)
            {
                return new PrenexFormula(v, gen);
            }
            else if (f is Exists e)
            {
                PrenexFormula prenexInner = PrenexRecursive(e.Inner, gen);
                prenexInner.Exists(e.Variable);
                return prenexInner;
            }
            else if (f is Forall a)
            {
                PrenexFormula prenexInner = PrenexRecursive(a.Inner, gen);
                prenexInner.Forall(a.Variable);
                return prenexInner;
            }
            if (f is Not n)
            {
                PrenexFormula prenexInner = PrenexRecursive(n.Inner, gen);
                prenexInner.Not();
                return prenexInner;
            }
            else if (f is BinaryOperator b)
            {
                PrenexFormula prenexLeft = PrenexRecursive(b.Left, gen);
                PrenexFormula prenexRight = PrenexRecursive(b.Right, gen);

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
