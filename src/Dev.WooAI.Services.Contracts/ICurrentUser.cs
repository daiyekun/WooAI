namespace Dev.WooAI.Services.Contracts;

public interface ICurrentUser
{
    string? Id { get; }
    
    string? UserName { get; }
    
    string? Role { get; }

    bool IsAuthenticated { get; }
}