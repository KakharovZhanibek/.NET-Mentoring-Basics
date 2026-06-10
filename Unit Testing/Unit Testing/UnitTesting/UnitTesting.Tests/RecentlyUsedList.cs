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
    }
}
