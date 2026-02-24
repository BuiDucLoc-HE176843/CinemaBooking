using CinemaBooking.Enums;
using CinemaBooking.Models;
using Microsoft.EntityFrameworkCore;

namespace CinemaBooking.Data
{
    public static class UserSeeder
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            if (await context.Users.AnyAsync())
                return; // đã có dữ liệu thì không seed nữa

            var users = new List<User>
        {
            new User
            {
                Username = "admin",
                PasswordHash = "1",
                Email = "admin@gmail.com",
                FullName = "System Admin",
                Phone = "0900000001",
                Role = UserRole.Admin,
                CreatedAt = DateTime.UtcNow
            },

            new User
            {
                Username = "user1",
                PasswordHash = "1",
                Email = "user1@gmail.com",
                FullName = "Nguyễn Văn A",
                Phone = "0900000002",
                Role = UserRole.User,
                CreatedAt = DateTime.UtcNow
            },

            new User
            {
                Username = "user2",
                PasswordHash = "1",
                Email = "user2@gmail.com",
                FullName = "Trần Thị B",
                Phone = "0900000003",
                Role = UserRole.User,
                CreatedAt = DateTime.UtcNow
            },

            new User
            {
                Username = "user3",
                PasswordHash = "1",
                Email = "user3@gmail.com",
                FullName = "Lê Văn C",
                Phone = "0900000004",
                Role = UserRole.User,
                CreatedAt = DateTime.UtcNow
            }
        };

            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();
        }
    }
}
