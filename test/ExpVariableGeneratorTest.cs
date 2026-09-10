using eqprenex.ExponentialPrenexing;
using eqprenex.Language;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace eqprenex.Tests
{
    public class ExpVariableGeneratorTest
    {
        private void TestNext(string name, string[] expectedNames)
        {
            TestNext(new ExpVariableGenerator(), name, expectedNames);
        }

        private void TestNext(ExpVariableGenerator gen, string name, string[] expectedNames)
        {
            for (int i = 0; i < expectedNames.Length; i++)
            {
                Variable actual = gen.Next(new Variable(name));

                Assert.Equal(expectedNames[i], actual.Name);
            }
        }


        [Fact]
        public void BasicSequence()
        {
            TestNext("a", ["a1", "a2", "a3", "a4", "a5", "a6", "a7", "a8", "a9", "a10"]);
            TestNext("a1", ["a1", "a2", "a3", "a4"]);
            TestNext("a2", ["a1", "a2", "a3", "a4"]);
            TestNext("a123", ["a1", "a2", "a3", "a4"]);

        }

        [Fact]
        public void LongVariables()
        {
            TestNext("var", ["var1", "var2", "var3"]);
            TestNext("var1", ["var1", "var2", "var3"]);
            TestNext("var2", ["var1", "var2", "var3"]);
            TestNext("var345", ["var1", "var2", "var3"]);
        }

        [Fact]
        public void VariablesStartingWithDigit()
        {
            TestNext("1", ["11", "12", "13"]);
            TestNext("5", ["51", "52", "53"]);
            TestNext("13", ["11", "12", "13", "14"]);
            TestNext("12345", ["11", "12", "13", "14"]);
        }

        [Fact]
        public void AddBeforeGenerating1()
        {
            var gen = new ExpVariableGenerator();
            gen.AddUnavailableVariable(new Variable("a1"));

            TestNext(gen, "a", ["a2", "a3", "a4", "a5"]);
        }

        [Fact]
        public void AddBeforeGenerating2()
        {
            var gen = new ExpVariableGenerator();
            gen.AddUnavailableVariable(new Variable("a2"));

            TestNext(gen, "a", ["a1", "a3", "a4", "a5"]);
        }

        [Fact]
        public void AddManyBeforeGenerating()
        {
            var gen = new ExpVariableGenerator();
            gen.AddUnavailableVariable(new Variable("a1"));
            gen.AddUnavailableVariable(new Variable("a2"));
            gen.AddUnavailableVariable(new Variable("a4"));
            gen.AddUnavailableVariable(new Variable("a5"));
            gen.AddUnavailableVariable(new Variable("a9"));
            gen.AddUnavailableVariable(new Variable("a10"));

            TestNext(gen, "a", ["a3", "a6", "a7", "a8", "a11", "a12", "a13"]);
        }

        [Fact]
        public void AddBetweenGenerating()
        {
            var gen = new ExpVariableGenerator();
            gen.Next(new Variable("x"));
            gen.Next(new Variable("x"));

            gen.AddUnavailableVariable(new Variable("x4"));

            TestNext(gen, "x", ["x3", "x5", "x6"]);
        }

        [Fact]
        public void TwoStems()
        {
            var gen = new ExpVariableGenerator();
            Assert.Equal("a1", gen.Next(new Variable("a")).Name);
            Assert.Equal("b1", gen.Next(new Variable("b")).Name);

            gen.AddUnavailableVariable(new Variable("a3"));
            gen.AddUnavailableVariable(new Variable("b4"));

            TestNext(gen, "a1", ["a2", "a4", "a5"]);
            TestNext(gen, "b1", ["b2", "b3", "b5", "b6"]);
        }
    }
}
