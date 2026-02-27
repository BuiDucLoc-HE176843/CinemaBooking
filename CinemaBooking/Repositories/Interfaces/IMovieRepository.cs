using CinemaBooking.Configuration;
using CinemaBooking.Enums;
using CinemaBooking.Models;

namespace CinemaBooking.Repositories.Interfaces
{
    public interface IMovieRepository
    {
        Task<PagedResult<Movie>> GetPagedAsync(
        string? title,
        DateTime? releaseDate,
        MovieStatus? status,
        int? genreId,
        bool? IsMainFeature,
        int pageNumber,
        int pageSize);

        Task<bool> GenreExistsAsync(int genreId);
    }
}
