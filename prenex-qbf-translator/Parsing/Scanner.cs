using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection.PortableExecutable;
using System.Runtime.CompilerServices;
using System.Text;
using static prenex_qbf_translator.Parsing.Token.Kind;

namespace prenex_qbf_translator.Parsing
{
    public class Scanner
    {
        /// <summary>
        /// Input data to read from
        /// </summary>
        private readonly Reader reader;

        /// <summary>
        /// Lookahead character
        /// </summary>
        private char ch;

        /// <summary>
        /// Current line in input stream
        /// </summary>
        private int line;

        /// <summary>
        /// Current column in input stream
        /// </summary>
        private int col;

        private const char EOF = '\0';


        public Scanner(string s)
        {
            reader = new Reader(s);
            line = 1;
            col = 0;
            NextCh();
        }

        public Token Next()
        {
            while (char.IsWhiteSpace(ch))
            {
                NextCh();
            }

            Token t = new(line, col);

            if (CanOccurAtStartOfVariable(ch) )
            {
                string name = ReadVariable();
                t.Kind_ = Variable;
                t.Name = name;
            }

            switch (ch)
            {
                case '<':
                    NextCh();
                    if (ch == '-')
                    {
                        NextCh();
                    }
                    else
                    {
                        throw new Exception(GetExceptionMessagePrefix(line, col) + $"expected '-' after '<'");
                    }
                    if (ch == '>')
                    {
                        NextCh();
                        t.Kind_ = Equiv;
                    }
                    else
                    {
                        t.Kind_ = IsImpliedBy;
                    }
                    break;
                case '-':
                    NextCh();
                    if (ch == '>')
                    {
                        t.Kind_ = Implies;
                        NextCh();
                    }
                    else
                    {
                        t.Kind_ = Not;
                    }
                    break;
                case '|':
                case '/':
                    t.Kind_ = Or;
                    NextCh();
                    break;
                case '&':
                    t.Kind_ = And;
                    NextCh();
                    break;
                case '!':
                    t.Kind_ = Not;
                    NextCh();
                    break;
                case '(':
                    t.Kind_ = LPar;
                    NextCh();
                    break;
                case ')':
                    t.Kind_ = RPar;
                    NextCh();
                    break;
                case '#':
                    t.Kind_ = Forall;
                    NextCh();
                    break;
                case '?':
                    t.Kind_ = Exists;
                    NextCh();
                    break;
                case EOF:
                    t.Kind_ = Eof;
                    break;
                default:
                    throw new Exception(GetExceptionMessagePrefix(line, col) + $"invalid character '{ch}'");
            }

            return t;
        }

        private void NextCh()
        {
            if (reader.HasNext())
            {
                ch = reader.Next();
                if (ch == '\n')
                {
                    line++;
                    col = 0;
                }
                else
                {
                    col++;
                }
            }
            else
            {
                ch = EOF;
            }
        }


        private string GetExceptionMessagePrefix(int line, int col)
        {
            return $"{line}:{col}: scan error: ";
        }

        private string ReadVariable()
        {
            var sb = new StringBuilder();
            do
            {
                sb.Append(ch);
                NextCh();
            } while (CanOccurInVariable(ch));
            var name = sb.ToString();
            if (name.EndsWith('-'))
            {
                throw new Exception($"{line}:{col}: scan error: variable '{name}' ends with '-'");
            }
            return sb.ToString();
        }

        private bool CanOccurInVariable(char c) =>
            (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') ||
            char.IsDigit(ch) ||
            ch == '-' || ch == '_' || ch == '.' || ch == '[' || ch == ']' ||
            ch == '$' || ch == '@';

        private bool CanOccurAtStartOfVariable(char c) => CanOccurInVariable(c) && c != '-';
    }
}
