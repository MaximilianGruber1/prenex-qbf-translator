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
            if (and.Left is Quantifier lq)
            {
                if (and.Right is Quantifier rq)
                {

                }
                else
                {

                }
            }
            else
            {
                if (and.Right is Quantifier rq)
                {

                }
            }
        }

        private void RenameVariable(Wrapper w, string oldName, string newName)
        {
            if (w.Formula is Variable v)
            {
                if (v.Name == oldName)
                    w.Formula = new Variable(newName);
            }
            else if (w.Formula is BooleanOperator b)
            {
                b.Subformulas = b.Subformulas.Select(subf => // prenex subformulas
                {
                    Wrapper w = new(subf);
                    RenameVariable(w, oldName, newName);
                    return w.Formula;
                });
            }
            else if (w.Formula is Quantifier q)
            {
                q.QuantifiedVariables = q.QuantifiedVariables.Select(qvar => qvar.Name == oldName ? new Variable(newName) : qvar);
                Wrapper ww = new(q.Inner);
                RenameVariable(ww, oldName, newName);
                q.Inner = ww.Formula;
            }
        }

        private IFormula FindBooleanFormulaRoot(IFormula prenexedFormula)
        {
            IFormula cur = prenexedFormula;
            while (cur is Quantifier q)
            {
                cur = q.Inner;
            }
            return cur;
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

        private class Wrapper(IFormula formula)
        {
            public IFormula Formula { get; set; } = formula;
        }
    }
}
