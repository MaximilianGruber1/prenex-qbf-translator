using prenex_qbf_translator.Language;

namespace prenex_qbf_translator.Translator
{
    /// <summary>
    /// Generates t_forall(phi), t_exists(phi), P(phi), and N(phi) for a given formula phi according to Definition 5. 
    /// </summary>
    public class SmallTGenerator
    {
        private readonly FreshVariableGenerator variableGenerator = new();
        private readonly OutermostQuantifierDecomposer decomposer = new();


        /// <summary>
        /// Generates t_forall(phi), N(phi), and P(phi) for a formula phi according to Definition 2.
        /// </summary>
        /// <param name="formula"></param>
        /// <param name="unavailableVariables"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IFormula GenerateSmallTForall(IFormula formula, IEnumerable<Variable>? unavailableVariables = null)
        {
            return GenerateSmallT(formula, true, unavailableVariables);
        }


        /// <summary>
        /// Generates t_exists(phi), N(phi), and P(phi) for a formula phi according to Definition 2.
        /// </summary>
        /// <param name="formula"></param>
        /// <param name="unavailableVariables0"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IFormula GenerateSmallTExists(IFormula formula, IEnumerable<Variable>? unavailableVariables = null)
        {
            return GenerateSmallT(formula, false, unavailableVariables);
        }

        public IEnumerable<Variable> GetP(IFormula formula, IEnumerable<Variable>? unavailableVariables = null)
        {
            unavailableVariables ??= [];

            if (formula.IsBoolean())
            {
                return [];
            }

            var decomp = decomposer.GetDecomposition(formula, unavailableVariables);
            var unav = new List<Variable>(unavailableVariables);
            unav.AddRange(formula.Variables());
            unav = unav.Distinct().ToList();
            var groups = ConstructGroups(decomp.Substitution, unav);
            return groups.SelectMany(g => g.XPlus).Concat(groups.Select(g => g.P));
        }

        public IEnumerable<Variable> GetN(IFormula formula, IEnumerable<Variable>? unavailableVariables = null)
        {
            unavailableVariables ??= [];

            if (formula.IsBoolean())
            {
                return [];
            }

            var decomp = decomposer.GetDecomposition(formula, unavailableVariables);
            var unav = new List<Variable>(unavailableVariables);
            unav.AddRange(formula.Variables());
            unav = unav.Distinct().ToList();
            var groups = ConstructGroups(decomp.Substitution, unav);
            return groups.SelectMany(g => g.XMinus);
        }


        public IFormula GenerateSmallT(IFormula formula, bool isForall, IEnumerable<Variable>? unavailableVariables = null)
        {
            unavailableVariables ??= [];

            if (formula.IsBoolean())
            {
                return formula; // all three big parentheses of Definition 2 (t terms) lines 1 and 2 are empty, so just return the formula itself
            }
            var unav = new List<Variable>(unavailableVariables);
            unav.AddRange(formula.Variables());
            unav = unav.Distinct().ToList();

            var decomp = decomposer.GetDecomposition(formula, unavailableVariables);
            var groups = ConstructGroups(decomp.Substitution, unav);

            IEnumerable<IFormula> parenthesis1 = GetParenthesis1(groups);
            IEnumerable<IFormula> parentheses2and3 = GetParentheses2And3(groups);
            if (isForall)
            {
                IFormula[] subs = [.. parenthesis1, .. parentheses2and3, decomp.Beta]; // the two parentheses lists have at least one element each
                return new And(subs[0], subs[1], subs[2..]);
            }
            else // exists
            {
                IFormula[] subs = [.. parenthesis1, .. parentheses2and3];
                return new Implies(
                        new And(subs[0], subs[1], subs[2..]), // same here
                        decomp.Beta
                    );
            }
        }

        private IEnumerable<Group> ConstructGroups(Substitution unnamedSub, IEnumerable<Variable> unavailableVariables)
        {
            List<Variable> unav = unavailableVariables.ToList();
            List<Group> groups = [];

            foreach (var (from, to) in unnamedSub.Mappings)
            {
                var q = (Quantifier)to;
                Group group = new();
                group.P = from;
                group.IsForall = q is Forall;
                group.BoundVariables = q.QuantifiedVariables.ToList();
                group.Phi = q.Inner;
                group.XPlus = [];
                group.XMinus = [];
                foreach (Variable v in group.BoundVariables)
                {
                    var args = variableGenerator.GetPositiveAndNegative(v, unav);
                    group.XPlus.Add(args.P);
                    group.XMinus.Add(args.N);
                    unav.Add(args.P);
                    unav.Add(args.N);
                }

                groups.Add(group);
            }
            return groups;
        }


        // of course assuming all lists have equal length, and that the lists are ordered in the same way as the entries of the substitution
        private IEnumerable<IFormula> GetParenthesis1(IEnumerable<Group> groups)
        {
            return groups.Select(GetParenthesis1Part);
        }

        private IFormula GetParenthesis1Part(Group group)
        {
            Substitution sigma = new();
            for (int i = 0; i < group.BoundVariables.Count; i++)
            {
                sigma.Add(group.BoundVariables[i], group.XPlus[i]);
            }

            return new Equivalent(
                group.P,
                sigma.ApplyTo(group.Phi)
                );
        }

        private IEnumerable<IFormula> GetParentheses2And3(IEnumerable<Group> groups)
        {
            return groups.Select(GetParentheses2And3Part);
        }

        private IFormula GetParentheses2And3Part(Group group)
        {
            List<IFormula> equivalences = [];
            for (int i = 0; i < group.BoundVariables.Count; i++)
            {
                equivalences.Add(new Equivalent(
                    group.XPlus[i],
                        group.XMinus[i]
                ));
            }

            IFormula right = equivalences.Count() == 1 ? // always at least 1
                equivalences[0] :
                new And(equivalences[0], equivalences[1], equivalences[2..].ToArray());
            if (!group.IsForall) // Exists
            {
                return
                    new Implies(
                        new Not(group.P),
                        right
                    );
            }
            else // Forall
            {
                return
                    new Implies(
                        group.P,
                        right
                    );
            }
        }



        private class Group
        {
            public Variable P { get; set; }
            public bool IsForall { get; set; }
            public List<Variable> BoundVariables { get; set; }
            public IFormula Phi { get; set; }
            public List<Variable> XPlus { get; set; }
            public List<Variable> XMinus { get; set; }
        }
    }
}
