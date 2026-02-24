using CinemaBooking.Models;

namespace CinemaBooking.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(int id);
        Task UpdateAsync(User user);
    }
}
