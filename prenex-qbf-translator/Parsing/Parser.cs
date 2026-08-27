using prenex_qbf_translator.Language;
using static prenex_qbf_translator.Parsing.Token.Kind;

namespace prenex_qbf_translator.Parsing
{
    public class Parser
    {
        /// <summary>
        /// Last recognized token;
        /// </summary>
        private Token t;

        /// <summary>
        /// Lookahead token (not recognized)
        /// </summary>
        private Token la;

        /// <summary>
        /// Shortcut to kind attribute of lookahead token (la)
        /// </summary>
        private Token.Kind sym;

        private readonly Scanner scanner;

        public Parser(string formula)
        {
            scanner = new Scanner(formula);
        }

        public IFormula Parse()
        {
            Scan(); // load first token
            IFormula equivalence = Equivalence();
            Check(Eof);
            return equivalence;
        }

        private IFormula Equivalence()
        {
            IFormula first = Implies();

            List<IFormula> other = [];
            while (sym == Equiv)
            {
                Scan();
                IFormula next = Implies();
                other.Add(next);
            }

            if (other.Count == 0) // only one subformula, return this subformula
            {
                return first;
            }
            else // more than one subformula, return new disjunction of these subformulas
            {
                return new Equivalent(first, other[0], other[1..].ToArray());
            }
        }

        private IFormula Implies()
        {
            IFormula left = Disjunction();

            if (sym == Token.Kind.Implies)
            {
                Scan();
                IFormula right = Disjunction();
                return new Implies(left, right);
            }
            else if (sym == Token.Kind.IsImpliedBy)
            {
                Scan();
                IFormula right = Disjunction();
                return new IsImpliedBy(left, right);
            }
            else
            {
                return left;
            }
        }

        private IFormula Disjunction()
        {
            IFormula first = Conjunction();

            List<IFormula> other = [];
            while (sym == Token.Kind.Or)
            {
                Scan();
                IFormula next = Conjunction();
                other.Add(next);
            }

            if (other.Count == 0)
            {
                return first;
            }
            else
            {
                return new Or(first, other[0], other[1..].ToArray());
            }
        }

        private IFormula Conjunction()
        {
            IFormula first = Unary();

            List<IFormula> other = [];
            while (sym == Token.Kind.And)
            {
                Scan();
                IFormula next = Unary();
                other.Add(next);
            }

            if (other.Count == 0)
            {
                return first;
            }
            else
            {
                return new And(first, other[0], other[1..].ToArray());
            }
        }

        private IFormula Unary()
        {
            if (sym == Token.Kind.Not)
            {
                Scan();
                IFormula f = Unary();
                return new Not(f);
            }
            else
            {
                IFormula expr = Expr();
                return expr;
            }
        }

        private IFormula Expr()
        {
            if (sym == Token.Kind.Variable)
            {
                Scan();
                return new Variable(t.Name);
            }
            else if (sym == LPar)
            {
                Scan();
                var equiv = Equivalence();
                Check(RPar);
                return equiv;
            }
            else if (sym == Token.Kind.Forall)
            {
                Scan();
                Check(Token.Kind.Variable);
                Variable v = new(t.Name);
                IFormula inner = Unary();
                return new Forall(v, inner);
            }
            else if (sym == Token.Kind.Exists)
            {
                Scan();
                Check(Token.Kind.Variable);
                Variable v = new(t.Name);
                IFormula inner = Unary();
                return new Exists(v, inner);
            }
            else
            {
                throw new Exception(GetExceptionMessagePrefix(la.Line, la.Column) + 
                    $"invalid token '{la.GetStringRepresentationOfKind()}'");
            }
        }



        /// <summary>
        /// reads ahead one symbol
        /// </summary>
        private void Scan()
        {
            t = la;
            la = scanner.Next();
            sym = la.Kind_;
        }

        /// <summary>
        /// verifies symbol and reads ahead
        /// </summary>
        /// <param name="expected"></param>
        private void Check(Token.Kind expected)
        {
            if (sym == expected)
            {
                Scan();
            }
            else
            {
                throw new Exception(GetExceptionMessagePrefix(la.Line, la.Column) + 
                    $"invalid token '{la.GetStringRepresentationOfKind()}', expected '{expected}'");
            }
        }

        private string GetExceptionMessagePrefix(int line, int col)
        {
            return $"{line}:{col}: parse error: ";
        }
    }
}
