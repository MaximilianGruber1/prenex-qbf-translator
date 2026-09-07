using System.Text;

namespace prenex_qbf_translator.Language.ToTextConverters
{
    public class FormulaToLatexConverter
    {
        public string Convert(IFormula formula, bool combineQuantifiers)
        {
            var sb = new StringBuilder();
            AppendRec(
                formula, 
                needsParentheses: false,
                combineQuantifiers,
                sb);
            return sb.ToString();
        }

        public void AppendRec(IFormula formula, bool needsParentheses, bool combineQuantifiers, StringBuilder sb)
        {
            if (needsParentheses)
                sb.Append('(');

            if (formula is Variable v)
            {
                string name = v.Name;

                sb.Append(name[0]);

                int i = 1;
                if (i < name.Length && name[i] != 'p' && name[i] != 'm')
                {
                    sb.Append("_{");
                    while (i < name.Length && name[i] != 'p' && name[i] != 'm')
                    {
                        sb.Append(name[i]);
                        i++;
                    }

                    sb.Append("}");
                }
                if (i < name.Length)
                {
                    sb.Append("^{");
                    while (i < name.Length)
                    {
                        if (name[i] == 'p')
                            sb.Append('+');
                        else if (name[i] == 'm')
                            sb.Append('-');
                        else
                            sb.Append(name[i]);
                        i++;
                    }

                    sb.Append('}');
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
                                sb);
                            sb.Append(", ");
                            q = e;
                        }

                        AppendRec(
                            q.Variable,
                            needsParentheses: false,
                            combineQuantifiers,
                            sb);
                        sb.Append("\\}");
                    }
                    else
                    {
                        AppendRec(
                            q.Variable,
                            needsParentheses: false,
                            combineQuantifiers,
                            sb);
                    }
                    sb.Append(' ');
                    AppendRec(
                        q.Inner,
                        needsParentheses: q.Inner is BinaryOperator,
                        combineQuantifiers,
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
                                sb);
                            sb.Append(", ");
                            q = e;
                        }

                        AppendRec(
                            q.Variable,
                            needsParentheses: false,
                            combineQuantifiers,
                            sb);
                        sb.Append("\\}");
                    }
                    else
                    {
                        AppendRec(
                            q.Variable,
                            needsParentheses: false,
                            combineQuantifiers,
                            sb);
                    }
                    sb.Append(' ');
                    AppendRec(
                        q.Inner,
                        needsParentheses: q.Inner is BinaryOperator,
                        combineQuantifiers,
                        sb);
                }
            }
            else if (formula is Not not)
            {
                sb.Append("\\neg ");
                AppendRec(not.Inner,
                    needsParentheses: not.Inner is BinaryOperator,
                    combineQuantifiers,
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
                    sb);
                sb.Append(' ')
                    .Append(symb)
                    .Append(' ');
                AppendRec(
                    b.Right, 
                    rightNeedsParentheses, 
                    combineQuantifiers,
                    sb);
            }

            if (needsParentheses)
                sb.Append(')');
        }
    }
}
