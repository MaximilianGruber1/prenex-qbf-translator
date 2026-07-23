namespace prenex_qbf_translator.Language
{
    public class Variable : IFormula, IEquatable<Variable>
    {
        public string Name { get; }
        public Variable(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
        public IFormula Negated()
        {
            return new Not(this);
        }


        public IEnumerable<Variable> Variables()
        {
            return [this];
        }

        public IEnumerable<Variable> FreeVariables()
        {
            return [this];
        }

        public int NBlocks()
        {
            return 0;
        }

        public int NQuantifiedVariables()
        {
            return 0;
        }

        public int Length()
        {
            return 1;
        }

        public int QuantifierDepth()
        {
            return 0;
        }

        public IFormula ApplySubstitution(Substitution substitution)
        {
            foreach (var kvp in substitution.Entries)
            {
                if (this.Equals(kvp.Key))
                {
                    return kvp.Value;
                }
            }

            return this;
        }




        public override string ToString()
        {
            return Name;
        }

        public bool Equals(Variable? other)
        {
            return other is not null && Name == other.Name;
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Variable);
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
