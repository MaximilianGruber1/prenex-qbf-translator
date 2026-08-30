using System;
using System.Collections.Generic;
using System.Text;

namespace prenex_qbf_translator.Language
{
    public interface IFormula
    {

    }

    public abstract class Formula : IFormula
    {
        public override string ToString()
        {
            return new FormulaToStringConverter().Convert(this);
        }
    }
}
