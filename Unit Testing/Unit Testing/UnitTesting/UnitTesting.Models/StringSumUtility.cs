using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTesting.Models
{
    public class StringSumUtility
    {
        public string Sum(string num1, string num2)
        {
            return (ParseNatural(num1) + ParseNatural(num2)).ToString();
        }

        private static int ParseNatural(string value)
        {
            return int.TryParse(value, out int n) && n >= 0 ? n : 0;
        }
    }
}
