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
            AppendRec(f, sb);
            return sb.ToString();
        }

        private void AppendRec(IFormula f, StringBuilder sb)
        {
            if (f is Variable v)
            {
                sb.Append(v.Name);
            }
            else if (f is Not n)
            {
                sb.Append('!');
                bool needsParentheses = n.Inner is BinaryOperator;
                AppendSubformula(n.Inner, needsParentheses, sb);
            }
            else if (f is BinaryOperator b)
            {
                string symb;
                bool leftNeedsParentheses;
                bool rightNeedsParentheses;
                if (f is And)
                {
                    bool NeedsParentheses(IFormula subf) => subf is Equivalent || subf is Implies || subf is IsImpliedBy || subf is Or;
                    symb = "&";
                    leftNeedsParentheses = NeedsParentheses(b.Left);
                    rightNeedsParentheses = NeedsParentheses(b.Right);
                }
                else if (f is Or)
                {
                    bool NeedsParentheses(IFormula subf) => subf is Equivalent || subf is Implies || subf is IsImpliedBy;
                    symb = "|";
                    leftNeedsParentheses = NeedsParentheses(b.Left);
                    rightNeedsParentheses = NeedsParentheses(b.Right);
                }
                else if (f is Implies)
                {
                    bool NeedsParentheses(IFormula subf) => subf is Equivalent || subf is Implies || subf is IsImpliedBy;
                    symb = "->";
                    leftNeedsParentheses = NeedsParentheses(b.Left);
                    rightNeedsParentheses = NeedsParentheses(b.Right);
                }
                else if (f is IsImpliedBy)
                {
                    bool NeedsParentheses(IFormula subf) => subf is Equivalent || subf is Implies || subf is IsImpliedBy;
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

                AppendSubformula(b.Left, leftNeedsParentheses, sb);
                sb.Append(' ')
                    .Append(symb)
                    .Append(' ');
                AppendSubformula(b.Right, rightNeedsParentheses, sb);
            }
            else if (f is Quantifier q)
            {
                char symb = f is Exists ? '?' : '#';
                sb.Append(symb)
                    .Append(q.Variable)
                    .Append(' ');
                bool needsParentheses = q.Inner is BinaryOperator;
                AppendSubformula(q.Inner, needsParentheses, sb);
            }
        }

        private void AppendSubformula(IFormula subf, bool needsParentheses, StringBuilder sb)
        {
            if (needsParentheses)
                sb.Append('(');
            AppendRec(subf, sb);
            if (needsParentheses)
                sb.Append(')');
        }

    }
    

}
