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
            var result = string.Empty;

            if (number % 3 == 0) result += "Fizz";
            if (number % 5 == 0) result += "Buzz";

            return result.Length > 0 ? result : number.ToString();
        }

        public IEnumerable<string> Generate()
        {
            for (int i = 1; i <= 100; i++)
                yield return GetResult(i);
        }
    }
}
