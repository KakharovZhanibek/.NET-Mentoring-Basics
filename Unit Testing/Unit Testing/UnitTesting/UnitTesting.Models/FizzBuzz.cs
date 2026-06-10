using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTesting.Models
{
    public class FizzBuzz
    {
        public string GetResult(int number)
        {
            if (number % 3 == 0) return "Fizz";

            return number.ToString();
        }
    }
}
