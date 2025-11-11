using Application.Abstractions;

public sealed record CreateMerchantCommand(
    string Name,
    string WebsiteUrl
) : ICommand<long>;
