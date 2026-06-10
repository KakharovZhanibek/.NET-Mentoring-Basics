namespace UnitTesting.Tests
{
    public class RecentlyUsedList
    {
        [Fact]
        public void NewList_HasCountOfZero()
        {
            var list = new UnitTesting.Models.RecentlyUsedList();

            Assert.Equal(0, list.Count);
        }

        [Fact]
        public void AddingOneItem_IncreasesCountToOne()
        {
            var list = new UnitTesting.Models.RecentlyUsedList();

            list.Add("alpha");

            Assert.Equal(1, list.Count);
        }

        [Fact]
        public void MostRecentlyAddedItem_IsAtIndexZero()
        {
            var list = new UnitTesting.Models.RecentlyUsedList();

            list.Add("alpha");

            Assert.Equal("alpha", list[0]);
        }

        [Fact]
        public void ItemsAreStoredInLastInFirstOutOrder()
        {
            var list = new UnitTesting.Models.RecentlyUsedList();

            list.Add("alpha");
            list.Add("beta");
            list.Add("gamma");

            Assert.Equal("gamma", list[0]);
            Assert.Equal("beta",  list[1]);
            Assert.Equal("alpha", list[2]);
        }

        [Fact]
        public void AddingDuplicateItem_MovesItToFront()
        {
            var list = new UnitTesting.Models.RecentlyUsedList();

            list.Add("alpha");
            list.Add("beta");
            list.Add("alpha");

            Assert.Equal(2,       list.Count);
            Assert.Equal("alpha", list[0]);
            Assert.Equal("beta",  list[1]);
        }

        [Fact]
        public void AddingNullItem_ThrowsArgumentException()
        {
            var list = new UnitTesting.Models.RecentlyUsedList();

            Assert.Throws<ArgumentException>(() => list.Add(null!));
        }

        [Fact]
        public void AddingEmptyStringItem_ThrowsArgumentException()
        {
            var list = new UnitTesting.Models.RecentlyUsedList();

            Assert.Throws<ArgumentException>(() => list.Add(string.Empty));
        }

        [Fact]
        public void DefaultCapacityIsFive_OldestItemDroppedOnOverflow()
        {
            var list = new UnitTesting.Models.RecentlyUsedList();

            list.Add("one");
            list.Add("two");
            list.Add("three");
            list.Add("four");
            list.Add("five");
            list.Add("six");

            Assert.Equal(5,     list.Count);
            Assert.Equal("six", list[0]);
            Assert.Equal("two", list[3]);
            Assert.Equal("one", list[4]);
        }

        [Fact]
        public void CustomBoundedCapacity_DropsOldestItemOnOverflow()
        {
            var list = new UnitTesting.Models.RecentlyUsedList(capacity: 3);

            list.Add("one");
            list.Add("two");
            list.Add("three");
            list.Add("four");

            Assert.Equal(3,       list.Count);
            Assert.Equal("four",  list[0]);
            Assert.Equal("three", list[1]);
            Assert.Equal("two",   list[2]);
        }

        [Fact]
        public void UnboundedList_NeverDropsItems()
        {
            var list = new UnitTesting.Models.RecentlyUsedList(capacity: null);

            for (int i = 1; i <= 20; i++)
                list.Add($"item{i}");

            Assert.Equal(20,        list.Count);
            Assert.Equal("item20",  list[0]);
            Assert.Equal("item1",   list[19]);
        }
    }
}
