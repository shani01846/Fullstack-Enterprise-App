using a.Dto;
using a.Interfaces;
using NET.Dto;
using NET.Models;
using StoreApi.Interfaces;

namespace a.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IDonorRepository _donorRepository;
        private readonly ITokenService _tokenService;
        private readonly IConfiguration _configuration;
        private readonly ILogger< UserService>_logger;

        public UserService(
            IUserRepository userRepository,
            ITokenService tokenService,IDonorRepository donorRepository,
            IConfiguration configuration,
            ILogger< UserService> logger)
        {
            _configuration = configuration;
            _userRepository = userRepository;
            _tokenService = tokenService;
            _logger = logger;
            _donorRepository = donorRepository;
        }
        public async Task<LoginResponeDto?> AuthenticateAsync(string email, string password)
        {
          
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
            {
                _logger.LogWarning("User Loggin attemp was failed:User not found for email {Email}", email);
                return null;
            }

            var hashedPassword = HashPassword(password);
            if(user.Password!=hashedPassword)
            {
                _logger.LogWarning("Login attempt failed: Invalid password for email {Email}", email);
                return null;
            }

            var token = _tokenService.GenerateToken(user.Id, user.Email, user.FirstName, user.LastName,user.IsManager);
            var expiryMinutes = _configuration.GetValue<int>("jwtSettings:ExpiryMinutes", 60);
            _logger.LogInformation("User {UserId} authenticated successfully", user.Id);

            return new LoginResponeDto
            {
                Token = token,
                TokenType = "Bearer",
                ExpiresIn = expiryMinutes * 60,
                User = MapToResponseDto(user)
            };
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto createDto)
        {
            if( await _userRepository.EmailExistsAsync(createDto.Email) )
            {
                throw new ArgumentException($"Email {createDto.Email} have been already exists");

            }

            var user = new User
            {
                Email = createDto.Email,
                FirstName = createDto.FirstName,
                LastName = createDto.LastName,
                Phone = createDto.Phone,
                Password = HashPassword(createDto.Password),
                Address = createDto.Address
            };

            var createdUser =await _userRepository.CreateAsync(user);
            _logger.LogInformation($"Created user: {createdUser.Id}");
            return MapToResponseDto(user);

        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            return await _userRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.Select(MapToResponseDto);

        }

        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var user = await _userRepository.GetByIdAsync(id);
            return user!=null ? MapToResponseDto(user) : null;
        }

        public async Task<UserDto?> UpdateUserAsync(int id, CreateUserDto updateDto)
        {
            var existingUser = await _userRepository.GetByIdAsync(id);
            if (existingUser == null || updateDto == null) return null;
            if (updateDto.Email != existingUser.Email)
            {
                if(await _userRepository.EmailExistsAsync(updateDto.Email))
                {
                    throw new ArgumentException($"Email {updateDto.Email} have been alresy exists");
                }

                existingUser.Email = updateDto.Email;
            }
            if (updateDto.FirstName != null) existingUser.FirstName = updateDto.FirstName;
            if (updateDto.LastName != null) existingUser.LastName = updateDto.LastName;
            if (updateDto.Address != null) existingUser.Address = updateDto.Address;
            if (updateDto.Phone != null) existingUser.Phone = updateDto.Phone;

            var updatedUser = await _userRepository.UpdateAsync(existingUser);
            return updatedUser != null ? MapToResponseDto(updatedUser) : null;


        }
        private static string HashPassword(string password)
        {
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));
        }
        private static UserDto MapToResponseDto(User user)
        {
            return new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Phone = user.Phone,
                Address = user.Address
            };
        }

    }
}
