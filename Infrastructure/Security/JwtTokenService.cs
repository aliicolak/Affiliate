using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Domain.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Infrastructure.Security
{
    public interface IJwtTokenService
    {
        Task<(string accessToken, DateTime expires)> CreateAccessTokenAsync(ApplicationUser user);
        Task<string> CreateRefreshTokenAsync();
        Task SaveRefreshTokenAsync(long userId, string refreshToken, DateTime expiresUtc);
        Task<(string? token, DateTime? expiresUtc)> GetRefreshTokenAsync(long userId);
        Task RevokeRefreshTokenAsync(long userId);
    }

    public sealed class JwtTokenService : IJwtTokenService
    {
        private const string Provider = "JWT";
        private const string RefreshName = "RefreshToken";
        private const string RefreshExpName = "RefreshTokenExpires";

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtOptions _options;

        public JwtTokenService(UserManager<ApplicationUser> userManager, IOptions<JwtOptions> options)
        {
            _userManager = userManager;
            _options = options.Value;
        }

        public async Task<(string accessToken, DateTime expires)> CreateAccessTokenAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new(JwtRegisteredClaimNames.UniqueName, user.UserName ?? ""),
                new(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new("displayName", user.DisplayName)
            };
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.UtcNow.AddMinutes(_options.AccessTokenMinutes);

            var token = new JwtSecurityToken(
                issuer: _options.Issuer,
                audience: _options.Audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expires,
                signingCredentials: creds);

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);
            return (accessToken, expires);
        }

        public Task<string> CreateRefreshTokenAsync()
        {
            // 256-bit random
            var bytes = RandomNumberGenerator.GetBytes(32);
            return Task.FromResult(Convert.ToBase64String(bytes));
        }

        public async Task SaveRefreshTokenAsync(long userId, string refreshToken, DateTime expiresUtc)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                throw new InvalidOperationException("User not found for refresh token save.");

            await _userManager.SetAuthenticationTokenAsync(user, Provider, RefreshName, refreshToken);
            await _userManager.SetAuthenticationTokenAsync(user, Provider, RefreshExpName, expiresUtc.ToString("O"));
        }

        public async Task<(string? token, DateTime? expiresUtc)> GetRefreshTokenAsync(long userId)
        {
            var token = await _userManager.GetAuthenticationTokenAsync(
                new ApplicationUser { Id = userId }, Provider, RefreshName);

            var expStr = await _userManager.GetAuthenticationTokenAsync(
                new ApplicationUser { Id = userId }, Provider, RefreshExpName);

            DateTime? exp = null;
            if (DateTime.TryParse(expStr, null, System.Globalization.DateTimeStyles.RoundtripKind, out var parsed))
                exp = parsed;

            return (token, exp);
        }

        public async Task RevokeRefreshTokenAsync(long userId)
        {
            await _userManager.RemoveAuthenticationTokenAsync(new ApplicationUser { Id = userId }, Provider, RefreshName);
            await _userManager.RemoveAuthenticationTokenAsync(new ApplicationUser { Id = userId }, Provider, RefreshExpName);
        }
    }
}
