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

        public Parser(Scanner scanner)
        {
            this.scanner = scanner;
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
            IFormula left = Implies();

            if (sym == Equiv) // 2 subformulas
            {
                Scan();
                IFormula right = Implies();
                return new Equivalent(left, right);
            }
            else // 1 subformula
            {
                return left;
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

            if (other.Count == 0) // only one subformula, return this subformula
            {
                return first;
            }
            else // more than one subformula, return new disjunction of these subformulas
            {
                var all = other.Prepend(first);
                return new Or(all);
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
                var all = other.Prepend(first);
                return new And(all);
            }
        }

        private IFormula Unary()
        {
            int count = 0;
            while (sym == Token.Kind.Not)
            {
                Scan();
                count++;
            }
            IFormula f = Expr();
            for (int i = 0; i < count; i++)
            {
                f = new Not(f);
            }
            return f;
        }

        private IFormula Expr()
        {
            if (sym == True)
            {
                Scan();
                return new TrueConstant();
            }
            else if (sym == False)
            {
                Scan();
                return new FalseConstant();
            }
            else if (sym == Token.Kind.Variable)
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
                (var variables, var inner) = QuantifierRest();
                return new Forall(variables, inner);
            }
            else if (sym == Token.Kind.Exists)
            {
                Scan();
                (var variables, var inner) = QuantifierRest();
                return new Exists(variables, inner);
            }
            else
            {
                throw new Exception($"Invalid token '{la.GetStringRepresentationOfKind()}' at line {la.Line}, column {la.Column}.");
            }
        }

        private (IEnumerable<Variable>, IFormula) QuantifierRest()
        {
            Check(LBrack);
            Check(Token.Kind.Variable);
            List<Variable> quantifiedVariables = [new(t.Name)];
            while (sym == Comma)
            {
                Scan();
                Check(Token.Kind.Variable);
                Variable v = new(t.Name);
                quantifiedVariables.Add(v);
            }
            Check(RBrack);
            Check(Colon);
            IFormula inner = Unary();
            return (quantifiedVariables, inner);
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
                throw new Exception($"Invalid token '{la.GetStringRepresentationOfKind()}' at line {la.Line}, column {la.Column}. Token '{expected}' is expected.");
            }
        }
    }
}
