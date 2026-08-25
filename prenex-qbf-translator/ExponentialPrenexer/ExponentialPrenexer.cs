using prenex_qbf_translator.Language;
using prenex_qbf_translator.Translator;
using System;
using System.Collections.Generic;
using System.Text;

namespace prenex_qbf_translator.ExponentialPrenexer
{
    public class ExponentialPrenexer
    {
        private readonly FreshVariableGenerator variableGenerator = new();

        public IFormula GeneratePrenexedFormula(IFormula f)
        {
            throw new NotImplementedException();
        }
        /*
        private IFormula PrenexedRecursive(IFormula f)
        {
            if 
        }

        private IFormula MoveQuantifierFromSecondToFirstLayer(BooleanOperator b)
        {
            if (!b.Subformulas.Any(sub => sub is Quantifier))
            {
                throw new Exception("This method is supposed to be called with a quantifier as a subformula.");
            }
            
            if (b is Not n) // e.g. '!?x phi' becomes '#x !phi'
            {
                var q = (Quantifier)n.Inner;
                var newInner = new Not(q.Inner);
                return CreateDual(q, newInner);
            }
            else if (b is And a)
            {
                List<Variable> unav = a.Subformulas.SelectMany(s => s.FreeVariables()).ToList();
                
                foreach (var sub in a.Subformulas)
                {
                    if (sub is Quantifier q)
                    {
                        foreach (Variable qvar in q.QuantifiedVariables)
                        {
                            Variable newQVar = GetAvailableVariable(qvar, unav);
                            unav.Add(newQVar);
                        }
                    }
                }
            }
        }


        private IFormula CreateDual(Quantifier q, IFormula newInner)
        {
            if (q is Forall forall)
            {
                return new Exists(forall.QuantifiedVariables, newInner);
            }
            else // exists
            {
                var exists = (Exists)q;
                return new Forall(exists.QuantifiedVariables, newInner);
            }
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
        */
    }
}
