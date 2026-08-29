using prenex_qbf_translator.Language;

namespace prenex_qbf_translator.Translator
{
    /// <summary>
    /// Generates t_forall(phi), t_exists(phi), P(phi), and N(phi) for a given formula phi according to Definition 5. 
    /// </summary>
    public class SmallTGenerator
    {
        private readonly OutermostQuantifierDecomposer decomposer = new();


        public Result GenerateSmallT(IFormula formula, bool isForall, FreshVariableGenerator varGenerator)
        {
            ArgumentNullException.ThrowIfNull(formula);
            ArgumentNullException.ThrowIfNull(varGenerator);

            var decomp = decomposer.Decompose(formula, varGenerator);

            if (decomp.Substitution.Count == 0)
            {
                return new Result(formula, [], []); // all three big parentheses of Definition 2 (t terms) lines 1 and 2 are empty, so just return the formula itself
            }

            var groups = ConstructGroups(decomp.Substitution, varGenerator);

            IEnumerable<IFormula> parenthesis1 = GetParenthesis1(groups);
            IEnumerable<IFormula> parentheses2and3 = GetParentheses2And3(groups);

            IFormula t;
            if (isForall)
            {
                IFormula[] subs = [.. parenthesis1, .. parentheses2and3, decomp.Beta]; // the two parentheses lists have at least one element each, so 'subs' has at least two elements
                t = new And(subs[0], subs[1], subs[2..]);
            }
            else // exists
            {
                IFormula[] subs = [.. parenthesis1, .. parentheses2and3];
                t = new Implies(
                        new And(subs[0], subs[1], subs[2..]), // same here
                        decomp.Beta
                    );
            }

            var p = groups.SelectMany(g => g.XPlus).Concat(groups.Select(g => g.P)).ToList();
            var n = groups.SelectMany(g => g.XMinus).ToList();

            return new Result(t, p, n);
        }

        private IEnumerable<Group> ConstructGroups(Substitution unnamedSub, FreshVariableGenerator variableGenerator)
        {
            List<Group> groups = [];

            foreach (var (from, to) in unnamedSub.Mappings)
            {
                var q = (Quantifier)to;
                Group group = new(
                
                    p: from,
                    isForall: q is Forall,
                    quantifiedVars: GetQuantifiedVariablesForGroup(q),
                    phi: GetPhiForGroup(q),
                    xPlus: [],
                    xMinus: []
                );
                foreach (Variable v in group.QuantifiedVariables)
                {
                    var args = variableGenerator.NextPositiveAndNegative(v);
                    group.XPlus.Add(args.P);
                    group.XMinus.Add(args.N);
                }

                groups.Add(group);
            }
            return groups;
        }


        private IEnumerable<IFormula> GetParenthesis1(IEnumerable<Group> groups)
        {
            return groups.Select(GetParenthesis1Part);
        }

        private IFormula GetParenthesis1Part(Group group)
        {
            Substitution sigma = new();
            for (int i = 0; i < group.QuantifiedVariables.Count; i++)
            {
                sigma.Add(group.QuantifiedVariables[i], group.XPlus[i]);
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
            for (int i = 0; i < group.QuantifiedVariables.Count; i++)
            {
                equivalences.Add(new Equivalent(
                    group.XPlus[i],
                        group.XMinus[i]
                ));
            }

            IFormula right = equivalences.Count() == 1 ? // always at least 1
                equivalences[0] :
                new And(equivalences[0], equivalences[1], equivalences[2..].ToArray());
            if (group.IsForall)
            {
                return
                    new Implies(
                        group.P,
                        right
                    );
            }
            else
            {
                return
                    new Implies(
                        new Not(group.P),
                        right
                    );
            }
        }

        /// <summary>
        /// Returns a list of the quantified variables of consecutive quantifiers of the same type, e.g. #a#b#c?d?e#f some_formula --> [a,b,c].
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        private List<Variable> GetQuantifiedVariablesForGroup(Quantifier q)
        {
            List<Variable> vars = [q.Variable];

            if (q is Exists)
            {
                while (q.Inner is Exists e)
                {
                    vars.Add(e.Variable);
                    q = e;
                }

            }
            else
            {
                while (q.Inner is Forall f)
                {
                    vars.Add(f.Variable);
                    q = f;
                }
            }

            return vars;
        }

        /// <summary>
        /// Returns the formula inside consecutive quantifiers of the same type, e.g. #a#b#c?d?e#f some_formula --> ?d?e#f some_formula
        /// </summary>
        /// <param name="q"></param>
        /// <returns></returns>
        private IFormula GetPhiForGroup(Quantifier q)
        {
            if (q is Exists e)
            {
                while (q.Inner is Exists ee)
                {
                    q = ee;
                }
            }
            else if (q is Forall f)
            {
                while (q.Inner is Forall ff)
                {
                    q = ff;
                }
            }
            return q.Inner;
        }


        /// <summary>
        /// Represents a mapping (p_i / Q_i X_i phi_i) of the unnamed substitution of Fact 4 combined with the corresponding sigma_i in Definition 2.
        /// </summary>
        private class Group(Variable p, bool isForall, List<Variable> quantifiedVars, IFormula phi, List<Variable> xPlus, List<Variable> xMinus)
        {
            public Variable P { get; private set; } = p;
            public bool IsForall { get; private set; } = isForall;
            public List<Variable> QuantifiedVariables { get; private set; } = quantifiedVars;
            public IFormula Phi { get; private set; } = phi;
            public List<Variable> XPlus { get; private set; } = xPlus;
            public List<Variable> XMinus { get; private set; } = xMinus;
        }

        public class Result(IFormula formula, IEnumerable<Variable> p, IEnumerable<Variable> n)
        {
            public IFormula Formula { get; set; } = formula;
            public IEnumerable<Variable> P { get; set; } = p;
            public IEnumerable<Variable> N { get; set; } = n;
        }
    }
}
