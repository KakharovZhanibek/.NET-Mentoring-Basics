using FluentAssertions;
using Store.DAL.Models;
using Store.DAL.Repositories;
using Store.DAL.Tests.Infrastructure;

namespace Store.DAL.Tests
{
    [Collection("Database")]
    public class ProductRepositoryTests
    {
        private readonly DatabaseFixture _fixture;
        private readonly IProductRepository _repo;

        public ProductRepositoryTests(DatabaseFixture fixture)
        {
            _fixture = fixture;
            _repo = new ProductRepository(fixture.ConnectionString);
            _fixture.ResetTables();
        }

        private static Product SampleProduct(string name = "Widget") => new()
        {
            Name = name,
            Description = "A test product",
            Weight = 1.5m,
            Height = 10m,
            Width = 5m,
            Length = 20m
        };

        [Fact]
        public async Task CreateAsync_ShouldReturnNewId()
        {
            var id = await _repo.CreateAsync(SampleProduct());
            id.Should().BeGreaterThan(0);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnProduct_WhenExists()
        {
            var id = await _repo.CreateAsync(SampleProduct("Gadget"));
            var product = await _repo.GetByIdAsync(id);

            product.Should().NotBeNull();
            product!.Name.Should().Be("Gadget");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
        {
            var product = await _repo.GetByIdAsync(99999);
            product.Should().BeNull();
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllProducts()
        {
            await _repo.CreateAsync(SampleProduct("A"));
            await _repo.CreateAsync(SampleProduct("B"));
            await _repo.CreateAsync(SampleProduct("C"));

            var products = await _repo.GetAllAsync();
            products.Should().HaveCount(3);
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyProduct()
        {
            var id = await _repo.CreateAsync(SampleProduct("Before"));
            var product = await _repo.GetByIdAsync(id);
            product!.Name = "After";

            await _repo.UpdateAsync(product);

            var updated = await _repo.GetByIdAsync(id);
            updated!.Name.Should().Be("After");
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveProduct()
        {
            var id = await _repo.CreateAsync(SampleProduct("ToDelete"));
            await _repo.DeleteAsync(id);

            var product = await _repo.GetByIdAsync(id);
            product.Should().BeNull();
        }
    }
}
