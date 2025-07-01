using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using IMDB.Data;
using IMDB.Models;
using IMDB.DTOs;
using IMDB.Services;

namespace IMDB.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtService _jwtService;
        private readonly IFirebaseAuthService _firebaseAuthService;
        private readonly IFileUploadService _fileUploadService;
        private readonly IMapper _mapper;

        public AuthController(
            ApplicationDbContext context,
            IJwtService jwtService,
            IFirebaseAuthService firebaseAuthService,
            IFileUploadService fileUploadService,
            IMapper mapper)
        {
            _context = context;
            _jwtService = jwtService;
            _firebaseAuthService = firebaseAuthService;
            _fileUploadService = fileUploadService;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            try
            {
                // Check if user already exists
                var existingUser = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == registerDto.Email);

                if (existingUser != null)
                {
                    return BadRequest(new { message = "User with this email already exists" });
                }

                // Create new user
                var user = _mapper.Map<User>(registerDto);
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);
                user.CreatedAt = DateTime.UtcNow;

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                // Generate JWT token
                var token = _jwtService.GenerateToken(user);
                var userDto = _mapper.Map<UserDto>(user);

                return Ok(new AuthResponseDto
                {
                    Token = token,
                    User = userDto
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Registration failed", error = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

                if (user == null)
                {
                    return BadRequest(new { message = "Invalid email or password" });
                }

                // For Google authenticated users, they can't use regular login
                if (user.IsGoogleAuth && string.IsNullOrEmpty(user.PasswordHash))
                {
                    return BadRequest(new { message = "This account is linked to Google. Please use Google login." });
                }

                // Verify password
                if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
                {
                    return BadRequest(new { message = "Invalid email or password" });
                }

                // Generate JWT token
                var token = _jwtService.GenerateToken(user);
                var userDto = _mapper.Map<UserDto>(user);

                return Ok(new AuthResponseDto
                {
                    Token = token,
                    User = userDto
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Login failed", error = ex.Message });
            }
        }

        [HttpPost("google")]
        public async Task<IActionResult> GoogleAuth([FromBody] GoogleAuthDto googleAuthDto)
        {
            try
            {
                // Verify Firebase ID token
                var firebaseToken = await _firebaseAuthService.VerifyIdTokenAsync(googleAuthDto.IdToken);

                // Check if user exists
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.GoogleId == firebaseToken.Uid || u.Email == firebaseToken.Claims["email"].ToString());

                if (user == null)
                {
                    // Create new user from Google account using provided data
                    user = new User
                    {
                        Email = firebaseToken.Claims["email"].ToString()!,
                        FirstName = googleAuthDto.FirstName,
                        LastName = googleAuthDto.LastName,
                        Country = googleAuthDto.Country,
                        City = googleAuthDto.City,
                        ProfilePicture = googleAuthDto.ImageUrl,
                        GoogleId = firebaseToken.Uid,
                        IsGoogleAuth = true,
                        CreatedAt = DateTime.UtcNow
                    };

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    // Update Google ID if not set
                    if (string.IsNullOrEmpty(user.GoogleId))
                    {
                        user.GoogleId = firebaseToken.Uid;
                        user.IsGoogleAuth = true;
                        await _context.SaveChangesAsync();
                    }
                    
                    // Update user info if it's empty (for existing users)
                    bool needsUpdate = false;
                    if (string.IsNullOrEmpty(user.FirstName) && !string.IsNullOrEmpty(googleAuthDto.FirstName))
                    {
                        user.FirstName = googleAuthDto.FirstName;
                        needsUpdate = true;
                    }
                    if (string.IsNullOrEmpty(user.LastName) && !string.IsNullOrEmpty(googleAuthDto.LastName))
                    {
                        user.LastName = googleAuthDto.LastName;
                        needsUpdate = true;
                    }
                    if ((user.Country == "Unknown" || string.IsNullOrEmpty(user.Country)) && !string.IsNullOrEmpty(googleAuthDto.Country))
                    {
                        user.Country = googleAuthDto.Country;
                        needsUpdate = true;
                    }
                    if ((user.City == "Unknown" || string.IsNullOrEmpty(user.City)) && !string.IsNullOrEmpty(googleAuthDto.City))
                    {
                        user.City = googleAuthDto.City;
                        needsUpdate = true;
                    }
                    if (string.IsNullOrEmpty(user.ProfilePicture) && !string.IsNullOrEmpty(googleAuthDto.ImageUrl))
                    {
                        user.ProfilePicture = googleAuthDto.ImageUrl;
                        needsUpdate = true;
                    }
                    
                    if (needsUpdate)
                    {
                        await _context.SaveChangesAsync();
                    }
                }

                // Generate JWT token
                var token = _jwtService.GenerateToken(user);
                var userDto = _mapper.Map<UserDto>(user);

                return Ok(new AuthResponseDto
                {
                    Token = token,
                    User = userDto
                });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { message = "Invalid Google token" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Google authentication failed", error = ex.Message });
            }
        }

        [HttpPost("upload-profile-picture")]
        [Authorize]
        public async Task<IActionResult> UploadProfilePicture(IFormFile file)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    return Unauthorized(new { message = "Invalid user" });
                }

                var user = await _context.Users.FindAsync(userId);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                // Upload file to Supabase
                var profilePictureUrl = await _fileUploadService.UploadProfilePictureAsync(file, userId.ToString());

                // Update user's profile picture
                user.ProfilePicture = profilePictureUrl;
                user.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                var userDto = _mapper.Map<UserDto>(user);

                return Ok(new { message = "Profile picture uploaded successfully", user = userDto });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to upload profile picture", error = ex.Message });
            }
        }
    }
} 