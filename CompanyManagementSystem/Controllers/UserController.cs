using Microsoft.AspNetCore.Mvc;
using CompanyManagementSystem.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using CompanyManagementSystem.Data;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

namespace CompanyManagementSystem.Models
{
    public class RegisterModel
    {
        [JsonPropertyName("username")]
        public required string Username { get; set; }
        
        [JsonPropertyName("password")]
        public required string Password { get; set; }

        [JsonPropertyName("firstName")]
        public string FirstName { get; set; } = string.Empty;

        [JsonPropertyName("lastName")]
        public string LastName { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;
    }
}

namespace CompanyManagementSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserController> _logger;

        public UserController(AppDbContext context, IConfiguration configuration, ILogger<UserController> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            try
            {
                _logger.LogInformation("Attempting to fetch users");
                var users = await _context.Users
                    .Select(u => new User
                    {
                        Id = u.Id,
                        Username = u.Username,
                        FirstName = u.FirstName,
                        LastName = u.LastName,
                        Email = u.Email,
                        PasswordHash = u.PasswordHash,
                        PurchaseOrders = new List<PurchaseOrder>(),
                        Companies = new List<Company>()
                    })
                    .ToListAsync();

                _logger.LogInformation("Successfully fetched {Count} users", users.Count);
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users");
                return StatusCode(500, new { message = "An error occurred while retrieving users" });
            }
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginModel model)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Username == model.Username);

                if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                {
                    _logger.LogWarning("Login failed for username: {Username}", model.Username);
                    return Unauthorized(new { message = "Invalid username or password" });
                }

                var token = GenerateJwtToken(user);
                _logger.LogInformation("User {Username} logged in successfully", user.Username);

                return Ok(new
                {
                    token,
                    user = new
                    {
                        id = user.Id,
                        username = user.Username,
                        firstName = user.FirstName,
                        lastName = user.LastName,
                        email = user.Email
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for {Username}", model.Username);
                return StatusCode(500, new { message = "An error occurred while logging in" });
            }
        }

        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not found")));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer not found"),
                audience: _configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT Audience not found"),
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private bool VerifyPassword(string enteredPassword, string storedPasswordHash)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(enteredPassword, storedPasswordHash);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error verifying password");
                return false;
            }
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterModel model)
        {
            try
            {
                if (_context.Users.Any(u => u.Username == model.Username))
                {
                    _logger.LogWarning("Registration failed: Username {Username} already exists", model.Username);
                    return BadRequest(new { message = "Username already exists" });
                }

                var user = new User
                {
                    Username = model.Username,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email
                };

                _context.Users.Add(user);
                _context.SaveChanges();

                _logger.LogInformation("User {Username} registered successfully", model.Username);
                return Ok(new { message = "User registered successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration for {Username}", model.Username);
                return StatusCode(500, new { message = "An error occurred while registering the user" });
            }
        }
    }

    public class LoginModel
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
    }

    public class RegisterModel
    {
        public required string Username { get; set; }
        public required string Password { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}