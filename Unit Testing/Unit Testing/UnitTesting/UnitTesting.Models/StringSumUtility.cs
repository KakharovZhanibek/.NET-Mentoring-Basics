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
            int n1 = int.TryParse(num1, out int v1) && v1 >= 0 ? v1 : 0;
            int n2 = int.TryParse(num2, out int v2) && v2 >= 0 ? v2 : 0;

            return (n1 + n2).ToString();
        }
    }
}
