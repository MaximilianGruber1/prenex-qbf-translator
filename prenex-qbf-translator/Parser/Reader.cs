using System;
using System.Collections.Generic;
using System.Text;

namespace prenex_qbf_translator.Parser
{
    internal class Reader
    {
        private readonly string s;
        private int index = 0;

        public Reader(string s)
        {
            this.s = s;
            index = 0;
        }

        private bool HasNext()
        {
            return index < s.Length;
        }

        /// <summary>
        /// returns one char at a time of a given string. If the End of the string is reached, an exception is thrown.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public char Next()
        {
            if (!HasNext())
            {
                throw new Exception("End of string reached");
            }
            return s[index++];
        }
    }
}
