using eqprenex.Language;

namespace eqprenex.ExponentialPrenexing
{
    public class ExpVariableGenerator
    {
        private readonly HashSet<string> unav = new();

        /// <summary>
        /// contains the next free index for every variable stem
        /// </summary>
        private readonly Dictionary<string, int> indices = new();

        public ExpVariableGenerator()
        {
            
        }


        /// <summary>
        /// Generates the next fresh variable for a variable v. For a variable xi with (possibly empty) index i, it is the first of x1, x2, x3, ... that is fresh.
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        public Variable Next(Variable variable)
        {
            string stem = variable.Stem;

            if (!indices.ContainsKey(stem))
            {
                indices[stem] = 1;
            }
            int index = indices[stem];

            string name;
            do
            {
                name = stem + index;
                index++;
            } while (unav.Contains(name));
            indices[stem] = index;

            return new Variable(name);
        }

        public void AddUnavailableVariable(Variable v)
        {
            unav.Add(v.Name);
        }
    }
}
