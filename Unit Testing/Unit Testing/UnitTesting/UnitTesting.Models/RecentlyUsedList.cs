namespace UnitTesting.Models
{
    public class RecentlyUsedList
    {
        private readonly List<string> _items = new();
        private readonly int? _capacity;

        public RecentlyUsedList(int? capacity = 5)
        {
            _capacity = capacity;
        }

        public int Count => _items.Count;

        public string this[int index] => _items[index];

        public void Add(string item)
        {
            if (string.IsNullOrEmpty(item))
                throw new ArgumentException("Item must not be null or empty.", nameof(item));

            _items.Remove(item);
            _items.Insert(0, item);

            if (_capacity.HasValue && _items.Count > _capacity.Value)
                _items.RemoveAt(_items.Count - 1);
        }
    }
}
