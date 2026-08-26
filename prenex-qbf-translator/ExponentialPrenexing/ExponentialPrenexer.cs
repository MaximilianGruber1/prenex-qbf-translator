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
            Wrapper w = new(f);
            PrenexRecursive(w);
            return w.Formula;
        }

        private void PrenexRecursive(Wrapper w)
        {
            if (w.Formula is Quantifier q)
            {
                var ww = new Wrapper(q.Inner);
                PrenexRecursive(ww);
                q.Inner = ww.Formula;
            }
            else if (w.Formula is BooleanOperator b)
            {
                b.Subformulas = b.Subformulas.Select(subf => // prenex subformulas
                {
                    Wrapper w = new(subf);
                    PrenexRecursive(w);
                    return w.Formula;
                });
                PrenexOneLayer(w);
            }
        }

        /// <summary>
        /// Prenexes boolean operator (inside a wrapper) with prexed subformulas
        /// </summary>
        /// <param name="w"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void PrenexOneLayer(Wrapper w)
        {
            if (w.Formula is Not n) // e.g. '!?x phi' becomes '#x !phi'
            {
                PullQuantifiersOutOfNot(w);
            }
            else if (w.Formula is And a)
            {
                PullQuantifiersOutOfAnd(w);

            }
            else if (w.Formula is Or or)
            {
                PullQuantifiersOutOfOr(w);
            }
        }

        /// <summary>
        /// Prenexes a Not with prenexed Inner
        /// </summary>
        /// <param name="w"></param>
        private void PullQuantifiersOutOfNot(Wrapper w)
        {
            var not = (Not)w.Formula;
            if (not.Inner is Quantifier q)
            {
                var ww = new Wrapper(new Not(q.Inner));
                PullQuantifiersOutOfNot(ww);
                var newInner = ww.Formula;

                var newOuter = CreateDual(q);
                newOuter.Inner = newInner;
                w.Formula = newOuter;
            }
            else // base case
            {
                SimplifyNotChain(w); // to avoid multiple negation
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

        private void SimplifyNotChain(Wrapper w)
        {
            while (w.Formula is Not outerNot && outerNot.Inner is Not innerNot)
            {
                w.Formula = innerNot.Inner;
            }
        }



        /// <summary>
        /// Prenexes an And with prenexed subformulas
        /// </summary>
        /// <param name="w"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void PullQuantifiersOutOfAnd(Wrapper w)
        {
            var and = (And)w.Formula;

            // rename all bound variables of Left that occur in Right
            var lBound = and.Left.BoundVariables(); 
            var rFree = and.Right.Variables();
            foreach (var v in rFree)
            {
                if (lBound.Contains(v))
                {
                    Variable freshVar = GetAvailableVariable(v, unav: and.Variables());
                    Wrapper ww = new(and.Left);
                    RenameVariable(ww, v, freshVar);
                    and.Left = ww.Formula;
                }
            }

            // rename all bound variables of Right that occur in Left
            var rBound = and.Right.BoundVariables(); 
            var lFree = and.Left.Variables();
            foreach (var v in lFree)
            {
                if (rBound.Contains(v))
                {
                    Variable freshVar = GetAvailableVariable(v, unav: and.Variables());
                    Wrapper ww = new(and.Right);
                    RenameVariable(ww, v, freshVar);
                    and.Right = ww.Formula;
                }
            }

            // rearrange formula tree
            if (and.Left is Quantifier lq)
            {
                if (and.Right is Quantifier rq)
                {
                    var lInnermostQ = FindInnermostQuantifier(lq);
                    var rInnermostQ = FindInnermostQuantifier(rq);
                    var lBooleanFormula = lInnermostQ.Inner;
                    var rBooleanFormula = rInnermostQ.Inner;

                    and.Left = lBooleanFormula;
                    and.Right = rBooleanFormula;
                    rInnermostQ.Inner = and;
                    lInnermostQ.Inner = rq;
                    w.Formula = lq;
                }
                else
                {
                    var lInnermostQ = FindInnermostQuantifier(lq);
                    var lBooleanFormula = lInnermostQ.Inner;

                    and.Left = lBooleanFormula;
                    lInnermostQ.Inner = and;
                    w.Formula = lq;
                }
            }
            else
            {
                if (and.Right is Quantifier rq)
                {
                    var rInnermostQ = FindInnermostQuantifier(rq);
                    var rBooleanFormula = rInnermostQ.Inner;

                    and.Right = rBooleanFormula;
                    rInnermostQ.Inner = and;
                    w.Formula = rq;
                }
                else
                {
                    // no quantifiers, therefore no prenexing needed
                }
            }
        }

        private void RenameVariable(Wrapper w, Variable oldVar, Variable newVar)
        {
            if (w.Formula is Variable v)
            {
                if (v.Equals(oldVar))
                    w.Formula = newVar;
            }
            else if (w.Formula is BooleanOperator b)
            {
                b.Subformulas = b.Subformulas.Select(subf => // prenex subformulas
                {
                    Wrapper w = new(subf);
                    RenameVariable(w, oldVar, newVar);
                    return w.Formula;
                });
            }
            else if (w.Formula is Quantifier q)
            {
                q.QuantifiedVariables = q.QuantifiedVariables.Select(qvar => qvar.Equals(oldVar) ? newVar : qvar);
                Wrapper ww = new(q.Inner);
                RenameVariable(ww, oldVar, newVar);
                q.Inner = ww.Formula;
            }
        }

        private Quantifier FindInnermostQuantifier(Quantifier prenexedFormula)
        {
            var cur = prenexedFormula;
            while (cur.Inner is Quantifier q)
            {
                cur = q;
            }
            return cur;
        }

        // treat the same as And
        private void PullQuantifiersOutOfOr(Wrapper w)
        {
            var or = (Or)w.Formula;
            var and = new And(or.Left, or.Right);

            Wrapper ww = new(and);
            PullQuantifiersOutOfAnd(ww);
            var prenexedAnd = ww.Formula;

            if (prenexedAnd is Quantifier q) // if there is at least one quantifier; otherwise do nothing
            {
                var innermostQ = FindInnermostQuantifier(q);
                var booleanAnd = (And)innermostQ.Inner;
                var booleanOr = new Or(booleanAnd.Left, booleanAnd.Right);
                innermostQ.Inner = booleanOr;

                w.Formula = q;
            }
        }



        /// <summary>
        /// returns the first of "vp", "vp1", "vp2", ... not appearing in unavailableVariables for a variable "v"
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

        private class Wrapper(IFormula formula)
        {
            public IFormula Formula { get; set; } = formula;
        }
    }
}
