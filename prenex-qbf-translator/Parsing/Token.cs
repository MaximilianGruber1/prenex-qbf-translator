namespace prenex_qbf_translator.Parsing
{
    public class Token
    {
        public enum Kind
        {
            Equiv,
            Implies,
            IsImpliedBy,
            Or,
            And,
            Not,
            Variable,
            LPar,
            RPar,
            Forall,
            Exists,
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
                Kind.Equiv => "<->",
                Kind.Implies => "->",
                Kind.IsImpliedBy => "<-",
                Kind.Or => "|",
                Kind.And => "&",
                Kind.Not => "!",
                Kind.Variable => "Variable",
                Kind.LPar => "(",
                Kind.RPar => ")",
                Kind.Forall => "#",
                Kind.Exists => "?",
                Kind.Eof => "EOF"
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


    }
}
