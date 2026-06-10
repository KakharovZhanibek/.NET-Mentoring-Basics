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
            var fizzBuzz = new FizzBuzz();

            var result = fizzBuzz.GetResult(1);

            Assert.Equal("1", result);
        }
    }
}
