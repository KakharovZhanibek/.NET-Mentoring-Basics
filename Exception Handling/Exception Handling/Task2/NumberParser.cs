using System;

namespace Task2
{
    public class NumberParser : INumberParser
    {
        public int Parse(string stringValue)
        {
            // Check for null
            if (stringValue == null)
            {
                throw new ArgumentNullException(nameof(stringValue));
            }
            
            // Trim trailing whitespace only
            stringValue = stringValue.TrimEnd();
            
            // Check for empty or whitespace-only string
            if (stringValue.Length == 0)
            {
                throw new FormatException("Input string was not in a correct format.");
            }
            
            int index = 0;
            bool isNegative = false;
            
            // Check for sign
            if (stringValue[index] == '+')
            {
                index++;
            }
            else if (stringValue[index] == '-')
            {
                isNegative = true;
                index++;
            }
            
            // Check if there are digits after the sign
            if (index >= stringValue.Length)
            {
                throw new FormatException("Input string was not in a correct format.");
            }
            
            // Check if character after sign is a digit
            if (!char.IsDigit(stringValue[index]))
            {
                throw new FormatException("Input string was not in a correct format.");
            }
            
            long result = 0;
            
            // Parse digits
            for (; index < stringValue.Length; index++)
            {
                char c = stringValue[index];
                
                if (!char.IsDigit(c))
                {
                    throw new FormatException("Input string was not in a correct format.");
                }
                
                int digit = c - '0';
                result = result * 10 + digit;
                
                // Check for overflow
                if (isNegative)
                {
                    if (-result < int.MinValue)
                    {
                        throw new OverflowException("Value was either too large or too small for an Int32.");
                    }
                }
                else
                {
                    if (result > int.MaxValue)
                    {
                        throw new OverflowException("Value was either too large or too small for an Int32.");
                    }
                }
            }
            
            return isNegative ? (int)-result : (int)result;
        }
    }
}