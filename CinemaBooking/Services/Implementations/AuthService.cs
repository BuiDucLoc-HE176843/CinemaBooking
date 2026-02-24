using CinemaBooking.Configuration;
using CinemaBooking.DTOs.Requests;
using CinemaBooking.DTOs.Responses;
using CinemaBooking.Models;
using CinemaBooking.Repositories.Interfaces;
using CinemaBooking.Services.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CinemaBooking.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly JwtSettings _jwtSettings;

        public AuthService(
            IUserRepository userRepository,
            IOptions<JwtSettings> jwtOptions)
        {
            _userRepository = userRepository;
            _jwtSettings = jwtOptions.Value;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                throw new AppException("Email không được để trống");

            if (string.IsNullOrWhiteSpace(request.Password))
                throw new AppException("Mật khẩu không được để trống");

            var user = await _userRepository.GetByEmailAsync(request.Email);

            if (user == null)
                throw new AppException("Tài khoản không tồn tại");

            if (user.PasswordHash != request.Password)
                throw new AppException("Email hoặc mật khẩu không chính xác");

            var token = GenerateJwtToken(user);

            return new LoginResponse
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Token = token
            };
        }

        // 🔥 Viết lại theo model User của bạn
        private string GenerateJwtToken(User user)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtSettings.Secret));

            var creds = new SigningCredentials(
                key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Username)
        };

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task ChangePasswordAsync(int userId, ChangePasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.OldPassword))
                throw new AppException("Mật khẩu cũ không được để trống");

            if (string.IsNullOrWhiteSpace(request.NewPassword))
                throw new AppException("Mật khẩu mới không được để trống");

            if (string.IsNullOrWhiteSpace(request.ConfirmNewPassword))
                throw new AppException("Xác nhận mật khẩu không được để trống");

            if (request.NewPassword != request.ConfirmNewPassword)
                throw new AppException("Mật khẩu mới và xác nhận mật khẩu không khớp");

            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
                throw new AppException("Người dùng không tồn tại");

            if (user.PasswordHash != request.OldPassword)
                throw new AppException("Mật khẩu cũ không chính xác");

            user.PasswordHash = request.NewPassword;

            await _userRepository.UpdateAsync(user);
        }
    }
}
