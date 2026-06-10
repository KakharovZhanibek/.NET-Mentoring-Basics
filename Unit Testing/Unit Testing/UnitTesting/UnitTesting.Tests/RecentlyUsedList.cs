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
    }
}
