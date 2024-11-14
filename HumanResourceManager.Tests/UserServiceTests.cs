using HumanResourceManager.Infra.Context;
using Microsoft.EntityFrameworkCore;
using Moq;
using Testcontainers.PostgreSql;
using Serilog;
using HumanResourceManager.Domain.Interfaces;
using HumanResourceManager.Domain.Services;
using HumanResourceManager.Infra.Repositories;
using Npgsql;
using HumanResourceManager.Domain.Entities;
using HumanResourceManager.Domain.Dto.Miscellaneous;

namespace HumanResourceManager.Tests
{
    public class UserServiceTests : IDisposable
    {
        private readonly PostgreSqlContainer _container;
        private readonly PostgresqlContext _dbContext;

        private readonly IRepository _repo;

        private readonly Mock<ILogger> _mockLogger;
        private readonly IUserService _userService;
        private readonly Mock<IUserService> _mockUserService;


        public UserServiceTests() 
        {
            // Start PostgreSQL container
            _container = new PostgreSqlBuilder()
                .WithImage("postgres:latest")
                .WithCleanUp(true)
                .WithDatabase("postgres")
                .WithUsername("postgres")
                .WithPassword("123@qwe")
                .Build();

            _container.StartAsync().GetAwaiter().GetResult();

            var options = new DbContextOptionsBuilder<PostgresqlContext>()
                .UseNpgsql(_container.GetConnectionString())
                .Options;

            _dbContext = new PostgresqlContext(options);
            _dbContext.Database.EnsureCreated();

            _repo = new Repository(_dbContext);

            _mockUserService = new Mock<IUserService>();
            _mockLogger = new Mock<ILogger>();
            _userService = new UserService(_mockLogger.Object, _repo);
        }

        [Fact]
        public async Task Create_ValidUser_ReturnsSuccessResponse()
        {
            // Arrange
            User user = new User { Name = "ValidUser ReturnsSuccessResponse", Email = "ValidUser@gmail.com",  Birthdate = DateTime.Now, Password = "password" };

            // Act
            var response = await _userService.Create(user);

            // Assert
            Assert.True(response.Success);
            Assert.Empty(response.Messages);
            Assert.NotNull(response.Data);
            _mockLogger.Verify(l => l.Information(It.IsAny<string>()), Times.Once);
            Assert.Contains(_dbContext.Users, u => u.Id == user.Id);
        }

        [Fact]
        public async Task Update_ValidUser_ReturnsSuccessResponse()
        {
            // Arrange
            User user = new User { Name = "ValidUser ReturnsSuccessResponse", Email = "ValidUser@gmail.com", Birthdate = DateTime.Now, Password = "password" };
            var createdResponse = await _userService.Create(user);

            user.Id = createdResponse.Data!.Id;
            user.Name = "ValidUser ReturnsSuccessResponse Updated";
            user.Email = "ValidUser-updated@gmail.com";

            // Act
            var response = await _userService.Update(user);

            // Assert
            Assert.True(response.Success);
            Assert.Empty(response.Messages);
            Assert.NotNull(response.Data);

            // Adjusted verification logic
            _mockLogger.Verify(l => l.Information(It.IsAny<string>()), Times.AtLeastOnce);
            Assert.Contains(_dbContext.Users, u => u.Id == user.Id);
        }

        [Fact]
        public async Task Delete_ValidUser_ReturnsSuccessResponse()
        {
            // Arrange
            User user = new User { Name = "ValidUser ReturnsSuccessResponse", Email = "ValidUser@gmail.com", Birthdate = DateTime.Now, Password = "password" };
            var createdResponse = await _userService.Create(user);

            user.Id = createdResponse.Data!.Id;

            // Act
            var response = await _userService.Delete(user.Id);

            // Assert
            Assert.True(response.Success);
            Assert.Empty(response.Messages);
            Assert.Null(response.Data);

            // Adjusted verification logic
            _mockLogger.Verify(l => l.Information(It.IsAny<string>()), Times.AtLeastOnce);
            Assert.DoesNotContain(_dbContext.Users, u => u.Id == user.Id);
        }

        [Fact]
        public async Task Update_InvalidUser_ReturnsErrorResponse()
        {
            // Arrange
            User user = new User { Name = "1", Email = "ValidUser@gmail.com", Birthdate = DateTime.Now, Password = "password" }; // Assume this user is invalid according to InputValidate
            _mockUserService.Setup(s => s.Update(user)).ReturnsAsync(new Response<User> { Success = false, Messages = ["Usuário não existe"], Data = null });

            // Act
            var response = await _userService.Update(user);

            // Assert
            Assert.False(response.Success);
            Assert.NotEmpty(response.Messages);
            Assert.Null(response.Data);
            _mockLogger.Verify(l => l.Error(It.IsAny<string>()), Times.Once);
            Assert.DoesNotContain(_dbContext.Users, u => u.Id == user.Id);
        }

        [Fact]
        public async Task Delete_InvalidUser_ReturnsErrorResponse()
        {
            // Arrange
            var userId = 1;
            _mockUserService.Setup(s => s.Delete(userId)).ReturnsAsync(new Response<dynamic> { Success = false, Messages = new List<string> { "Usuário não existe" }, Data = null });

            // Act
            var response = await _userService.Delete(userId);

            // Assert
            Assert.False(response.Success);
            Assert.NotEmpty(response.Messages);
            Assert.Null(response.Data);
            _mockLogger.Verify(l => l.Information(It.IsAny<string>()), Times.AtLeastOnce);
            _mockLogger.Verify(l =>  l.Warning(It.IsAny<string>()), Times.AtLeastOnce);
            Assert.DoesNotContain(_dbContext.Users, u => u.Id == userId);
        }


        [Fact]
        public async Task Create_InvalidUser_ReturnsErrorResponse()
        {
            // Arrange
            User user = new User { Name = "1", Email = "ValidUser@gmail.com", Birthdate = DateTime.Now, Password = "password" }; // Assume this user is invalid according to InputValidate
            _mockUserService.Setup(s => s.Create(user)).ReturnsAsync(new Response<User> { Success = false, Messages = ["Tamanho mínimo do campo nome é de 3 caracteres"], Data = null });

            // Act
            var response = await _userService.Create(user);

            // Assert
            Assert.False(response.Success);
            Assert.NotEmpty(response.Messages);
            Assert.Null(response.Data);
            _mockLogger.Verify(l => l.Error(It.IsAny<string>()), Times.Once);
            Assert.DoesNotContain(_dbContext.Users, u => u.Id == user.Id);
        }

        [Fact]
        public async Task Create_InvalidUser_Email_ReturnsErrorResponse()
        {
            // Arrange
            User user = new User { Name = "InvalidUser Email", Email = "1", Birthdate = DateTime.Now, Password = "password" }; // Assume this user is invalid according to InputValidate
            _mockUserService.Setup(s => s.Create(user)).ReturnsAsync(new Response<User> { Success = false, Messages = ["E-mail inválido"], Data = null });

            // Act
            var response = await _userService.Create(user);

            // Assert
            Assert.False(response.Success);
            Assert.NotEmpty(response.Messages);
            Assert.Null(response.Data);
            _mockLogger.Verify(l => l.Error(It.IsAny<string>()), Times.Once);
            Assert.DoesNotContain(_dbContext.Users, u => u.Id == user.Id);
        }

        [Fact]
        public async Task Create_InvalidUser_Password_ReturnsErrorResponse()
        {
            // Arrange
            User user = new User { Name = "InvalidUser Email", Email = "1", Birthdate = DateTime.Now, Password = "2" }; // Assume this user is invalid according to InputValidate
            _mockUserService.Setup(s => s.Create(user)).ReturnsAsync(new Response<User> { Success = false, Messages = ["Tamanho mínimo do campo senha é de 6 caracteres"], Data = null });

            // Act
            var response = await _userService.Create(user);

            // Assert
            Assert.False(response.Success);
            Assert.NotEmpty(response.Messages);
            Assert.Null(response.Data);
            _mockLogger.Verify(l => l.Error(It.IsAny<string>()), Times.Once);
            Assert.DoesNotContain(_dbContext.Users, u => u.Id == user.Id);
        }

        [Fact]
        public async Task Create_ThrowsException_ReturnsErrorResponse()
        {
            // Arrange
            User user = new User { Name = "ThrowsException", Email = "ValidUser@gmail.com", Birthdate = DateTime.Now, Password = "password" };
            _dbContext.Database.ExecuteSqlRaw("DROP TABLE tb_user;"); // Simulate an exception by deleting the database

            // Act
            var response = await _userService.Create(user);

            // Assert
            Assert.False(response.Success);
            Assert.NotEmpty(response.Messages);
            Assert.Null(response.Data);
            _mockLogger.Verify(l => l.Error(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task List_DefaultParameters_ReturnsExpectedPagination()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Name = "User1", Email = "User1@gmail.com", Password = "User1-passphrase" },
                new User { Name = "User2", Email = "User2@gmail.com", Password = "User2-passphrase" }
            };

            // Act
            await _userService.Create(users.First());
            await _userService.Create(users.Last());

            var result = await _userService.List();

            // Assert
            Assert.Equal(1, result.Page);
            Assert.Equal(users.Count, result.Total);
            Assert.Equal(users, result.Content);
            _mockLogger.Verify(l => l.Information(It.IsAny<string>()), Times.AtLeastOnce);
            _mockLogger.Verify(l => l.Error(It.IsAny<string>()), Times.Never);
        }


        [Fact]
        public async Task List_CustomParameters_ReturnsExpectedPagination()
        {
            // Arrange
            var users = new List<User>
            {
                new User { Name = "User1", Email = "User1@gmail.com", Password = "User1-passphrase" },
                new User { Name = "User2", Email = "User2@gmail.com", Password = "User2-passphrase" }
            };

            // Act
            await _userService.Create(users.First());
            await _userService.Create(users.Last());
            var result = await _userService.List(10, 20);

            // Assert
            Assert.Equal(1, result.Page);
            Assert.Equal(users.Count, result.Total);
            Assert.Equal([], result.Content);
            _mockLogger.Verify(l => l.Information(It.IsAny<string>()), Times.AtLeastOnce);
            _mockLogger.Verify(l => l.Error(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task List_ExceptionThrown_LogsErrorAndReturnsEmptyPagination()
        {
            // Arrange
            _dbContext.Database.ExecuteSqlRaw("DROP TABLE tb_user;");

            // Act
            var result = await _userService.List();

            // Assert
            Assert.Equal(1, result.Page);
            Assert.Equal(0, result.Total);
            Assert.Empty(result!.Content!);
            _mockLogger.Verify(l => l.Information(It.IsAny<string>()), Times.AtLeastOnce);
            _mockLogger.Verify(l => l.Error(It.IsAny<string>()), Times.AtLeastOnce);
        }

        public void Dispose()
        {
            _dbContext?.Dispose();
            _container?.DisposeAsync().GetAwaiter().GetResult();
        }
    }
}