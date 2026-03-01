using CinemaBooking.Configuration;
using CinemaBooking.Enums;
using CinemaBooking.Models;

namespace CinemaBooking.Repositories.Interfaces
{
    public interface IMovieRepository
    {
        Task<PagedResult<Movie>> GetPagedAsync(
        int? id,
        string? title,
        DateTime? releaseDate,
        MovieStatus? status,
        int? genreId,
        bool? IsMainFeature,
        int pageNumber,
        int pageSize);

        Task<bool> GenreExistsAsync(int genreId);
        Task<bool> IdExistsAsync(int genreId);

        Task<Movie?> GetByIdWithGenresAsync(int id);
        Task UpdateAsync(Movie movie);
        Task<bool> AllGenresExistAsync(List<int> genreIds);
        Task AddAsync(Movie movie);
        Task<List<Movie>> GetMainFeatureMoviesAsync();
    }
}
