using System.Text;

namespace prenex_qbf_translator.Language
{
    public class Substitution
    {
        public Dictionary<Variable, IFormula> Dictionary { get; }
        
        public Substitution(Dictionary<Variable, IFormula> substitutions)
        {
            if (substitutions == null) throw new ArgumentNullException(nameof(substitutions));
            foreach (var val in substitutions.Values)
            {
                if (val == null)
                {
                    throw new ArgumentNullException("Substitution cannot contain null keys or values.");
                }
            }
            Dictionary = substitutions;
        }

        public override string ToString()
        {
            return "{" + string.Join(", ", Dictionary.Select(kvp => $"{kvp.Key} -> {kvp.Value}")) + "}";
        }
    }
}
