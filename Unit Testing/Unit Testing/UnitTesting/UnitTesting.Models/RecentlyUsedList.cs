namespace UnitTesting.Models
{
    public class RecentlyUsedList
    {
        private readonly List<string> _items = new();

        public int Count => _items.Count;
    }
}
