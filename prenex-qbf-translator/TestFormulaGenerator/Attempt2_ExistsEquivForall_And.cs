using prenex_qbf_translator.Language;

namespace prenex_qbf_translator.TestFormulaGenerator
{
    public class Attempt2_ExistsEquivForall_And : IFormulaGenerator
    {
        public IFormula GenerateFormula(int size)
        {
            if (size == 0)
                throw new ArgumentException("size must be > 0");

            var g = new VariableGenerator();

            return GenerateFormulaRecursive(size, [], g);
        }

        public IFormula GenerateFormulaRecursive(int size, Stack<Variable> varsInBranch, VariableGenerator gen)
        {
            if (size == 0)
            {
                if (varsInBranch.Count == 1)
                {
                    return varsInBranch.Peek();
                }
                return new And(varsInBranch.Reverse().ToArray());
            }
            else
            {
                var leftVar = gen.Next();
                varsInBranch.Push(leftVar);
                IFormula leftFormula = GenerateFormulaRecursive(size - 1, varsInBranch, gen);
                varsInBranch.Pop();

                var rightVar = gen.Next();
                varsInBranch.Push(rightVar);
                IFormula rightFormula = GenerateFormulaRecursive(size - 1, varsInBranch, gen);
                varsInBranch.Pop();

                leftFormula = new Exists(leftVar, leftFormula);
                rightFormula = new Forall(rightVar, rightFormula);

                return new Equivalent(leftFormula, rightFormula);
            }
        }
    }
}
