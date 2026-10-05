using a.Dto;
using a.Interfaces;
using a.Models;
using a.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using NET.Models;
using StoreApi.DTOs;
using StoreApi.Interfaces;

namespace StoreApi.Tests;

[Trait("Category", "Unit")]
public class CategoryServiceTests : IDisposable
{
    private readonly Mock<ICategoryRepository> _mockCategoryRepo;
    private readonly Mock<ILogger<CategoryService>> _mockLogger;
    private readonly CategoryService _service;

    public CategoryServiceTests()
    {
        _mockCategoryRepo = new Mock<ICategoryRepository>();
        _mockLogger = new Mock<ILogger<CategoryService>>();
        _service = new CategoryService(_mockCategoryRepo.Object, _mockLogger.Object);
    }

    public void Dispose() { }

    [Fact]
    public async Task GetAllCategoriesAsync_WhenCategoriesExist_ReturnsCategoryDtos()
    {
        var categories = new List<Category>
        {
            CreateCategory(1, "Electronics"),
            CreateCategory(2, "Office")
        };
        _mockCategoryRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(categories);

        var result = await _service.GetAllCategoriesAsync();

        Assert.NotNull(result);
        Assert.Equal(2, result.Count());
        Assert.Contains(result, c => c.Name == "Electronics");
    }

    [Fact]
    public async Task GetCategoryByIdAsync_WithValidId_ReturnsCategoryDto()
    {
        _mockCategoryRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(CreateCategory(1, "Electronics"));

        var result = await _service.GetCategoryByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Electronics", result.Name);
    }

    [Fact]
    public async Task GetCategoryByIdAsync_WithInvalidId_ReturnsNull()
    {
        _mockCategoryRepo.Setup(repo => repo.GetByIdAsync(999)).ReturnsAsync((Category?)null);

        var result = await _service.GetCategoryByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateCategoryAsync_WithValidData_ReturnsCreatedCategoryDto()
    {
        var dto = new CategoryDto { Name = "Books" };
        _mockCategoryRepo.Setup(repo => repo.CreateAsync(It.IsAny<Category>()))
            .ReturnsAsync((Category category) => new Category { Id = 7, Name = category.Name });

        var result = await _service.CreateCategoryAsync(dto);

        Assert.NotNull(result);
        Assert.Equal("Books", result.Name);
        _mockCategoryRepo.Verify(repo => repo.CreateAsync(It.IsAny<Category>()), Times.Once);
    }

    [Fact]
    public async Task UpdateCategoryAsync_WithValidId_ReturnsUpdatedCategoryDto()
    {
        var existing = CreateCategory(3, "Old Name");
        var update = new CategoryDto { Name = "Updated Name" };
        _mockCategoryRepo.Setup(repo => repo.GetByIdAsync(3)).ReturnsAsync(existing);
        _mockCategoryRepo.Setup(repo => repo.UpdateAsync(It.IsAny<Category>())).ReturnsAsync((Category category) => category);

        var result = await _service.UpdateCategoryAsync(3, update);

        Assert.NotNull(result);
        Assert.Equal("Updated Name", result.Name);
    }

    [Fact]
    public async Task UpdateCategoryAsync_WithInvalidId_ReturnsNull()
    {
        _mockCategoryRepo.Setup(repo => repo.GetByIdAsync(404)).ReturnsAsync((Category?)null);

        var result = await _service.UpdateCategoryAsync(404, new CategoryDto { Name = "Ghost" });

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteCategoryAsync_WithValidId_ReturnsTrue()
    {
        _mockCategoryRepo.Setup(repo => repo.DeleteAsync(1)).ReturnsAsync(true);

        var result = await _service.DeleteCategoryAsync(1);

        Assert.True(result);
    }

    [Fact]
    public async Task DeleteCategoryAsync_WithInvalidId_ReturnsFalse()
    {
        _mockCategoryRepo.Setup(repo => repo.DeleteAsync(999)).ReturnsAsync(false);

        var result = await _service.DeleteCategoryAsync(999);

        Assert.False(result);
    }

    private static Category CreateCategory(int id, string name) => new() { Id = id, Name = name, Presents = new List<Present>() };
}

[Trait("Category", "Unit")]
public class DonorServiceTests : IDisposable
{
    private readonly Mock<IDonorRepository> _mockDonorRepo;
    private readonly Mock<ILogger<DonorService>> _mockLogger;
    private readonly DonorService _service;

    public DonorServiceTests()
    {
        _mockDonorRepo = new Mock<IDonorRepository>();
        _mockLogger = new Mock<ILogger<DonorService>>();
        _service = new DonorService(_mockLogger.Object, _mockDonorRepo.Object);
    }

    public void Dispose() { }

    [Fact]
    public async Task GetAllAsync_WhenDonorsExist_ReturnsDonorDtos()
    {
        var donors = new List<Donor>
        {
            new() { Id = 1, Name = "Alice", Email = "alice@test.com", Presents = new List<Present>() },
            new() { Id = 2, Name = "Bob", Email = "bob@test.com", Presents = new List<Present>() }
        };
        _mockDonorRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(donors);

        var result = await _service.GetAllAsync();

        Assert.Equal(2, result.Count());
        Assert.Contains(result, d => d.Name == "Alice");
    }

    [Fact]
    public async Task GetByIdAsync_WithValidId_ReturnsDonorDto()
    {
        var donor = new Donor { Id = 5, Name = "Carol", Email = "carol@test.com", Presents = new List<Present>() };
        _mockDonorRepo.Setup(repo => repo.GetByIdAsync(5)).ReturnsAsync(donor);

        var result = await _service.GetByIdAsync(5);

        Assert.NotNull(result);
        Assert.Equal("Carol", result.Name);
    }

    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        _mockDonorRepo.Setup(repo => repo.GetByIdAsync(999)).ReturnsAsync((Donor?)null);

        var result = await _service.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateDonorAsync_WithValidData_ReturnsCreatedDonorDto()
    {
        var dto = new CreateDonorDto { Name = "Dana", Email = "dana@test.com" };
        _mockDonorRepo.Setup(repo => repo.CreateDonorAsync(It.IsAny<Donor>())).ReturnsAsync((Donor donor) => new Donor { Id = 11, Name = donor.Name, Email = donor.Email, Presents = new List<Present>() });

        var result = await _service.CreateDonorAsync(dto);

        Assert.NotNull(result);
        Assert.Equal("Dana", result.Name);
        Assert.Equal("dana@test.com", result.Email);
    }

    [Fact]
    public async Task UpdateAsync_WithValidData_ReturnsUpdatedDonorDto()
    {
        var donor = new Donor { Id = 3, Name = "Old Name", Email = "old@test.com", Presents = new List<Present>() };
        var update = new UpdateDonorDto { Id = 3, Name = "Updated", Email = "updated@test.com" };
        _mockDonorRepo.Setup(repo => repo.GetByIdAsync(3)).ReturnsAsync(donor);
        _mockDonorRepo.Setup(repo => repo.UpdateAsync(It.IsAny<Donor>())).ReturnsAsync((Donor d) => d);

        var result = await _service.UpdateAsync(update);

        Assert.NotNull(result);
        Assert.Equal("Updated", result.Name);
        Assert.Equal("updated@test.com", result.Email);
    }

    [Fact]
    public async Task UpdateAsync_WithInvalidId_ReturnsNull()
    {
        _mockDonorRepo.Setup(repo => repo.GetByIdAsync(404)).ReturnsAsync((Donor?)null);

        var result = await _service.UpdateAsync(new UpdateDonorDto { Id = 404, Name = "Ghost", Email = "ghost@test.com" });

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_WithValidId_ReturnsTrue()
    {
        _mockDonorRepo.Setup(repo => repo.DeleteAsync(1)).ReturnsAsync(true);

        var result = await _service.DeleteAsync(1);

        Assert.True(result);
    }

    [Fact]
    public async Task GetByEmailAsync_WhenDonorExists_ReturnsDonorDto()
    {
        var donor = new Donor { Id = 7, Name = "Email Donor", Email = "lookup@test.com", Presents = new List<Present>() };
        _mockDonorRepo.Setup(repo => repo.GetByEmailAsync("lookup@test.com")).ReturnsAsync(donor);

        var result = await _service.GetByEmailAsync("lookup@test.com");

        Assert.NotNull(result);
        Assert.Equal("lookup@test.com", result.Email);
    }

    [Fact]
    public async Task GetByNameAsync_WhenDonorsExist_ReturnsDonorDtos()
    {
        var donors = new List<Donor?> { new() { Id = 1, Name = "Alpha", Email = "alpha@test.com", Presents = new List<Present>() } };
        _mockDonorRepo.Setup(repo => repo.GetByNameAsync("Alpha")).ReturnsAsync(donors);

        var result = await _service.getByNameAsync("Alpha");

        Assert.Single(result);
        Assert.Equal("Alpha", result.First().Name);
    }
}

[Trait("Category", "Unit")]
public class PresentServiceTests : IDisposable
{
    private readonly Mock<IPresentRepository> _mockPresentRepo;
    private readonly Mock<ICategoryRepository> _mockCategoryRepo;
    private readonly Mock<IPurchaseRepository> _mockPurchaseRepo;
    private readonly Mock<ILogger<PresentService>> _mockLogger;
    private readonly PresentService _service;

    public PresentServiceTests()
    {
        _mockPresentRepo = new Mock<IPresentRepository>();
        _mockCategoryRepo = new Mock<ICategoryRepository>();
        _mockPurchaseRepo = new Mock<IPurchaseRepository>();
        _mockLogger = new Mock<ILogger<PresentService>>();
        _service = new PresentService(_mockPresentRepo.Object, _mockCategoryRepo.Object, _mockPurchaseRepo.Object, _mockLogger.Object);
    }

    public void Dispose() { }

    [Fact]
    public async Task GetAllPresentsAsync_WhenPresentsExist_ReturnsPresentDtos()
    {
        var presents = new List<Present>
        {
            CreatePresent(1, "Laptop", 5000, 1),
            CreatePresent(2, "Phone", 2500, 1)
        };
        _mockPresentRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(presents);

        var result = await _service.GetAllPresentsAsync();

        Assert.Equal(2, result.Count());
        Assert.Contains(result, p => p.Name == "Laptop");
    }

    [Fact]
    public async Task GetPresentByIdAsync_WithValidId_ReturnsPresentDto()
    {
        var present = CreatePresent(10, "Tablet", 1200, 4);
        _mockPresentRepo.Setup(repo => repo.GetByIdAsync(10)).ReturnsAsync(present);

        var result = await _service.GetPresentByIdAsync(10);

        Assert.NotNull(result);
        Assert.Equal("Tablet", result.Name);
        Assert.Equal(1200, result.Price);
    }

    [Fact]
    public async Task GetPresentByIdAsync_WithInvalidId_ReturnsNull()
    {
        _mockPresentRepo.Setup(repo => repo.GetByIdAsync(999)).ReturnsAsync((Present?)null);

        var result = await _service.GetPresentByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task SearchPresentsByNameAsync_WithEmptySearchTerm_ReturnsEmptyList()
    {
        _mockPresentRepo.Setup(repo => repo.SearchByNameAsync(It.IsAny<string>())).ReturnsAsync(new List<Present>());

        var result = await _service.SearchPresentsByNameAsync(string.Empty);

        Assert.Empty(result);
    }

    [Fact]
    public async Task CreatePresentAsync_WithValidData_ReturnsCreatedPresentDto()
    {
        var createDto = new CreatePresentDto { Name = "Monitor", Description = "4K", Price = 1500, CategoryId = 2, DonorId = 3, ImageUrl = "/img/monitor.png" };
        _mockCategoryRepo.Setup(repo => repo.ExistsAsync(2)).ReturnsAsync(true);
        _mockPresentRepo.Setup(repo => repo.CreateAsync(It.IsAny<Present>())).ReturnsAsync((Present p) => new Present { Id = 5, Name = p.Name, Description = p.Description, Price = p.Price, CategoryId = p.CategoryId, DonorId = p.DonorId, ImageUrl = p.ImageUrl, Category = new Category { Id = 2, Name = "Electronics" }, Purchases = new List<Purchase>() });

        var result = await _service.CreatePresentAsync(createDto);

        Assert.NotNull(result);
        Assert.Equal("Monitor", result.Name);
        Assert.Equal(1500, result.Price);
    }

    [Fact]
    public async Task CreatePresentAsync_WithInvalidCategory_ThrowsArgumentException()
    {
        var createDto = new CreatePresentDto { Name = "Broken Gift", Price = 200, CategoryId = 33, DonorId = 1 };
        _mockCategoryRepo.Setup(repo => repo.ExistsAsync(33)).ReturnsAsync(false);

        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreatePresentAsync(createDto));
    }

    [Fact]
    public async Task UpdatePresentAsync_WithValidData_ReturnsUpdatedPresentDto()
    {
        var present = CreatePresent(8, "Old Name", 2000, 4);
        var update = new UpdatePresentDto { Name = "New Name", Price = 2500, CategoryId = 4, ImageUrl = "/img/new.png" };
        _mockPresentRepo.Setup(repo => repo.GetByIdAsync(8)).ReturnsAsync(present);
        _mockCategoryRepo.Setup(repo => repo.ExistsAsync(4)).ReturnsAsync(true);
        _mockPresentRepo.Setup(repo => repo.UpdateAsync(It.IsAny<Present>())).ReturnsAsync((Present p) => p);

        var result = await _service.UpdatePresentAsync(8, update);

        Assert.NotNull(result);
        Assert.Equal("New Name", result.Name);
        Assert.Equal(2500, result.Price);
    }

    [Fact]
    public async Task UpdatePresentAsync_WithInvalidCategory_ThrowsArgumentException()
    {
        var present = CreatePresent(8, "Old Name", 2000, 4);
        var update = new UpdatePresentDto { Price = 2500, CategoryId = 20 };
        _mockPresentRepo.Setup(repo => repo.GetByIdAsync(8)).ReturnsAsync(present);
        _mockCategoryRepo.Setup(repo => repo.ExistsAsync(20)).ReturnsAsync(false);

        await Assert.ThrowsAsync<ArgumentException>(() => _service.UpdatePresentAsync(8, update));
    }

    [Fact]
    public async Task DeletePresentAsync_WithValidId_ReturnsTrue()
    {
        _mockPresentRepo.Setup(repo => repo.DeleteAsync(4)).ReturnsAsync(true);

        var result = await _service.DeletePresentAsync(4);

        Assert.True(result);
    }

    [Fact]
    public async Task DeletePresentAsync_WithInvalidId_ReturnsFalse()
    {
        _mockPresentRepo.Setup(repo => repo.DeleteAsync(999)).ReturnsAsync(false);

        var result = await _service.DeletePresentAsync(999);

        Assert.False(result);
    }

    [Fact]
    public async Task GetAllPresentsPagedAsync_WhenPageRequested_ReturnsPagedResult()
    {
        var presents = new List<Present> { CreatePresent(1, "Laptop", 5000, 1), CreatePresent(2, "Phone", 2500, 1) };
        _mockPresentRepo.Setup(repo => repo.GetAllPagedAsync(1, 10)).ReturnsAsync((presents, 2));

        var result = await _service.GetAllPresentsPagedAsync(new PaginationParams { PageNumber = 1, PageSize = 10 });

        Assert.NotNull(result);
        Assert.Equal(2, result.TotalCount);
        Assert.Equal(2, result.Items.Count());
    }

    private static Present CreatePresent(int id, string name, int price, int categoryId)
    {
        return new Present
        {
            Id = id,
            Name = name,
            Price = price,
            CategoryId = categoryId,
            DonorId = 1,
            Category = new Category { Id = categoryId, Name = "Electronics" },
            Donor = new Donor { Id = 1, Name = "Donor", Email = "donor@test.com", Presents = new List<Present>() },
            Purchases = new List<Purchase>()
        };
    }
}

[Trait("Category", "Unit")]
public class PurchaseServiceTests : IDisposable
{
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly Mock<IPurchaseRepository> _mockPurchaseRepo;
    private readonly Mock<IPresentRepository> _mockPresentRepo;
    private readonly Mock<ILogger<PurchaseService>> _mockLogger;
    private readonly PurchaseService _service;

    public PurchaseServiceTests()
    {
        _mockUserRepo = new Mock<IUserRepository>();
        _mockPurchaseRepo = new Mock<IPurchaseRepository>();
        _mockPresentRepo = new Mock<IPresentRepository>();
        _mockLogger = new Mock<ILogger<PurchaseService>>();
        _service = new PurchaseService(_mockUserRepo.Object, _mockPurchaseRepo.Object, _mockPresentRepo.Object, _mockLogger.Object);
    }

    public void Dispose() { }

    [Fact]
    public async Task CreatePurchaseAsync_WithValidData_ReturnsPurchaseDto()
    {
        var dto = new PurchaseDto { UserId = 1, PresentId = 2, NumOfTickets = 3, IsDraft = true };
        _mockUserRepo.Setup(repo => repo.ExistsAsync(1)).ReturnsAsync(true);
        _mockPurchaseRepo.Setup(repo => repo.CreateAsync(It.IsAny<Purchase>())).ReturnsAsync((Purchase purchase) => new Purchase { Id = 9, UserId = purchase.UserId, PresentId = purchase.PresentId, NumOfTickets = purchase.NumOfTickets, IsDraft = purchase.IsDraft, User = new User { Id = 1, Email = "u@test.com", Password = "pw", FirstName = "A", LastName = "B", Address = "Addr", Phone = "123" }, Present = new Present { Id = 2, Name = "Gift", Price = 100, CategoryId = 1, DonorId = 1, Category = new Category { Id = 1, Name = "Cats" }, Donor = new Donor { Id = 1, Name = "Donor", Email = "d@test.com", Presents = new List<Present>() }, Purchases = new List<Purchase>() } });

        var result = await _service.CreatePurchaseAsync(dto);

        Assert.NotNull(result);
        Assert.Equal(1, result.UserId);
        Assert.Equal(3, result.NumOfTickets);
    }

    [Fact]
    public async Task CreatePurchaseAsync_WithInvalidUser_ThrowsArgumentException()
    {
        var dto = new PurchaseDto { UserId = 99, PresentId = 2, NumOfTickets = 3 };
        _mockUserRepo.Setup(repo => repo.ExistsAsync(99)).ReturnsAsync(false);

        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreatePurchaseAsync(dto));
    }

    [Fact]
    public async Task GetPurchaseByIdAsync_WithValidId_ReturnsPurchaseDto()
    {
        var purchase = new Purchase { Id = 4, UserId = 1, PresentId = 2, NumOfTickets = 5, IsDraft = true, User = new User { Id = 1, Email = "u@test.com", Password = "pw", FirstName = "A", LastName = "B", Address = "Addr", Phone = "123" }, Present = new Present { Id = 2, Name = "Gift", Price = 100, CategoryId = 1, DonorId = 1, Category = new Category { Id = 1, Name = "Cats" }, Donor = new Donor { Id = 1, Name = "Donor", Email = "d@test.com", Presents = new List<Present>() }, Purchases = new List<Purchase>() } };
        _mockPurchaseRepo.Setup(repo => repo.GetByIdAsync(4)).ReturnsAsync(purchase);

        var result = await _service.GetPurchaseByIdAsync(4);

        Assert.NotNull(result);
        Assert.Equal(4, result.Id);
        Assert.Equal(5, result.NumOfTickets);
    }

    [Fact]
    public async Task GetPurchaseByIdAsync_WithInvalidId_ReturnsNull()
    {
        _mockPurchaseRepo.Setup(repo => repo.GetByIdAsync(404)).ReturnsAsync((Purchase?)null);

        var result = await _service.GetPurchaseByIdAsync(404);

        Assert.Null(result);
    }

    [Fact]
    public async Task GetPurchasesByUserIdAsync_WhenPurchasesExist_ReturnsPurchaseDtos()
    {
        var purchases = new List<Purchase>
        {
            new() { Id = 1, UserId = 5, PresentId = 2, NumOfTickets = 2, IsDraft = true },
            new() { Id = 2, UserId = 5, PresentId = 3, NumOfTickets = 4, IsDraft = true }
        };
        _mockPurchaseRepo.Setup(repo => repo.GetByUserIdAsync(5)).ReturnsAsync(purchases);

        var result = await _service.GetPurchasesByUserIdAsync(5);

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task UpdatePurchaseAsync_WithValidDraftPurchase_ReturnsUpdatedPurchaseDto()
    {
        var existing = new Purchase { Id = 1, UserId = 1, PresentId = 2, NumOfTickets = 1, IsDraft = true };
        var update = new UpdatePurchaseDto { Id = 1, UserId = 2, NumOfTickets = 10, IsDraft = true, PresentId = 2 };
        _mockPurchaseRepo.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(existing);
        _mockPresentRepo.Setup(repo => repo.GetByIdAsync(2)).ReturnsAsync(new Present { Id = 2, Name = "Gift", Price = 100, CategoryId = 1, DonorId = 1, Category = new Category { Id = 1, Name = "Cats" }, Donor = new Donor { Id = 1, Name = "Donor", Email = "d@test.com", Presents = new List<Present>() }, Purchases = new List<Purchase>() });
        _mockPurchaseRepo.Setup(repo => repo.UpdateAsync(It.IsAny<Purchase>())).ReturnsAsync((Purchase purchase) => purchase);

        var result = await _service.UpdatePurchaseAsync(1, update);

        Assert.NotNull(result);
        Assert.Equal(10, result.NumOfTickets);
        Assert.Equal(2, result.UserId);
    }

    [Fact]
    public async Task UpdatePurchaseAsync_WithNonDraftPurchase_ReturnsNull()
    {
        var existing = new Purchase { Id = 2, UserId = 1, PresentId = 2, NumOfTickets = 1, IsDraft = false };
        _mockPurchaseRepo.Setup(repo => repo.GetByIdAsync(2)).ReturnsAsync(existing);

        var result = await _service.UpdatePurchaseAsync(2, new UpdatePurchaseDto { Id = 2, NumOfTickets = 9, IsDraft = true });

        Assert.Null(result);
    }

    [Fact]
    public async Task DeletePurchaseAsync_WithValidId_ReturnsTrue()
    {
        _mockPurchaseRepo.Setup(repo => repo.DeleteAsync(3)).ReturnsAsync(true);

        var result = await _service.DeletePurchaseAsync(3);

        Assert.True(result);
    }

    [Fact]
    public async Task DeletePurchaseAsync_WithInvalidId_ReturnsFalse()
    {
        _mockPurchaseRepo.Setup(repo => repo.DeleteAsync(999)).ReturnsAsync(false);

        var result = await _service.DeletePurchaseAsync(999);

        Assert.False(result);
    }
}

[Trait("Category", "Unit")]
public class UserServiceTests : IDisposable
{
    private readonly Mock<IUserRepository> _mockUserRepo;
    private readonly Mock<IDonorRepository> _mockDonorRepo;
    private readonly Mock<ITokenService> _mockTokenService;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly Mock<ILogger<UserService>> _mockLogger;
    private readonly UserService _service;

    public UserServiceTests()
    {
        _mockUserRepo = new Mock<IUserRepository>();
        _mockDonorRepo = new Mock<IDonorRepository>();
        _mockTokenService = new Mock<ITokenService>();
        _mockConfiguration = new Mock<IConfiguration>();
        _mockLogger = new Mock<ILogger<UserService>>();

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["jwtSettings:ExpiryMinutes"] = "60"
            })
            .Build();

        _service = new UserService(_mockUserRepo.Object, _mockTokenService.Object, _mockDonorRepo.Object, configuration, _mockLogger.Object);
    }

    public void Dispose() { }

    [Fact]
    public async Task GetAllUsersAsync_WhenUsersExist_ReturnsUserDtos()
    {
        var users = new List<User>
        {
            new() { Id = 1, Email = "a@test.com", Password = "pw", FirstName = "A", LastName = "A", Phone = "1", Address = "addr" },
            new() { Id = 2, Email = "b@test.com", Password = "pw", FirstName = "B", LastName = "B", Phone = "2", Address = "addr" }
        };
        _mockUserRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(users);

        var result = await _service.GetAllUsersAsync();

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetUserByIdAsync_WithValidId_ReturnsUserDto()
    {
        var user = new User { Id = 9, Email = "u@test.com", Password = "pw", FirstName = "User", LastName = "Nine", Phone = "111", Address = "addr" };
        _mockUserRepo.Setup(repo => repo.GetByIdAsync(9)).ReturnsAsync(user);

        var result = await _service.GetUserByIdAsync(9);

        Assert.NotNull(result);
        Assert.Equal("u@test.com", result.Email);
    }

    [Fact]
    public async Task GetUserByIdAsync_WithInvalidId_ReturnsNull()
    {
        _mockUserRepo.Setup(repo => repo.GetByIdAsync(999)).ReturnsAsync((User?)null);

        var result = await _service.GetUserByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task CreateUserAsync_WithValidData_ReturnsCreatedUserDto()
    {
        var createDto = new CreateUserDto { Email = "create@test.com", Password = "password123", FirstName = "Create", LastName = "User", Phone = "123", Address = "Addr" };
        _mockUserRepo.Setup(repo => repo.EmailExistsAsync("create@test.com")).ReturnsAsync(false);
        _mockUserRepo.Setup(repo => repo.CreateAsync(It.IsAny<User>())).ReturnsAsync((User user) => new User { Id = 7, Email = user.Email, FirstName = user.FirstName, LastName = user.LastName, Password = user.Password, Phone = user.Phone, Address = user.Address });

        var result = await _service.CreateUserAsync(createDto);

        Assert.NotNull(result);
        Assert.Equal("create@test.com", result.Email);
        Assert.Equal("Create", result.FirstName);
    }

    [Fact]
    public async Task CreateUserAsync_WithDuplicateEmail_ThrowsArgumentException()
    {
        var createDto = new CreateUserDto { Email = "exists@test.com", Password = "password123", FirstName = "A", LastName = "B", Phone = "123", Address = "Addr" };
        _mockUserRepo.Setup(repo => repo.EmailExistsAsync("exists@test.com")).ReturnsAsync(true);

        await Assert.ThrowsAsync<ArgumentException>(() => _service.CreateUserAsync(createDto));
    }

    [Fact]
    public async Task UpdateUserAsync_WithValidData_ReturnsUpdatedUserDto()
    {
        var user = new User { Id = 4, Email = "old@test.com", Password = "pw", FirstName = "Old", LastName = "Name", Phone = "123", Address = "Addr" };
        var updateDto = new CreateUserDto { Email = "new@test.com", Password = "pw2", FirstName = "New", LastName = "Name", Phone = "456", Address = "New Addr" };
        _mockUserRepo.Setup(repo => repo.GetByIdAsync(4)).ReturnsAsync(user);
        _mockUserRepo.Setup(repo => repo.EmailExistsAsync("new@test.com")).ReturnsAsync(false);
        _mockUserRepo.Setup(repo => repo.UpdateAsync(It.IsAny<User>())).ReturnsAsync((User entity) => entity);

        var result = await _service.UpdateUserAsync(4, updateDto);

        Assert.NotNull(result);
        Assert.Equal("New", result.FirstName);
    }

    [Fact]
    public async Task UpdateUserAsync_WithInvalidId_ReturnsNull()
    {
        _mockUserRepo.Setup(repo => repo.GetByIdAsync(404)).ReturnsAsync((User?)null);

        var result = await _service.UpdateUserAsync(404, new CreateUserDto { Email = "x@test.com", Password = "pw", FirstName = "X", LastName = "Y", Address = "Z" });

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteUserAsync_WithValidId_ReturnsTrue()
    {
        _mockUserRepo.Setup(repo => repo.DeleteAsync(10)).ReturnsAsync(true);

        var result = await _service.DeleteUserAsync(10);

        Assert.True(result);
    }

    [Fact]
    public async Task AuthenticateAsync_WithValidCredentials_ReturnsLoginResponse()
    {
        var user = new User { Id = 1, Email = "john@test.com", Password = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("correctpass")), FirstName = "John", LastName = "Doe", Phone = "123", Address = "Addr" };
        _mockUserRepo.Setup(repo => repo.GetByEmailAsync("john@test.com")).ReturnsAsync(user);
        _mockTokenService.Setup(ts => ts.GenerateToken(1, "john@test.com", "John", "Doe", false)).Returns("jwt-token");

        var result = await _service.AuthenticateAsync("john@test.com", "correctpass");

        Assert.NotNull(result);
        Assert.Equal("jwt-token", result.Token);
        Assert.Equal("Bearer", result.TokenType);
    }

    [Fact]
    public async Task AuthenticateAsync_WithInvalidPassword_ReturnsNull()
    {
        var user = new User { Id = 1, Email = "john@test.com", Password = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("correctpass")), FirstName = "John", LastName = "Doe", Phone = "123", Address = "Addr" };
        _mockUserRepo.Setup(repo => repo.GetByEmailAsync("john@test.com")).ReturnsAsync(user);

        var result = await _service.AuthenticateAsync("john@test.com", "wrongpass");

        Assert.Null(result);
    }
}

[Trait("Category", "Unit")]
public class LotteryResultServiceTests : IDisposable
{
    private readonly Mock<ILotteryResultRepository> _mockRepo;
    private readonly Mock<ILogger<LotteryResultService>> _mockLogger;
    private readonly LotteryResultService _service;

    public LotteryResultServiceTests()
    {
        _mockRepo = new Mock<ILotteryResultRepository>();
        _mockLogger = new Mock<ILogger<LotteryResultService>>();
        _service = new LotteryResultService(_mockLogger.Object, _mockRepo.Object);
    }

    public void Dispose() { }

    [Fact]
    public async Task CreateWinnerAsync_WithValidData_ReturnsLotteryResultDto()
    {
        var dto = new CreateLotteryResultDto { PresentId = 12, WinnerUserId = 20, LotteryDate = DateTime.UtcNow };
        _mockRepo.Setup(repo => repo.CreateWinnerAsync(It.IsAny<LotteryResult>())).ReturnsAsync((LotteryResult result) => new LotteryResult { Id = 5, PresentId = result.PresentId, WinnerUserId = result.WinnerUserId, LotteryDate = result.LotteryDate, Present = new Present { Id = 12, Name = "Gift", Price = 100, CategoryId = 1, DonorId = 1, Description = "Desc", Category = new Category { Id = 1, Name = "Category" }, Donor = new Donor { Id = 1, Name = "Donor", Email = "donor@test.com", Presents = new List<Present>() }, Purchases = new List<Purchase>() }, Winner = new User { Id = 20, Email = "winner@test.com", Password = "pw", FirstName = "Winner", LastName = "User", Phone = "1", Address = "Addr" } });

        var result = await _service.CreateWinnerAsync(dto);

        Assert.NotNull(result);
        Assert.Equal(12, result.PresentId);
        Assert.Equal(20, result.Winner.Id);
    }

    [Fact]
    public async Task GetAllAsync_WhenResultsExist_ReturnsLotteryResultDtos()
    {
        var results = new List<LotteryResult>
        {
            new() { Id = 1, PresentId = 1, WinnerUserId = 2, LotteryDate = DateTime.UtcNow, Present = new Present { Id = 1, Name = "Gift", Description = "You won", Price = 100, CategoryId = 1, DonorId = 1, Category = new Category { Id = 1, Name = "Cat" }, Donor = new Donor { Id = 1, Name = "Donor", Email = "d@test.com", Presents = new List<Present>() }, Purchases = new List<Purchase>() }, Winner = new User { Id = 2, Email = "winner@test.com", Password = "pw", FirstName = "Winner", LastName = "User", Phone = "123", Address = "Addr" } },
            new() { Id = 2, PresentId = 2, WinnerUserId = 3, LotteryDate = DateTime.UtcNow, Present = new Present { Id = 2, Name = "Gift2", Description = "You won", Price = 110, CategoryId = 1, DonorId = 1, Category = new Category { Id = 1, Name = "Cat" }, Donor = new Donor { Id = 1, Name = "Donor", Email = "d@test.com", Presents = new List<Present>() }, Purchases = new List<Purchase>() }, Winner = new User { Id = 3, Email = "winner2@test.com", Password = "pw", FirstName = "Winner2", LastName = "User", Phone = "123", Address = "Addr" } }
        };
        _mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(results);

        var result = await _service.GetAllAsync();

        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task MakeLotteryAsync_WithValidPresentId_ReturnsLotteryResultDto()
    {
        var result = new LotteryResult { Id = 15, PresentId = 99, WinnerUserId = 6, LotteryDate = DateTime.UtcNow, Present = new Present { Id = 99, Name = "Prize", Description = "Prize", Price = 123, CategoryId = 1, DonorId = 1, Category = new Category { Id = 1, Name = "Cat" }, Donor = new Donor { Id = 1, Name = "Donor", Email = "d@test.com", Presents = new List<Present>() }, Purchases = new List<Purchase>() }, Winner = new User { Id = 6, Email = "w@test.com", Password = "pw", FirstName = "W", LastName = "User", Phone = "123", Address = "Addr" } };
        _mockRepo.Setup(repo => repo.MakeLotteryAsync(99)).ReturnsAsync(result);

        var dto = await _service.MakeLotteryAsync(99);

        Assert.NotNull(dto);
        Assert.Equal(99, dto.PresentId);
    }
}

[Trait("Category", "Unit")]
public class TokenServiceTests
{
    [Fact]
    public void GenerateToken_WhenValidInput_ReturnsJwtToken()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JwtSettings:SecretKey"] = "ThisIsAVeryLongAndSecureSecretKey123456",
                ["JwtSettings:Issuer"] = "store-api",
                ["JwtSettings:Audience"] = "store-clients",
                ["JwtSettings:ExpiryMinutes"] = "60"
            })
            .Build();
        var logger = new Mock<ILogger<TokenService>>();
        var service = new TokenService(config, logger.Object);

        var token = service.GenerateToken(42, "user@test.com", "Jane", "Doe", true);

        Assert.False(string.IsNullOrWhiteSpace(token));
    }

    [Fact]
    public void GenerateToken_WhenSecretKeyMissing_ThrowsInvalidOperationException()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JwtSettings:Issuer"] = "store-api",
                ["JwtSettings:Audience"] = "store-clients",
                ["JwtSettings:ExpiryMinutes"] = "60"
            })
            .Build();
        var logger = new Mock<ILogger<TokenService>>();
        var service = new TokenService(config, logger.Object);

        Assert.Throws<InvalidOperationException>(() => service.GenerateToken(42, "user@test.com", "Jane", "Doe", true));
    }
}

