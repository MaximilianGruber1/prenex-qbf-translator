using System.Text;
using System.Xml;

namespace prenex_qbf_translator.Language
{
    public class Substitution
    {
        private Dictionary<Variable, IFormula> mappings;

        public IEnumerable<(Variable, IFormula)> Mappings
        {
            get
            {
                return mappings.Select(kvp => (kvp.Key, kvp.Value));
            }
        }

        public int Count => mappings.Count;


        public Substitution(params (Variable, IFormula)[] mappings)
        {
            ArgumentNullException.ThrowIfNull(mappings);
            this.mappings = [];
            foreach (var (from, to) in mappings)
            {
                ArgumentNullException.ThrowIfNull(to);
                this.mappings.Add(from, to);
            }
        }

        public Substitution(Substitution toCopy)
        {
            mappings = toCopy.mappings.ToDictionary();
        }

        public bool Contains(Variable v)
        {
            return mappings.ContainsKey(v);
        }

        /// <summary>
        /// Returns the formula v is mapped to. Throws exception if it does not contain v.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        public IFormula GetReplacement(Variable v)
        {
            return mappings[v];
        }

        public void Add(Variable variable, IFormula replacement)
        {
            mappings.Add(variable, replacement);
        }

        public void Add(Substitution sub)
        {
            foreach (var pair in sub.mappings)
            {
                mappings.Add(pair.Key, pair.Value);
            }
        }

        public void Remove(Variable v)
        {
            mappings.Remove(v);
        }
        

        public IFormula ApplyTo(IFormula f)
        {
            f = f.DeepCopy();
            return ApplyToRecursive(this, f);
        }

        private static IFormula ApplyToRecursive(Substitution subst, IFormula f)
        {
            if (f is Variable v)
            {
                foreach (var (from, to) in subst.Mappings)
                {
                    if (v.Equals(from))
                    {
                        return to;
                    }
                }
                return v;
            }
            else if (f is Quantifier q)
            {
                Substitution newSubst = new(subst);
                newSubst.Remove(q.Variable);
                q.Inner = ApplyToRecursive(newSubst, q.Inner);
                return q;
            }
            else if (f is Not n)
            {
                n.Inner = ApplyToRecursive(subst, n.Inner);
                return n;
            }
            else if (f is BinaryOperator b)
            {
                b.Left = ApplyToRecursive(subst, b.Left);
                b.Right = ApplyToRecursive(subst, b.Right);
                return b;
            }
            else
            {
                throw new NotImplementedException("impossible case");
            }
        }

        public override string ToString()
        {
            return "{" + string.Join(", ", mappings.Select(kvp => $"{kvp.Key} / {kvp.Value}")) + "}";
        }
    }
}
