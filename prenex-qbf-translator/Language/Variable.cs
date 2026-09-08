namespace prenex_qbf_translator.Language
{
    public class Variable : Formula, IEquatable<Variable>, IComparable<Variable>
    {
        /// <summary>
        /// The string the variable represents.0
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// The name without the index.
        /// </summary>
        public string Stem => stem;
        private readonly string stem;

        /// <summary>
        /// The trailing digit sequence. If the variable name consists of only digits, the first digit is the stem
        /// </summary>
        public string Index => index;
        private readonly string index;
        private readonly int indexInt; // for more efficient comparison

        private readonly record struct VariableParts(string Stem, string Index);

        public Variable(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));

            if (name.Length < 1)
            {
                throw new ArgumentException("empty variable name");
            }

            var parts = GetVariableParts(name);

            stem = parts.Stem;
            index = parts.Index;
            indexInt = index == "" ? -1 : int.Parse(index);
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

        public int CompareTo(Variable? other)
        {
            if (other is null)
                return 1;

            int result = string.Compare(
                stem,
                other.stem,
                StringComparison.Ordinal);

            if (result != 0)
                return result;

            result = index.CompareTo(other.index);

            if (result != 0)
                return result;

            // oo be consistent with Equals when e.g. a1 and a01 are compared.
            return string.Compare(
                Name,
                other.Name,
                StringComparison.Ordinal);
        }

        private static VariableParts GetVariableParts(string name)
        {
            int i = name.Length - 1;

            // Find the beginning of the trailing digit sequence. If name consists of only digits, the first digit is the stem.
            while (i >= 1 && char.IsDigit(name[i]))
            {
                i--;
            }

            string stem = name[..(i + 1)];
            string index = name[(i + 1)..];

            return new VariableParts(stem, index);
        }
    }
}