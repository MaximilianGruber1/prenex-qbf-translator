using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection.PortableExecutable;
using System.Text;
using static prenex_qbf_translator.Parser.Token.Kind;

namespace prenex_qbf_translator.Parser
{
    internal class Scanner
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
        /// Current column in input stream
        /// </summary>
        private int col;

        private const char EOF = '\0';


        public Scanner(string s)
        {
            reader = new Reader(s);
            col = 0;
            NextCh();
        }

        public Token Next()
        {
            while (char.IsWhiteSpace(ch))
            {
                NextCh();
            }

            Token t = new(col);

            switch (ch)
            {
                case '<':
                    t.Kind_ = Equiv;
                    NextCh();
                    if (ch == '=')
                    {
                        NextCh();
                    }
                    else
                    {
                        throw GetInvalidCharacterException(ch, col);
                    }
                    if (ch == '>')
                    {
                        NextCh();
                    }
                    else
                    {
                        throw GetInvalidCharacterException(ch, col);
                    }
                    break;
                case '|':
                    t.Kind_ = Or;
                    NextCh();
                    break;
                case '&':
                    t.Kind_ = And;
                    NextCh();
                    break;
                case '~':
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
                case '!':
                    t.Kind_ = Forall;
                    NextCh();
                    break;
                case '?':
                    t.Kind_ = Exists;
                    NextCh();
                    break;
                case ',':
                    t.Kind_ = Comma;
                    NextCh();
                    break;
                case ':':
                    t.Kind_ = Colon;
                    NextCh();
                    break;
                
                case EOF:
                    t.Kind_ = Eof;
                    break;
                default:
                    throw GetInvalidCharacterException(ch, col);
            }

            return t;
        }

        private void NextCh()
        {
            try
            {
                ch = reader.Next();
                col++;
            }
            catch (Exception)
            {
                ch = EOF;
            }
        }


        private Exception GetInvalidCharacterException(char c, int pos)
        {
            return new Exception($"Invalid character '{c}' at position {pos}");
        }

    }
}
