using Weather.Domain;

namespace Weather.Domain.Interfaces;

public interface IUserRepository
{
    Task<User> AddUser(User user);
    Task<User?> GetByEmail(string email);
}