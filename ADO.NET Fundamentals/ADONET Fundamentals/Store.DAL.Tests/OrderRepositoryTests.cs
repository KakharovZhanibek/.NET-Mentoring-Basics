using FluentAssertions;
using Store.DAL.Enums;
using Store.DAL.Models;
using Store.DAL.Repositories;
using Store.DAL.Tests.Infrastructure;

namespace Store.DAL.Tests
{
    [Collection("Database")]
    public class OrderRepositoryTests
    {
        private readonly DatabaseFixture _fixture;
        private readonly IOrderRepository _orderRepo;
        private readonly IProductRepository _productRepo;

        public OrderRepositoryTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _orderRepo = new OrderRepository(fixture.ConnectionString);
            _productRepo = new ProductRepository(fixture.ConnectionString);
            _fixture.ResetTables();
        }

        private static Product SampleProduct() => new()
        {
            Name = "Test Product",
            Description = null,
            Weight = 1m,
            Height = 1m,
            Width = 1m,
            Length = 1m
        };

        private static Order SampleOrder(int productId, OrderStatus status = OrderStatus.NotStarted, DateTime? createdDate = null) => new()
        {
            Status = status,
            CreatedDate = createdDate ?? new DateTime(2024, 6, 15),
            UpdatedDate = DateTime.UtcNow,
            ProductId = productId
        };

        [Fact]
        public async Task CreateAsync_ShouldReturnNewId()
        {
            var productId = await _productRepo.CreateAsync(SampleProduct());
            var id = await _orderRepo.CreateAsync(SampleOrder(productId));
            id.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnOrder_WhenExists()
        {
            var productId = await _productRepo.CreateAsync(SampleProduct());
            var id = await _orderRepo.CreateAsync(SampleOrder(productId, OrderStatus.Loading));

            var order = await _orderRepo.GetByIdAsync(id);

            order.Should().NotBeNull();
            order!.Status.Should().Be(OrderStatus.Loading);
            order.ProductId.Should().Be(productId);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
        {
            var order = await _orderRepo.GetByIdAsync(99999);
            order.Should().BeNull();
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyOrder()
        {
            var productId = await _productRepo.CreateAsync(SampleProduct());
            var id = await _orderRepo.CreateAsync(SampleOrder(productId, OrderStatus.NotStarted));

            var order = await _orderRepo.GetByIdAsync(id);
            order!.Status = OrderStatus.Done;
            await _orderRepo.UpdateAsync(order);

            var updated = await _orderRepo.GetByIdAsync(id);
            updated!.Status.Should().Be(OrderStatus.Done);
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveOrder()
        {
            var productId = await _productRepo.CreateAsync(SampleProduct());
            var id = await _orderRepo.CreateAsync(SampleOrder(productId));

            await _orderRepo.DeleteAsync(id);

            var order = await _orderRepo.GetByIdAsync(id);
            order.Should().BeNull();
        }

        [Fact]
        public async Task GetOrdersAsync_NoFilter_ShouldReturnAllOrders()
        {
            var productId = await _productRepo.CreateAsync(SampleProduct());
            await _orderRepo.CreateAsync(SampleOrder(productId));
            await _orderRepo.CreateAsync(SampleOrder(productId));

            var orders = await _orderRepo.GetOrdersAsync();
            orders.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetOrdersAsync_FilterByStatus_ShouldReturnMatchingOrders()
        {
            var productId = await _productRepo.CreateAsync(SampleProduct());
            await _orderRepo.CreateAsync(SampleOrder(productId, OrderStatus.InProgress));
            await _orderRepo.CreateAsync(SampleOrder(productId, OrderStatus.Done));
            await _orderRepo.CreateAsync(SampleOrder(productId, OrderStatus.Done));

            var orders = await _orderRepo.GetOrdersAsync(status: OrderStatus.Done);
            orders.Should().HaveCount(2);
            orders.Should().OnlyContain(o => o.Status == OrderStatus.Done);
        }

        [Fact]
        public async Task GetOrdersAsync_FilterByMonth_ShouldReturnMatchingOrders()
        {
            var productId = await _productRepo.CreateAsync(SampleProduct());
            await _orderRepo.CreateAsync(SampleOrder(productId, createdDate: new DateTime(2024, 3, 10)));
            await _orderRepo.CreateAsync(SampleOrder(productId, createdDate: new DateTime(2024, 6, 20)));
            await _orderRepo.CreateAsync(SampleOrder(productId, createdDate: new DateTime(2024, 6, 25)));

            var orders = await _orderRepo.GetOrdersAsync(month: 6);
            orders.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetOrdersAsync_FilterByYear_ShouldReturnMatchingOrders()
        {
            var productId = await _productRepo.CreateAsync(SampleProduct());
            await _orderRepo.CreateAsync(SampleOrder(productId, createdDate: new DateTime(2023, 1, 1)));
            await _orderRepo.CreateAsync(SampleOrder(productId, createdDate: new DateTime(2024, 1, 1)));
            await _orderRepo.CreateAsync(SampleOrder(productId, createdDate: new DateTime(2024, 1, 2)));

            var orders = await _orderRepo.GetOrdersAsync(year: 2024);
            orders.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetOrdersAsync_FilterByProductId_ShouldReturnMatchingOrders()
        {
            var productId1 = await _productRepo.CreateAsync(SampleProduct());
            var productId2 = await _productRepo.CreateAsync(SampleProduct());
            await _orderRepo.CreateAsync(SampleOrder(productId1));
            await _orderRepo.CreateAsync(SampleOrder(productId1));
            await _orderRepo.CreateAsync(SampleOrder(productId2));

            var orders = await _orderRepo.GetOrdersAsync(productId: productId1);
            orders.Should().HaveCount(2);
            orders.Should().OnlyContain(o => o.ProductId == productId1);
        }

        [Fact]
        public async Task DeleteBulkAsync_FilterByStatus_ShouldDeleteMatchingOrders()
        {
            var productId = await _productRepo.CreateAsync(SampleProduct());
            await _orderRepo.CreateAsync(SampleOrder(productId, OrderStatus.Cancelled));
            await _orderRepo.CreateAsync(SampleOrder(productId, OrderStatus.Cancelled));
            await _orderRepo.CreateAsync(SampleOrder(productId, OrderStatus.Done));

            await _orderRepo.DeleteBulkAsync(status: OrderStatus.Cancelled);

            var remaining = await _orderRepo.GetOrdersAsync();
            remaining.Should().HaveCount(1);
            remaining.First().Status.Should().Be(OrderStatus.Done);
        }

        [Fact]
        public async Task DeleteBulkAsync_FilterByMonth_ShouldDeleteMatchingOrders()
        {
            var productId = await _productRepo.CreateAsync(SampleProduct());
            await _orderRepo.CreateAsync(SampleOrder(productId, createdDate: new DateTime(2024, 5, 1)));
            await _orderRepo.CreateAsync(SampleOrder(productId, createdDate: new DateTime(2024, 5, 15)));
            await _orderRepo.CreateAsync(SampleOrder(productId, createdDate: new DateTime(2024, 8, 1)));

            await _orderRepo.DeleteBulkAsync(month: 5);

            var remaining = await _orderRepo.GetOrdersAsync();
            remaining.Should().HaveCount(1);
        }

        [Fact]
        public async Task DeleteBulkAsync_FilterByProductId_ShouldDeleteMatchingOrders()
        {
            var productId1 = await _productRepo.CreateAsync(SampleProduct());
            var productId2 = await _productRepo.CreateAsync(SampleProduct());
            await _orderRepo.CreateAsync(SampleOrder(productId1));
            await _orderRepo.CreateAsync(SampleOrder(productId2));

            await _orderRepo.DeleteBulkAsync(productId: productId1);

            var remaining = await _orderRepo.GetOrdersAsync();
            remaining.Should().HaveCount(1);
            remaining.First().ProductId.Should().Be(productId2);
        }
    }
}
