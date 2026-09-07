using prenex_qbf_translator.Language;
using System;
using System.Collections.Generic;
using System.Text;

namespace prenex_qbf_translator.FormulaGeneration
{
    public class VariableGenerator
    {
        private int index = 0;
        private char letter = 'a';

        /// <summary>
        /// Generates the variable sequence a, b, ..., z, A, B, ..., Z, a1, ... , Z1, a2, ..., Z2, ... 
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
