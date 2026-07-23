using Xunit;
using prenex_qbf_translator.Language;

namespace prenex_qbf_translator.Tests
{
    public class ConstantsTests
    {
        [Fact]
        public void TrueConstant_ToString()
        {
            var t = new TrueConstant();
            Assert.Equal("true", t.ToString());
        }

        [Fact]
        public void FalseConstant_ToString()
        {
            var f = new FalseConstant();
            Assert.Equal("false", f.ToString());
        }

        [Fact]
        public void True_Negated_IsFalse()
        {
            var t = new TrueConstant();
            Assert.IsType<FalseConstant>(t.Negated());
        }
    }
}
