using prenex_qbf_translator.Language;
using prenex_qbf_translator.Translator;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace prenex_qbf_translator.ExponentialPrenexing
{
    public class ExponentialPrenexer
    {
        
        public IFormula Prenexed(IFormula f)
        {
            f = f.DeepCopy();
            return PrenexRecursive(f).Formula;
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
            else if (f is Quantifier q)
            {
                PrenexFormula prenexInner = PrenexRecursive(q.Inner);
                q.Inner = prenexInner.Formula; // prenex inner
                return new PrenexFormula(q);
            }
            if (f is Not n)
            {
                PrenexFormula prenexInner = PrenexRecursive(n.Inner);
                return PrenexNot(prenexInner);
            }
            else if (f is BinaryOperator bo)
            {
                PrenexFormula prenexLeft = PrenexRecursive(bo.Left);
                PrenexFormula prenexRight = PrenexRecursive(bo.Right);

                if (bo is And a)
                {
                    return PrenexAnd(prenexLeft, prenexRight);
                }
                else if (bo is Or o)
                {
                    return PrenexOr(prenexLeft, prenexRight);
                }
                else if (bo is Implies i)
                {
                    return PrenexImplies(prenexLeft, prenexRight);
                }
                else if (bo is IsImpliedBy iib)
                {
                    return PrenexIsImpliedBy(prenexLeft, prenexRight);
                }
                else if (bo is Equivalent e)
                {
                    return PrenexEquivalent(prenexLeft, prenexRight);
                }
                else
                {
                    throw new Exception("impossible case");
                }
            }
            else
            {
                throw new Exception("impossible case");
            }
        }



        /// <summary>
        /// Prenexes the formula '!inner'.
        /// </summary>
        /// <param name="inner"></param>
        private PrenexFormula PrenexNot(PrenexFormula inner)
        {
            inner.SetToDual();
            var oldMatrix = inner.GetMatrix();
            PrenexFormula newMatrix = new(
                oldMatrix is Not n ?
                n.Inner :
                new Not(inner.GetMatrix()));
            inner.ReplaceMatrix(newMatrix);
            return inner;
        }

        /// <summary>
        /// Prenexes the formula 'left & right'.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        private PrenexFormula PrenexAnd(PrenexFormula left, PrenexFormula right)
        {
            RenameVariables(left, right);

            PrenexFormula newMatrix = new(new And(left.GetMatrix(), right.GetMatrix()));
            right.ReplaceMatrix(newMatrix);
            left.ReplaceMatrix(right);
            return left;
        }

        /// <summary>
        /// Prenexes the formula 'left | right'.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        private PrenexFormula PrenexOr(PrenexFormula left, PrenexFormula right)
        {
            RenameVariables(left, right);

            PrenexFormula newMatrix = new(new Or(left.GetMatrix(), right.GetMatrix()));
            right.ReplaceMatrix(newMatrix);
            left.ReplaceMatrix(right);
            return left;
        }

        /// <summary>
        /// Prenexes the formula 'left -> right'.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        private PrenexFormula PrenexImplies(PrenexFormula left, PrenexFormula right)
        {
            RenameVariables(left, right);

            left.SetToDual();
            PrenexFormula newMatrix = new(new Implies(left.GetMatrix(), right.GetMatrix()));
            right.ReplaceMatrix(newMatrix);
            left.ReplaceMatrix(right);
            return left;
        }

        /// <summary>
        /// Prenexes the formula 'left <- right'.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        private PrenexFormula PrenexIsImpliedBy(PrenexFormula left, PrenexFormula right)
        {

            RenameVariables(left, right);

            right.SetToDual();
            PrenexFormula newMatrix = new(new IsImpliedBy(left.GetMatrix(), right.GetMatrix()));
            right.ReplaceMatrix(newMatrix);
            left.ReplaceMatrix(right);
            return left;
        }

        /// <summary>
        /// Prenexes the formula 'left <-> right' by transforming it to 'left & right | !left & !right.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        private PrenexFormula PrenexEquivalent(PrenexFormula left, PrenexFormula right)
        {
            if (left.IsBoolean() && right.IsBoolean())
                return new(new Equivalent(left.Formula, right.Formula));

            PrenexFormula p1 = left;
            PrenexFormula p2 = right;
            PrenexFormula p3 = left.DeepCopy();
            PrenexFormula p4 = right.DeepCopy();

            PrenexFormula and12 = PrenexAnd(p1, p2);
            PrenexFormula not3 = PrenexNot(p3);
            PrenexFormula not4 = PrenexNot(p4);
            PrenexFormula and34 = PrenexAnd(not3, not4);
            PrenexFormula or = PrenexOr(and12, and34);

            return or;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        private void RenameVariables(PrenexFormula left, PrenexFormula right)
        {
            // rename all variables of 'Right' that are bound in 'Left'
            var lBound = left.BoundVariables();
            var rvar = right.Variables();
            foreach (var v in rvar)
            {
                if (lBound.Contains(v))
                {
                    var allVariables = left.Variables().Concat(right.Variables()).Distinct();
                    Variable freshVar = GetAvailableVariable(v, unav: allVariables);
                    RenameVariable(right, v, freshVar);
                }
            }

            // rename all bound variables of Right that occur in Left
            var rBound = right.BoundVariables();
            var lvar = left.Variables();
            foreach (var v in lvar)
            {
                if (rBound.Contains(v))
                {
                    var allVariables = left.Variables().Concat(right.Variables()).Distinct();
                    Variable freshVar = GetAvailableVariable(v, unav: allVariables);
                    RenameVariable(right, v, freshVar);
                }
            }
        }

        private void RenameVariable(PrenexFormula p, Variable oldVar, Variable newVar)
        {
            if (p.Formula is Variable v)
            {
                if (v.Equals(oldVar))
                    p.Formula = newVar;
            }
            else if (p.Formula is BooleanOperator b)
            {
                b.Subformulas = b.Subformulas.Select(subf => // prenex subformulas
                {
                    PrenexFormula pp = new(subf);
                    RenameVariable(pp, oldVar, newVar);
                    return pp.Formula;
                });
            }
            else if (p.Formula is Quantifier q)
            {
                if (q.QuantifiedVariable.Equals(oldVar))
                    q.QuantifiedVariable = newVar;
                PrenexFormula ww = new(q.Inner);
                RenameVariable(ww, oldVar, newVar);
                q.Inner = ww.Formula;
            }
        }

        /// <summary>
        /// Returns the first of "vp", "vp1", "vp2", ... not appearing in unavailableVariables for a variable "v".
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
