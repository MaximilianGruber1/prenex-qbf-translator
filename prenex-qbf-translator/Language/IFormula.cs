using System;
using System.Collections.Generic;
using System.Text;

namespace prenex_qbf_translator.Language
{
    public interface IFormula
    {
        /**
         * Returns the negation of the formula.
         * For example, if the formula is P(x), then the negation is ¬P(x).
         */
        IFormula Negated();

        /**
         * Returns the variables in the formula.
         * A variable is a symbol that can take on different values.
         * For example, in the formula ∀x ∃y P(x,y,z), the variables are x, y, and z.
         */
        IEnumerable<Variable> Variables();

        /**
         * Returns the free variables in the formula.
         * A variable is free if it is not bound by a quantifier.
         * For example, in the formula ∀x ∃y P(x,y,z), the variable z is free, while x and y are bound.
         */
        IEnumerable<Variable> FreeVariables();

        /**
         * Returns the number of blocks in the formula.
         * A block is a sequence of quantifiers of the same type (either all existential or all universal).
         * For example, the formula ∀x ∃y ∀z P(x,y,z) has 3 blocks: one universal block (∀x), one existential block (∃y), and one universal block (∀z).
         */
        int NBlocks();

        /**
         * Returns the number of quantified variables in the formula.
         * A quantified variable is a variable that is bound by a quantifier.
         * For example, in the formula ∀x ∃y P(x,y,z), the quantified variables are x and y, while z is free.
         */
        int NQuantifiedVariables();

        /**
         * Returns the length of the formula.
         * the number of logical and non-logical symbols
           occurring in φ, where propositional variables and logical operators both have length 1 and negations and parentheses do not count.
         */
        int Length();

        /**
         * Returns the depth of the formula.
         * The depth of a formula is the maximum number of nested logical operators in the formula.
         * For example, the formula ∀x ∃y P(x,y,z) has depth 2, while the formula ∀x ∃y ∀z P(x,y,z) has depth 3.
         */
        int QuantifierDepth();

        /**
         * Applies a substitution to the formula.
         * A substitution is a mapping from variables to formulas.
         * For example, if the formula is P(x) and the substitution is {x -> Q(y)}, then the result of applying the substitution is P(Q(y)).
         */
        IFormula ApplySubstitution(Substitution substitution);
        bool Equals(object obj);
    }
}
