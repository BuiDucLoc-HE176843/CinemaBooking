using CinemaBooking.Repositories.Implementations;
using CinemaBooking.Repositories.Interfaces;
using CinemaBooking.Services.Implementations;
using CinemaBooking.Services.Interfaces;

namespace CinemaBooking.Configuration
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddProjectServices(this IServiceCollection services)
        {
            // ===== Repositories =====
            //services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITheaterRepository, TheaterRepository>();
            services.AddScoped<IRoomRepository, RoomRepository>();
            services.AddScoped<IMovieRepository, MovieRepository>();

            // ===== Services =====
            //services.AddScoped<IUserService, UserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ITheaterService, TheaterService>();
            services.AddScoped<IRoomService, RoomService>();
            services.AddScoped<IMovieService, MovieService>();

            return services;
        }
    }
}
