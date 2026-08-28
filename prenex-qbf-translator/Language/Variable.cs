namespace prenex_qbf_translator.Language
{
    public class Variable : IFormula, IEquatable<Variable>
    {
        public string Name { get; }
        public Variable(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            if (name.Length < 1)
            {
                throw new ArgumentException("empty variable name");
            }
        }

        public HashSet<Variable> Variables()
        {
            return [this];
        }

        public HashSet<Variable> FreeVariables()
        {
            return [this];
        }

        public HashSet<Variable> BoundVariables()
        {
            return [];
        }


        public IFormula DeepCopy()
        {
            return this; // immutable, therefore fine
        }

        public bool IsBoolean()
        {
            return true;
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
