namespace prenex_qbf_translator.Parser
{
    public class Token
    {
        public enum Kind
        {
            None,
            Equiv,
            Or,
            And,
            Not,
            True,
            False,
            Variable,
            LPar,
            RPar,
            LBrack,
            RBrack,
            Forall,
            Exists,
            Comma,
            Colon,
            Eof
        }

        public Kind Kind_ { get; set; }

        public int Line { get; set; }

        public int Column { get; set; }

        /// <summary>
        /// Piece type (for pieces)
        /// </summary>
        public string Name { get; set; }


        public Token(int line, int col)
        {
            Line = line;
            Column = col;
        }

        public Token(int line, int col, Kind kind) : this(line, col)
        {
            Kind_ = kind;
        }

        private string GetStringRepresentation(Kind kind)
        {
            return kind switch
            {
                Kind.None => "none",
                Kind.Equiv => "<=>",
                Kind.Or => "|",
                Kind.And => "&",
                Kind.Not => "~",
                Kind.True => "true",
                Kind.False => "false",
                Kind.Variable => "variable",
                Kind.LPar => "(",
                Kind.RPar => ")",
                Kind.Forall => "forall",
                Kind.Exists => "exists",
                Kind.Comma => ",",
                Kind.Colon => ":"
            };
        }

        public override string ToString()
        {
            string result = $"Column {Column}, Kind {Kind_} (\"{GetStringRepresentation(Kind_)}\")";
            if (Kind_ == Kind.Variable)
            {
                result += " " + Name;
            }
            return result;
        }

        public string GetStringRepresentationOfKind()
        {
            return GetStringRepresentation(Kind_);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            Token other = (Token)obj;
            return Kind_ == other.Kind_ && Name == other.Name && Column == other.Column;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Kind_, Name, Column);
        }

    }
}
