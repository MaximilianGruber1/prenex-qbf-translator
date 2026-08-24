using System;
using System.Collections.Generic;
using System.Text;

namespace prenex_qbf_translator.Parsing
{
    /// <summary>
    /// Returns one character at a time of a given string.
    /// </summary>
    public class Reader
    {
        private readonly string s;
        private int index = 0;

        public Reader(string s)
        {
            this.s = s;
            index = 0;
        }
        
        /// <summary>
        /// Returns whether the end of the string has been reached.
        /// </summary>
        /// <returns></returns>
        public bool HasNext()
        {
            return index < s.Length;
        }

        /// <summary>
        /// Returns the next character. If the end is reached, an exception is thrown.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public char Next()
        {
            if (!HasNext())
            {
                throw new Exception("end of string reached");
            }
            return s[index++];
        }
    }
}
