using System.Text;

namespace prenex_qbf_translator.Language.ToTextConverters
{
    public class FormulaToLatexConverter
    {
        public string Convert(IFormula formula, bool combineQuantifiers, bool formatVariables)
        {
            var sb = new StringBuilder();
            AppendRec(
                formula, 
                needsParentheses: false,
                combineQuantifiers,
                formatVariables,
                sb);
            return sb.ToString();
        }

        public void AppendRec(IFormula formula, bool needsParentheses, bool combineQuantifiers, bool formatVariables, StringBuilder sb)
        {
            if (needsParentheses)
                sb.Append('(');

            if (formula is Variable v)
            {
                string name = v.Name;

                if (formatVariables)
                {
                    string superscript;
                    Variable baseVar;
                    if (name.Length > 1 && name.EndsWith('p'))
                    {
                        superscript = "+";
                        baseVar = new Variable(name[..(name.Length - 1)]);
                    }
                    else if (name.Length > 1 && name.EndsWith('m'))
                    {
                        superscript = "-";
                        baseVar = new Variable(name[..(name.Length - 1)]);
                    }
                    else
                    {
                        superscript = "";
                        baseVar = v;
                    }

                    string stem = baseVar.Stem;
                    string subscript = baseVar.Index;

                    sb.Append(stem);
                    if (subscript.Length > 0)
                    {
                        sb.Append('_')
                            .Append(subscript);
                    }
                    if (superscript.Length > 0)
                    {
                        sb.Append('^')
                            .Append(superscript);
                    }
                }
                else
                {
                    sb.Append(name);
                }
            }
            else if (formula is Quantifier q)
            {
                if (q is Exists)
                {
                    sb.Append("\\exists ");

                    if (combineQuantifiers && q.Inner is Exists)
                    {
                        sb.Append("\\{");

                        while (q.Inner is Exists e)
                        {
                            AppendRec(
                                q.Variable,
                                needsParentheses: false,
                                combineQuantifiers,
                                formatVariables,
                                sb);
                            sb.Append(", ");
                            q = e;
                        }

                        AppendRec(
                            q.Variable,
                            needsParentheses: false,
                            combineQuantifiers,
                            formatVariables,
                            sb);
                        sb.Append("\\}");
                    }
                    else
                    {
                        AppendRec(
                            q.Variable,
                            needsParentheses: false,
                            combineQuantifiers,
                            formatVariables,
                            sb);
                    }
                    sb.Append(' ');
                    AppendRec(
                        q.Inner,
                        needsParentheses: q.Inner is BinaryOperator,
                        combineQuantifiers,
                        formatVariables,
                        sb);
                }
                else // forall
                {
                    sb.Append("\\forall ");

                    if (combineQuantifiers && q.Inner is Forall)
                    {
                        sb.Append("\\{");

                        while (q.Inner is Forall e)
                        {
                            AppendRec(
                                q.Variable,
                                needsParentheses: false,
                                combineQuantifiers,
                                formatVariables,
                                sb);
                            sb.Append(", ");
                            q = e;
                        }

                        AppendRec(
                            q.Variable,
                            needsParentheses: false,
                            combineQuantifiers,
                            formatVariables,
                            sb);
                        sb.Append("\\}");
                    }
                    else
                    {
                        AppendRec(
                            q.Variable,
                            needsParentheses: false,
                            combineQuantifiers,
                            formatVariables,
                            sb);
                    }
                    sb.Append(' ');
                    AppendRec(
                        q.Inner,
                        needsParentheses: q.Inner is BinaryOperator,
                        combineQuantifiers,
                        formatVariables,
                        sb);
                }
            }
            else if (formula is Not not)
            {
                sb.Append("\\neg ");
                AppendRec(not.Inner,
                    needsParentheses: not.Inner is BinaryOperator,
                    combineQuantifiers,
                    formatVariables,
                    sb);
            }
            else if (formula is BinaryOperator b)
            {
                string symb;
                bool leftNeedsParentheses;
                bool rightNeedsParentheses;

                if (b is And)
                {
                    bool NeedsParentheses(IFormula subf) =>
                        subf is Equivalent ||
                        subf is Implies ||
                        subf is IsImpliedBy ||
                        subf is Or;

                    symb = "\\land";
                    leftNeedsParentheses = NeedsParentheses(b.Left);
                    rightNeedsParentheses = NeedsParentheses(b.Right);
                }
                else if (b is Or)
                {
                    bool NeedsParentheses(IFormula subf) =>
                    subf is Equivalent ||
                    subf is Implies ||
                    subf is IsImpliedBy;

                    symb = "\\lor";
                    leftNeedsParentheses = NeedsParentheses(b.Left);
                    rightNeedsParentheses = NeedsParentheses(b.Right);
                }
                else if (b is Implies)
                {
                    bool NeedsParentheses(IFormula subf) =>
                        subf is Equivalent ||
                        subf is Implies ||
                        subf is IsImpliedBy;

                    symb = "\\rightarrow";
                    leftNeedsParentheses = NeedsParentheses(b.Left);
                    rightNeedsParentheses = NeedsParentheses(b.Right);
                }
                else if (b is IsImpliedBy)
                {
                    bool NeedsParentheses(IFormula subf) =>
                        subf is Equivalent ||
                        subf is Implies ||
                        subf is IsImpliedBy;

                    symb = "\\leftarrow";
                    leftNeedsParentheses = NeedsParentheses(b.Left);
                    rightNeedsParentheses = NeedsParentheses(b.Right);
                }
                else if (b is Equivalent)
                {
                    symb = "\\leftrightarrow";
                    leftNeedsParentheses = false;
                    rightNeedsParentheses = false;
                }
                else
                {
                    throw new Exception("unknown type");
                }

                AppendRec(
                    b.Left, 
                    leftNeedsParentheses, 
                    combineQuantifiers,
                    formatVariables,
                    sb);
                sb.Append(' ')
                    .Append(symb)
                    .Append(' ');
                AppendRec(
                    b.Right, 
                    rightNeedsParentheses, 
                    combineQuantifiers,
                    formatVariables,
                    sb);
            }

            if (needsParentheses)
                sb.Append(')');
        }
    }
}
