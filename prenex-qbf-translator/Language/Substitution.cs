namespace prenex_qbf_translator.Language
{
    public class Substitution
    {
        public Dictionary<Variable, IFormula> Entries { get; }
        
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
            Entries = substitutions;
        }
    }
}
