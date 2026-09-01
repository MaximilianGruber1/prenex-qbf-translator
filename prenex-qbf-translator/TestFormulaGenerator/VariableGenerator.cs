using prenex_qbf_translator.Language;
using System;
using System.Collections.Generic;
using System.Text;

namespace prenex_qbf_translator.TestFormulaGenerator
{
    public class VariableGenerator
    {
        private int index = 0;
        private char letter = 'a';

        /// <summary>
        /// Generates the variables "a", "b", ... , "z", "a1", ... , "z1", "a2", ... "z2", ...
        /// </summary>
        /// <returns></returns>
        public Variable Next()
        {
            string name = index == 0 ? 
                letter.ToString() : 
                (letter.ToString() + index);

            Increase();

            return new Variable(name);
        }


        private void Increase() 
        { 
            if (letter == 'z')
            {
                letter = 'A';
            }
            else if (letter == 'Z')
            {
                letter = 'a';
                index++;
            }
            else
            {
                letter++;
            }
        }
    }
}
