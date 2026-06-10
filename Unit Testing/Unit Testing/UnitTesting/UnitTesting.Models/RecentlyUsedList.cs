namespace UnitTesting.Models
{
    public class RecentlyUsedList
    {
        private readonly List<string> _items = new();

        public int Count => _items.Count;

        public void Add(string item)
        {
            _items.Insert(0, item);
        }
    }
}
