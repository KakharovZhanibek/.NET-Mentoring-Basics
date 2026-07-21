using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitTesting.Models;
using Xunit;

namespace UnitTesting.Tests
{
    public class FizzBuzzTests
    {
        [Fact]
        public void PlainNumber_ReturnsNumberAsString()
        {
            var fizzBuzz = new UnitTesting.Models.FizzBuzz();

            var result = fizzBuzz.GetResult(1);

            Assert.Equal("1", result);
        }

        [Fact]
        public void MultipleOfThree_ReturnsFizz()
        {
            var fizzBuzz = new UnitTesting.Models.FizzBuzz();

            var result = fizzBuzz.GetResult(3);

            Assert.Equal("Fizz", result);
        }

        [Fact]
        public void MultipleOfFive_ReturnsBuzz()
        {
            var fizzBuzz = new UnitTesting.Models.FizzBuzz();

            var result = fizzBuzz.GetResult(5);

            Assert.Equal("Buzz", result);
        }

        [Fact]
        public void MultipleOfThreeAndFive_ReturnsFizzBuzz()
        {
            var fizzBuzz = new UnitTesting.Models.FizzBuzz();

            var result = fizzBuzz.GetResult(15);

            Assert.Equal("FizzBuzz", result);
        }

        [Fact]
        public void Generate_Returns100Results()
        {
            var fizzBuzz = new UnitTesting.Models.FizzBuzz();

            var results = fizzBuzz.Generate().ToList();

            Assert.Equal(100,       results.Count);
            Assert.Equal("1",       results[0]);
            Assert.Equal("2",       results[1]);
            Assert.Equal("Fizz",    results[2]);
            Assert.Equal("Buzz",    results[4]);
            Assert.Equal("FizzBuzz",results[14]);
            Assert.Equal("Buzz",    results[99]);
        }
    }
}
