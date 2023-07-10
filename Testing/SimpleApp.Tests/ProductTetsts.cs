using SimpleApp.Models;
using Xunit;

namespace SimpleApp.Tests {
    public class ProductsTests {
        [Fact]
        public void CanChangProductName() {
            // Arrange
            var p = new Product { Name = "Test", Price = 100M };

            // Act
            p.Name = "New Name";

            // Assert
            Assert.Equal("New Name", p.Name);
        }

        [Fact]
        public void CanChangeProductPrice() {
            // Arrange
            var p = new Product { Name = "Test", Price = 100M };

            // Act
            p.Price = 200M;

            // Assert
            Assert.Equal(200M, p.Price);
        }
    }
}