using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace UnitTesting.Tests
{
    public class StringSumUtilityTests
    {
        [Fact]
        public void TwoEmptyStrings_ReturnsZero()
        {
            var utility = new UnitTesting.Models.StringSumUtility();

            var result = utility.Sum("", "");

            Assert.Equal("0", result);
        }

        [Fact]
        public void TwoSmallNaturalNumbers_ReturnsTheirSum()
        {
            var utility = new UnitTesting.Models.StringSumUtility();

            var result = utility.Sum("1", "2");

            Assert.Equal("3", result);
        }
    }
}
