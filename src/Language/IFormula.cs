using eqprenex.Language.ToTextConverters;
using System;
using System.Collections.Generic;
using System.Text;

namespace eqprenex.Language
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
