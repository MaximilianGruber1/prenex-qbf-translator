
using Xunit;
using prenex_qbf_translator.Language;

namespace prenex_qbf_translator.Tests
{
    public class VariableTest
    {
        [Fact]
        public void SingleCharacter_NoIndex()
        {
            var v = new Variable("a");
            Assert.Equal("a", v.Name);
            Assert.Equal("a", v.Stem);
            Assert.Equal("", v.Index);
        }

        [Fact]
        public void MultiCharacter_NoIndex()
        {
            var v = new Variable("abc");
            Assert.Equal("abc", v.Name);
            Assert.Equal("abc", v.Stem);
            Assert.Equal("", v.Index);
        }

        [Fact]
        public void SingleCharacter_SingleIndex()
        {
            var v = new Variable("a3");
            Assert.Equal("a3", v.Name);
            Assert.Equal("a", v.Stem);
            Assert.Equal("3", v.Index);
        }

        [Fact]
        public void MultiCharacter_SingleIndex()
        {
            var v = new Variable("abc9");
            Assert.Equal("abc9", v.Name);
            Assert.Equal("abc", v.Stem);
            Assert.Equal("9", v.Index);
        }

        [Fact]
        public void SingleCharacter_MultiIndex()
        {
            var v = new Variable("a123");
            Assert.Equal("a123", v.Name);
            Assert.Equal("a", v.Stem);
            Assert.Equal("123", v.Index);
        }

        [Fact]
        public void MultiCharacter_MultiIndex()
        {
            var v = new Variable("abc123");
            Assert.Equal("abc123", v.Name);
            Assert.Equal("abc", v.Stem);
            Assert.Equal("123", v.Index);
        }

        [Fact]
        public void IndexZero()
        {
            var v = new Variable("a0");
            Assert.Equal("a0", v.Name);
            Assert.Equal("a", v.Stem);
            Assert.Equal("0", v.Index);
        }

        [Fact]
        public void IndexStartsWith0()
        {
            var v = new Variable("a012");
            Assert.Equal("a012", v.Name);
            Assert.Equal("a", v.Stem);
            Assert.Equal("012", v.Index);
        }

        [Fact]
        public void DigitsInStem()
        {
            var v = new Variable("a0b34c789");
            Assert.Equal("a0b34c789", v.Name);
            Assert.Equal("a0b34c", v.Stem);
            Assert.Equal("789", v.Index);
        }

        [Fact]
        public void OnlySingleDigit()
        {
            var v = new Variable("1");
            Assert.Equal("1", v.Name);
            Assert.Equal("1", v.Stem);
            Assert.Equal("", v.Index);
        }

        [Fact]
        public void OnlyMultipleDigits()
        {
            var v = new Variable("012345");
            Assert.Equal("012345", v.Name);
            Assert.Equal("0", v.Stem);
            Assert.Equal("12345", v.Index);
        }
    }
}
