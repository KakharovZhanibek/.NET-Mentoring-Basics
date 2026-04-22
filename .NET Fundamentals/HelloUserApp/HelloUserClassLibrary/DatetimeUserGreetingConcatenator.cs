using System;

namespace HelloUserLibrary
{
    public static class DatetimeUserGreetingConcatenator
    {
        public static string Concatenate(string userGreeting)
        {
            var dateTime = DateTime.Now;
            return $"{dateTime} {userGreeting}";
        }
    }
}
