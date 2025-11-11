namespace API.Contracts
{
    public record RegisterRequest(string Email, string Password, string DisplayName, string? UserName);
    public record LoginRequest(string UserNameOrEmail, string Password);
    public record RefreshRequest(string RefreshToken);

    public record AuthResponse(string AccessToken, DateTime ExpiresUtc, string RefreshToken);
    public record MeResponse(long Id, string DisplayName, string? Email, IEnumerable<string> Roles);
}
