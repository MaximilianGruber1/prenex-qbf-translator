using prenex_qbf_translator.Language;

namespace prenex_qbf_translator.ExponentialPrenexing
{
    public class FormulaDuplicator
    {
        public IFormula Duplicate(IFormula f)
        {
            if (f is Variable var)
            {
                return f;
            }
            else if (f is Exists e)
            {
                return new Exists(e.Variable, Duplicate(e.Inner));
            }
            else if (f is Forall fa)
            {
                return new Forall(fa.Variable, Duplicate(fa.Inner));
            }
            else if (f is Not n)
            {
                return new Not(Duplicate(n.Inner));
            }
            else if (f is And a)
            {
                return new And(Duplicate(a.Left), Duplicate(a.Right));
            }
            else if (f is Or o)
            {
                return new Or(Duplicate(o.Left), Duplicate(o.Right));
            }
            else if (f is Implies i)
            {
                return new Implies(Duplicate(i.Left), Duplicate(i.Right));
            }
            else if (f is IsImpliedBy iib)
            {
                return new IsImpliedBy(Duplicate(iib.Left), Duplicate(iib.Right));
            }
            else if (f is Equivalent eq)
            {
                return new Equivalent(Duplicate(eq.Left), Duplicate(eq.Right));
            }
            else
            {
                throw new Exception("impossible case");
            }
        }
    }
}
