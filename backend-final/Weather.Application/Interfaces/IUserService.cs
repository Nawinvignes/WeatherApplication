using Weather.Domain;

namespace Weather.Application.Interfaces;

public interface IUserService
{
    Task<User> Register(User user);
    Task<User?> GetByEmail(string email);
}