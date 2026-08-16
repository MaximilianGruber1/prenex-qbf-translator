using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection.PortableExecutable;
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

            switch (ch)
            {
                case 'A': case 'B': case 'C': case 'D': case 'E': case 'F': case 'G': case 'H': case 'I': case 'J': case 'K': case 'L': case 'M': case 'N': case 'O': case 'P': case 'Q': case 'R': case 'S': case 'T': case 'U': case 'V': case 'W': case 'X': case 'Y': case 'Z': case 'a': case 'b': case 'c': case 'd': case 'e': case 'f': case 'g': case 'h': case 'i': case 'j': case 'k': case 'l': case 'm': case 'n': case 'o': case 'p': case 'q': case 'r': case 's': case 't': case 'u': case 'v': case 'w': case 'x': case 'y': case 'z':
                    t.Kind_ = Variable;
                    t.Name = ReadWord();
                    break;
                case '$':
                    NextCh();
                    string word = ReadWord();
                    if (word == "true")
                    {
                        t.Kind_ = True;
                    }
                    else if (word == "false")
                    {
                        t.Kind_ = False;
                    }
                    else
                    {
                        throw new Exception($"Invalid truth constant '{word}' at line {line}, column {col}. '$true' or '$false' expected.");
                    }
                    break;
                case '<':
                    t.Kind_ = Equiv;
                    NextCh();
                    if (ch == '=')
                    {
                        NextCh();
                    }
                    else
                    {
                        throw GetInvalidCharacterException(ch, line, col);
                    }
                    if (ch == '>')
                    {
                        NextCh();
                    }
                    else
                    {
                        throw GetInvalidCharacterException(ch, line, col);
                    }
                    break;
                case '=':
                    t.Kind_ = Implies;
                    NextCh();
                    if (ch == '>')
                    {
                        NextCh();
                    }
                    else
                    {
                        throw GetInvalidCharacterException(ch, line, col);
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
                case '[':
                    t.Kind_ = LBrack;
                    NextCh();
                    break;
                case ']':
                    t.Kind_ = RBrack;
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
                    throw GetInvalidCharacterException(ch, line, col);
            }

            return t;
        }

        private void NextCh()
        {
            try
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
            catch (Exception)
            {
                ch = EOF;
            }
        }


        private Exception GetInvalidCharacterException(char c, int line, int col)
        {
            return new Exception($"Invalid character '{c}' at line {line}, column {col}.");
        }

        private string ReadWord()
        {
            var sb = new StringBuilder();
            do
            {
                sb.Append(ch);
                NextCh();
            } while (char.IsLetter(ch) || char.IsDigit(ch) || ch == '_');
            return sb.ToString();
        }
    }
}
