using API.Contracts;
using Domain.Entities.Identity;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IJwtTokenService _jwt;

        public AccountController(UserManager<ApplicationUser> userManager,
                                 SignInManager<ApplicationUser> signInManager,
                                 IJwtTokenService jwt)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwt = jwt;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        [EnableRateLimiting("AuthPolicy")]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest req)
        {
            var user = new ApplicationUser
            {
                Email = req.Email.Trim(),
                UserName = string.IsNullOrWhiteSpace(req.UserName) ? req.Email.Trim() : req.UserName.Trim(),
                DisplayName = req.DisplayName.Trim(),
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(user, req.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);

            await _userManager.AddToRoleAsync(user, "User");

            var (access, exp) = await _jwt.CreateAccessTokenAsync(user);
            var refresh = await _jwt.CreateRefreshTokenAsync();
            await _jwt.SaveRefreshTokenAsync(user.Id, refresh, DateTime.UtcNow.AddDays(14));

            return Ok(new AuthResponse(access, exp, refresh));
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [EnableRateLimiting("AuthPolicy")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest req)
        {
            // username veya email
            ApplicationUser? user =
                await _userManager.FindByNameAsync(req.UserNameOrEmail) ??
                await _userManager.FindByEmailAsync(req.UserNameOrEmail);

            if (user == null) return Unauthorized("Kullanıcı bulunamadı.");

            var pwd = await _signInManager.CheckPasswordSignInAsync(user, req.Password, lockoutOnFailure: true);
            if (!pwd.Succeeded) return Unauthorized("Kullanıcı adı / şifre hatalı.");

            var (access, exp) = await _jwt.CreateAccessTokenAsync(user);

            var refresh = await _jwt.CreateRefreshTokenAsync();
            await _jwt.SaveRefreshTokenAsync(user.Id, refresh, DateTime.UtcNow.AddDays(14));

            return Ok(new AuthResponse(access, exp, refresh));
        }

        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponse>> Refresh([FromBody] RefreshRequest req)
        {
            // Access token gönderilmeden sadece refresh ile çalışır.
            // Header’daki bearer’ı dikkate almıyoruz.
            // Refresh token UserTokens tablosunda saklı.

            // Kim bu kullanıcı? Refresh token tek başına user'ı söylemez; biz projede
            // refresh'i kullanıcıya bağlı tutuyoruz: client, refresh ile birlikte current userId claim'i
            // yoksa "401" dönebilirsin. Alternatif: tüm kullanıcılar taranmaz; pratik çözüm olarak
            // access süresi dolduysa client login çağrısı yaptığında refresh’i ve username/email’i gönderebilir.
            // Basit çözüm: header'da son kullanılan access token varsa ondan userId çekelim (opsiyonel).
            var bearer = HttpContext.Request.Headers.Authorization.ToString();
            long? userId = null;
            if (bearer?.StartsWith("Bearer ") == true)
            {
                var token = bearer.Substring("Bearer ".Length).Trim();
                var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
                if (handler.CanReadToken(token))
                {
                    var jwt = handler.ReadJwtToken(token);
                    var sub = jwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "sub")?.Value;
                    if (long.TryParse(sub, out var id)) userId = id;
                }
            }
            if (userId is null) return Unauthorized("Kullanıcı bilgisi bulunamadı.");

            var user = await _userManager.FindByIdAsync(userId.ToString()!);
            if (user is null) return Unauthorized();

            var (stored, expiresUtc) = await _jwt.GetRefreshTokenAsync(user.Id);
            if (string.IsNullOrWhiteSpace(stored) || stored != req.RefreshToken)
                return Unauthorized("Geçersiz refresh token.");

            if (expiresUtc is null || expiresUtc < DateTime.UtcNow)
                return Unauthorized("Refresh token süresi dolmuş.");

            // rotate refresh
            var (access, exp) = await _jwt.CreateAccessTokenAsync(user);
            var newRefresh = await _jwt.CreateRefreshTokenAsync();
            await _jwt.SaveRefreshTokenAsync(user.Id, newRefresh, DateTime.UtcNow.AddDays(14));

            return Ok(new AuthResponse(access, exp, newRefresh));
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _jwt.RevokeRefreshTokenAsync(userId);
            return NoContent();
        }

        [HttpGet("me")]
        [Authorize]
        public async Task<ActionResult<MeResponse>> Me()
        {
            var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            return new MeResponse(user.Id, user.DisplayName, user.Email, roles);
        }
    }
}
