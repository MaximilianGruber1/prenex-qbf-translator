using prenex_qbf_translator.Language;

namespace prenex_qbf_translator.Translator
{
    /// <summary>
    /// Generates t_forall(phi), t_exists(phi), N(phi), and P(phi) for a given formula phi according to Definition 5. 
    /// </summary>
    public class SmallTGenerator
    {
        private readonly FreshVariableGenerator variableGenerator = new();
        private readonly OutermostQuantifierDecomposer decomposer = new();


        /// <summary>
        /// Generates t_forall(phi), N(phi), and P(phi) for a given formula phi according to Definition 5.
        /// </summary>
        /// <param name="formula"></param>
        /// <param name="unavailableVariables"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IFormula GenerateSmallTForall(IFormula formula, IEnumerable<Variable>? unavailableVariables = null)
        {
            unavailableVariables ??= [];

            return GenerateSmallT(formula, true, unavailableVariables);
        }


        /// <summary>
        /// Generates t_exists(phi), N(phi), and P(phi) for a given formula phi according to Definition 5.
        /// </summary>
        /// <param name="formula"></param>
        /// <param name="unavailableVariables0"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IFormula GenerateSmallTExists(IFormula formula, IEnumerable<Variable>? unavailableVariables = null)
        {
            unavailableVariables ??= [];

            return GenerateSmallT(formula, false, unavailableVariables);
        }

        public IEnumerable<Variable> GetP(IFormula formula, IEnumerable<Variable>? unavailableVariables = null)
        {
            unavailableVariables ??= [];

            if (formula.IsBoolean())
            {
                return [];
            }

            var decomp = decomposer.Decompose(formula, unavailableVariables);
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

            var decomp = decomposer.Decompose(formula, unavailableVariables);
            var unav = new List<Variable>(unavailableVariables);
            unav.AddRange(formula.Variables());
            unav = unav.Distinct().ToList();
            var groups = ConstructGroups(decomp.Substitution, unav);
            return groups.SelectMany(g => g.XMinus);
        }


        /// <summary>
        /// Generates t_forall(phi) or t_exists(phi), N(phi), and P(phi) for a given formula phi according to Definition 5.
        /// </summary>
        /// <param name="formula"></param>
        /// <param name="isForall"></param>
        /// <param name="unavailableVariables"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public IFormula GenerateSmallT(IFormula formula, bool isForall, IEnumerable<Variable> unavailableVariables)
        {
            if (formula.IsBoolean())
            {
                return formula; // all 3 parentheses are empty, so we just return the formula itself
            }
            var unav = new List<Variable>(unavailableVariables);
            unav.AddRange(formula.Variables());
            unav = unav.Distinct().ToList();

            var decomp = decomposer.Decompose(formula, unavailableVariables);
            var groups = ConstructGroups(decomp.Substitution, unav);

            IEnumerable<IFormula> parenthesis1 = GetParenthesis1(groups);
            IEnumerable<IFormula> parentheses2and3 = GetParentheses2And3(groups);
            if (isForall)
            {
                return new And([.. parenthesis1, .. parentheses2and3, decomp.Beta]);
            }
            else // exists
            {
                return new Implies(
                        new And([.. parenthesis1, .. parentheses2and3]),
                        decomp.Beta
                    );
            }
        }

        private IEnumerable<Group> ConstructGroups(Substitution unnamedSub, IEnumerable<Variable> unavailableVariables)
        {
            List<Variable> unav = unavailableVariables.ToList();
            List<Group> groups = [];

            foreach (var entry in unnamedSub.Dictionary)
            {
                var from = entry.Key;
                var to = (IQuantifier)entry.Value;

                Group group = new();
                group.P = from;
                group.IsForall = entry.Value is Forall;
                group.BoundVariables = to.BoundVariables.ToList();
                group.Phi = to.Inner;
                group.XPlus = [];
                group.XMinus = [];
                foreach (Variable v in group.BoundVariables)
                {
                    var args = variableGenerator.GetPositiveAndNegative(v, unav);
                    group.XPlus.Add(args.Plus);
                    group.XMinus.Add(args.Minus);
                    unav.Add(args.Plus);
                    unav.Add(args.Minus);
                }

                groups.Add(group);
            }
            return groups;
        }


        // of course assuming all list have equal length, and that the lists are ordered in the same way as the entries of the substitution
        private IEnumerable<IFormula> GetParenthesis1(IEnumerable<Group> groups)
        {
            return groups.Select(GetParenthesis1Part);
        }

        private IFormula GetParenthesis1Part(Group group)
        {
            Dictionary<Variable, IFormula> sigmaDic = new();
            for (int i = 0; i < group.BoundVariables.Count; i++)
            {
                sigmaDic[group.BoundVariables[i]] = group.XPlus[i];
            }
            Substitution sigma = new(sigmaDic);

            return new Equivalent([
                group.P,
                group.Phi.ApplySubstitution(sigma)
                ]);
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
                equivalences.Add(new Equivalent([
                    group.XPlus[i],
                        group.XMinus[i]
                ]));
            }

            IFormula right = equivalences.Count() == 1 ?
                equivalences[0] :
                new And(equivalences);
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
