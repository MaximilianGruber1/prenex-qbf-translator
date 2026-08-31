using prenex_qbf_translator.Language;
using System.Text;


namespace prenex_qbf_translator.Language
{
    /// <summary>
    /// Efficiently converts a formula to a string.
    /// </summary>
    public class FormulaToStringConverter
    {
        /// <summary>
        /// Efficiently converts a formula to a string.
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        public string Convert(IFormula f)
        {
            var sb = new StringBuilder();
            AppendIterative(f, sb);
            return sb.ToString();
        }

        private void AppendIterative(IFormula root, StringBuilder sb)
        {
            var stack = new Stack<PrintOperation>();

            stack.Push(new PrintFormula(root, false));

            while (stack.Count > 0)
            {
                var operation = stack.Pop();

                switch (operation)
                {
                    case PrintText text:
                        sb.Append(text.Text);
                        break;

                    case PrintCloseParenthesis:
                        sb.Append(')');
                        break;

                    case PrintFormula(var f, var needsParentheses):
                        // Die schließende Klammer wird direkt beim
                        // Verarbeiten der Formel eingeplant.
                        if (needsParentheses)
                        {
                            sb.Append('(');
                            stack.Push(new PrintCloseParenthesis());
                        }

                        if (f is Variable v)
                        {
                            sb.Append(v.Name);
                        }
                        else if (f is Not n)
                        {
                            sb.Append('!');

                            bool innerNeedsParentheses =
                                n.Inner is BinaryOperator;

                            stack.Push(new PrintFormula(
                                n.Inner,
                                innerNeedsParentheses));
                        }
                        else if (f is BinaryOperator b)
                        {
                            GetBinaryOperatorInfo(
                                f,
                                b,
                                out string symb,
                                out bool leftNeedsParentheses,
                                out bool rightNeedsParentheses);

                            // Reverse order because Stack is LIFO.
                            stack.Push(new PrintFormula(
                                b.Right,
                                rightNeedsParentheses));

                            stack.Push(new PrintText($" {symb} "));

                            stack.Push(new PrintFormula(
                                b.Left,
                                leftNeedsParentheses));
                        }
                        else if (f is Quantifier q)
                        {
                            char symb = f is Exists ? '?' : '#';

                            sb.Append(symb)
                              .Append(q.Variable)
                              .Append(' ');

                            bool innerNeedsParentheses =
                                q.Inner is BinaryOperator;

                            stack.Push(new PrintFormula(
                                q.Inner,
                                innerNeedsParentheses));
                        }
                        else
                        {
                            throw new Exception("unknown formula type");
                        }

                        break;
                }
            }
        }

        private void GetBinaryOperatorInfo(
            IFormula f,
            BinaryOperator b,
            out string symb,
            out bool leftNeedsParentheses,
            out bool rightNeedsParentheses)
        {
            if (f is And)
            {
                bool NeedsParentheses(IFormula subf) =>
                    subf is Equivalent ||
                    subf is Implies ||
                    subf is IsImpliedBy ||
                    subf is Or;

                symb = "&";
                leftNeedsParentheses = NeedsParentheses(b.Left);
                rightNeedsParentheses = NeedsParentheses(b.Right);
            }
            else if (f is Or)
            {
                bool NeedsParentheses(IFormula subf) =>
                    subf is Equivalent ||
                    subf is Implies ||
                    subf is IsImpliedBy;

                symb = "|";
                leftNeedsParentheses = NeedsParentheses(b.Left);
                rightNeedsParentheses = NeedsParentheses(b.Right);
            }
            else if (f is Implies)
            {
                bool NeedsParentheses(IFormula subf) =>
                    subf is Equivalent ||
                    subf is Implies ||
                    subf is IsImpliedBy;

                symb = "->";
                leftNeedsParentheses = NeedsParentheses(b.Left);
                rightNeedsParentheses = NeedsParentheses(b.Right);
            }
            else if (f is IsImpliedBy)
            {
                bool NeedsParentheses(IFormula subf) =>
                    subf is Equivalent ||
                    subf is Implies ||
                    subf is IsImpliedBy;

                symb = "<-";
                leftNeedsParentheses = NeedsParentheses(b.Left);
                rightNeedsParentheses = NeedsParentheses(b.Right);
            }
            else if (f is Equivalent)
            {
                symb = "<->";
                leftNeedsParentheses = false;
                rightNeedsParentheses = false;
            }
            else
            {
                throw new Exception("unknown type");
            }
        }



        private abstract record PrintOperation;

        private record PrintFormula(
            IFormula Formula,
            bool Parentheses
        ) : PrintOperation;

        private record PrintText(string Text) : PrintOperation;

        private record PrintCloseParenthesis() : PrintOperation;
    }
    

}
