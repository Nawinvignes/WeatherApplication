using Weather.Domain;

namespace Weather.Application;

public interface IUserService
{
    Task<User> Register(User user);
    Task<User?> GetByEmail(string email);
}

public class UserService : IUserService
{
    private readonly List<User> _users = new();

    public Task<User> Register(User user)
    {
        _users.Add(user);
        return Task.FromResult(user);
    }

    public Task<User?> GetByEmail(string email)
    {
        var user = _users.FirstOrDefault(x => x.Email == email);
        return Task.FromResult(user);
    }
}