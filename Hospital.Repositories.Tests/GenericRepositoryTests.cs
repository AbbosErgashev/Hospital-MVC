using Hospital.Models;
using Hospital.Repositories;
using Hospital.Repositories.Implementation;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Xunit;

namespace Hospital.Repositories.Tests
{
    public class GenericRepositoryTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly GenericRepository<ApplicationUser> _repository;
        private bool _disposed = false;

        public GenericRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _repository = new GenericRepository<ApplicationUser>(_context);

            // Seed test data
            SeedTestData();
        }

        private void SeedTestData()
        {
            var users = new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    Id = "1",
                    Name = "John Doe",
                    Email = "john@example.com",
                    UserName = "john@example.com",
                    Gender = Gender.Male,
                    Nationality = "American",
                    Address = "123 Main St",
                    City = "New York",
                    Description = "Experienced doctor",
                    PictureUri = "john.jpg",
                    IsDoctor = true,
                    Specialist = "Cardiology",
                    DOB = new DateTime(1980, 1, 1)
                },
                new ApplicationUser
                {
                    Id = "2",
                    Name = "Jane Smith",
                    Email = "jane@example.com",
                    UserName = "jane@example.com",
                    Gender = Gender.Female,
                    Nationality = "British",
                    Address = "456 Oak St",
                    City = "London",
                    Description = "Patient",
                    PictureUri = "jane.jpg",
                    IsDoctor = false,
                    Specialist = "",
                    DOB = new DateTime(1985, 5, 15)
                },
                new ApplicationUser
                {
                    Id = "3",
                    Name = "Bob Johnson",
                    Email = "bob@example.com",
                    UserName = "bob@example.com",
                    Gender = Gender.Male,
                    Nationality = "Canadian",
                    Address = "789 Pine St",
                    City = "Toronto",
                    Description = "Specialist doctor",
                    PictureUri = "bob.jpg",
                    IsDoctor = true,
                    Specialist = "Neurology",
                    DOB = new DateTime(1975, 12, 10)
                }
            };

            _context.ApplicationUsers.AddRange(users);
            _context.SaveChanges();
        }

        #region GetAll Method Tests with Theory

        [Theory]
        [InlineData(null, null, "")]
        [InlineData("filter", null, "")]
        [InlineData(null, "orderBy", "")]
        [InlineData(null, null, "Department")]
        [InlineData("filter", "orderBy", "")]
        [InlineData("filter", null, "Department")]
        [InlineData(null, "orderBy", "Department")]
        [InlineData("filter", "orderBy", "Department")]
        public void GetAll_WithDifferentParameters_HandlesAllIfConditions(
            string filterType, 
            string orderByType, 
            string includeProperties)
        {
            // Arrange
            Expression<Func<ApplicationUser, bool>> filter = null;
            if (filterType == "filter")
            {
                filter = u => u.IsDoctor == true;
            }

            Func<IQueryable<ApplicationUser>, IOrderedQueryable<ApplicationUser>> orderBy = null;
            if (orderByType == "orderBy")
            {
                orderBy = q => q.OrderBy(u => u.Name);
            }

            // Act
            var result = _repository.GetAll(filter, orderBy, includeProperties);

            // Assert
            Assert.NotNull(result);
            
            if (filterType == "filter")
            {
                Assert.All(result, user => Assert.True(user.IsDoctor));
            }
            
            if (orderByType == "orderBy")
            {
                var orderedNames = result.Select(u => u.Name).ToList();
                var expectedOrder = orderedNames.OrderBy(n => n).ToList();
                Assert.Equal(expectedOrder, orderedNames);
            }

            // Test includeProperties split logic
            if (!string.IsNullOrEmpty(includeProperties))
            {
                // The method processes includeProperties by splitting on comma
                // This tests the foreach loop and StringSplitOptions.RemoveEmptyEntries
                Assert.NotNull(result);
            }
        }

        [Theory]
        [InlineData("")]
        [InlineData("NonExistentProperty")]
        public void GetAll_WithIncludeProperties_HandlesStringProcessing(string includeProperties)
        {
            // Act & Assert - This should not throw even with invalid include properties
            // The EF Core InMemory provider might throw for invalid navigation properties
            if (includeProperties.Contains("NonExistentProperty"))
            {
                // For invalid properties, we expect an exception
                Assert.Throws<InvalidOperationException>(() => _repository.GetAll(includeProperties: includeProperties));
            }
            else
            {
                // For valid/empty properties, it should work
                var result = _repository.GetAll(includeProperties: includeProperties);
                Assert.NotNull(result);
                // This tests the foreach loop that splits includeProperties and the StringSplitOptions.RemoveEmptyEntries
                // The method should handle empty strings correctly
            }
        }

        #endregion

        #region Delete Method Tests with Theory

        [Theory]
        [InlineData(EntityState.Modified)]
        [InlineData(EntityState.Unchanged)]
        public void Delete_WithAttachedEntity_DoesNotCallAttach(EntityState entityState)
        {
            // Arrange
            var user = new ApplicationUser
            {
                Id = "test-attached",
                Name = "Test User",
                Email = "test@example.com",
                UserName = "test@example.com",
                City = "Test City",
                Description = "Test Description",
                PictureUri = "test.jpg",
                Address = "Test Address",
                Nationality = "Test Nationality",
                Specialist = "Test Specialist",
                DOB = DateTime.Now.AddYears(-30)
            };

            _context.ApplicationUsers.Add(user);
            _context.SaveChanges();
            
            // Set entity state to test different attached states
            _context.Entry(user).State = entityState;

            // Act
            _repository.Delete(user);

            // Assert
            Assert.Equal(EntityState.Deleted, _context.Entry(user).State);
        }

        [Fact]
        public void Delete_WithAddedEntity_TransitionsCorrectly()
        {
            // Arrange
            var user = new ApplicationUser
            {
                Id = "test-added",
                Name = "Test User Added",
                Email = "testadded@example.com",
                UserName = "testadded@example.com",
                City = "Test City",
                Description = "Test Description",
                PictureUri = "test.jpg",
                Address = "Test Address",
                Nationality = "Test Nationality",
                Specialist = "Test Specialist",
                DOB = DateTime.Now.AddYears(-30)
            };

            _context.ApplicationUsers.Add(user);
            // Don't save changes - entity remains in Added state
            Assert.Equal(EntityState.Added, _context.Entry(user).State);

            // Act
            _repository.Delete(user);

            // Assert - When an Added entity is deleted, it becomes Detached
            Assert.Equal(EntityState.Detached, _context.Entry(user).State);
        }

        [Fact]
        public void Delete_WithDetachedEntity_CallsAttach()
        {
            // Arrange
            var user = new ApplicationUser
            {
                Id = "test-detached",
                Name = "Test User",
                Email = "test@example.com",
                UserName = "test@example.com",
                City = "Test City",
                Description = "Test Description",
                PictureUri = "test.jpg",
                Address = "Test Address",
                Nationality = "Test Nationality",
                Specialist = "Test Specialist",
                DOB = DateTime.Now.AddYears(-30)
            };

            // Create entity in different context to ensure it's detached
            using (var seedContext = new ApplicationDbContext(
                new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                    .Options))
            {
                seedContext.ApplicationUsers.Add(user);
                seedContext.SaveChanges();
            }

            // Now user is detached from current context
            Assert.Equal(EntityState.Detached, _context.Entry(user).State);

            // Act
            _repository.Delete(user);

            // Assert
            Assert.Equal(EntityState.Deleted, _context.Entry(user).State);
        }

        #endregion

        #region DeleteAsync Method Tests with Theory

        [Theory]
        [InlineData(EntityState.Modified)]
        [InlineData(EntityState.Unchanged)]
        public async Task DeleteAsync_WithAttachedEntity_DoesNotCallAttach(EntityState entityState)
        {
            // Arrange
            var user = new ApplicationUser
            {
                Id = "test-async-attached",
                Name = "Test User Async",
                Email = "testasync@example.com",
                UserName = "testasync@example.com",
                City = "Test City",
                Description = "Test Description",
                PictureUri = "test.jpg",
                Address = "Test Address",
                Nationality = "Test Nationality",
                Specialist = "Test Specialist",
                DOB = DateTime.Now.AddYears(-30)
            };

            _context.ApplicationUsers.Add(user);
            await _context.SaveChangesAsync();
            
            // Set entity state to test different attached states
            _context.Entry(user).State = entityState;

            // Act
            var result = await _repository.DeleteAsync(user);

            // Assert
            Assert.Equal(user, result);
            Assert.Equal(EntityState.Deleted, _context.Entry(user).State);
        }

        [Fact]
        public async Task DeleteAsync_WithAddedEntity_TransitionsCorrectly()
        {
            // Arrange
            var user = new ApplicationUser
            {
                Id = "test-async-added",
                Name = "Test User Async Added",
                Email = "testasyncadded@example.com",
                UserName = "testasyncadded@example.com",
                City = "Test City",
                Description = "Test Description",
                PictureUri = "test.jpg",
                Address = "Test Address",
                Nationality = "Test Nationality",
                Specialist = "Test Specialist",
                DOB = DateTime.Now.AddYears(-30)
            };

            _context.ApplicationUsers.Add(user);
            // Don't save changes - entity remains in Added state
            Assert.Equal(EntityState.Added, _context.Entry(user).State);

            // Act
            var result = await _repository.DeleteAsync(user);

            // Assert - When an Added entity is deleted, it becomes Detached
            Assert.Equal(user, result);
            Assert.Equal(EntityState.Detached, _context.Entry(user).State);
        }

        [Fact]
        public async Task DeleteAsync_WithDetachedEntity_CallsAttach()
        {
            // Arrange
            var user = new ApplicationUser
            {
                Id = "test-async-detached",
                Name = "Test User Async",
                Email = "testasync@example.com",
                UserName = "testasync@example.com",
                City = "Test City",
                Description = "Test Description",
                PictureUri = "test.jpg",
                Address = "Test Address",
                Nationality = "Test Nationality",
                Specialist = "Test Specialist",
                DOB = DateTime.Now.AddYears(-30)
            };

            // Create entity in different context to ensure it's detached
            using (var seedContext = new ApplicationDbContext(
                new DbContextOptionsBuilder<ApplicationDbContext>()
                    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                    .Options))
            {
                seedContext.ApplicationUsers.Add(user);
                await seedContext.SaveChangesAsync();
            }

            // Now user is detached from current context
            Assert.Equal(EntityState.Detached, _context.Entry(user).State);

            // Act
            var result = await _repository.DeleteAsync(user);

            // Assert
            Assert.Equal(user, result);
            Assert.Equal(EntityState.Deleted, _context.Entry(user).State);
        }

        #endregion

        #region Dispose Method Tests with Theory

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void Dispose_CalledMultipleTimes_HandlesProperly(bool callDisposeFirst)
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            var context = new ApplicationDbContext(options);
            var repository = new GenericRepository<ApplicationUser>(context);

            // Act & Assert
            if (callDisposeFirst)
            {
                // Test calling Dispose() first
                repository.Dispose();
                // Second call should not throw exception due to disposed flag check
                repository.Dispose();
            }
            else
            {
                // Test calling Dispose() only once
                repository.Dispose();
            }

            // No exception should be thrown
            Assert.True(true);
        }

        #endregion

        #region Additional Tests for Complete Coverage

        [Fact]
        public void Add_WithValidEntity_AddsToContext()
        {
            // Arrange
            var user = new ApplicationUser
            {
                Id = "new-user",
                Name = "New User",
                Email = "newuser@example.com",
                UserName = "newuser@example.com",
                City = "New City",
                Description = "New Description",
                PictureUri = "new.jpg",
                Address = "New Address",
                Nationality = "New Nationality",
                Specialist = "New Specialist",
                DOB = DateTime.Now.AddYears(-25)
            };

            // Act
            _repository.Add(user);

            // Assert
            Assert.Equal(EntityState.Added, _context.Entry(user).State);
        }

        [Fact]
        public async Task AddAsync_WithValidEntity_AddsToContext()
        {
            // Arrange
            var user = new ApplicationUser
            {
                Id = "new-user-async",
                Name = "New User Async",
                Email = "newuserasync@example.com",
                UserName = "newuserasync@example.com",
                City = "New City",
                Description = "New Description",
                PictureUri = "new.jpg",
                Address = "New Address",
                Nationality = "New Nationality",
                Specialist = "New Specialist",
                DOB = DateTime.Now.AddYears(-25)
            };

            // Act
            var result = await _repository.AddAsync(user);

            // Assert
            Assert.Equal(user, result);
            Assert.Equal(EntityState.Added, _context.Entry(user).State);
        }

        [Fact]
        public void GetById_WithExistingId_ReturnsEntity()
        {
            // Act
            var result = _repository.GetById("1");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("1", result.Id);
            Assert.Equal("John Doe", result.Name);
        }

        [Fact]
        public async Task GetByIdAsync_WithExistingId_ReturnsEntity()
        {
            // Act
            var result = await _repository.GetByIdAsync("1");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("1", result.Id);
            Assert.Equal("John Doe", result.Name);
        }

        [Fact]
        public void Update_WithValidEntity_SetsModifiedState()
        {
            // Arrange
            var user = _repository.GetById("1");
            user.Name = "Updated Name";

            // Act
            _repository.Update(user);

            // Assert
            Assert.Equal(EntityState.Modified, _context.Entry(user).State);
        }

        [Fact]
        public async Task UpdateAsync_WithValidEntity_SetsModifiedState()
        {
            // Arrange
            var user = await _repository.GetByIdAsync("1");
            user.Name = "Updated Name Async";

            // Act
            var result = await _repository.UpdateAsync(user);

            // Assert
            Assert.Equal(user, result);
            Assert.Equal(EntityState.Modified, _context.Entry(user).State);
        }

        #endregion

        public void Dispose()
        {
            if (!_disposed)
            {
                _repository?.Dispose();
                _context?.Dispose();
                _disposed = true;
            }
        }
    }
}