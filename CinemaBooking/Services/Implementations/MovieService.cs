using CinemaBooking.Configuration;
using CinemaBooking.DTOs.Requests;
using CinemaBooking.DTOs.Responses;
using CinemaBooking.Enums;
using CinemaBooking.Repositories.Interfaces;
using CinemaBooking.Services.Interfaces;

namespace CinemaBooking.Services.Implementations
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _movieRepository;

        public MovieService(IMovieRepository movieRepository)
        {
            _movieRepository = movieRepository;
        }

        public async Task<PagedResult<MovieResponse>> GetMoviesAsync(MovieFilterRequest request)
        {
            MovieStatus? statusEnum = null;

            // Validate Status
            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                if (!Enum.TryParse<MovieStatus>(request.Status, true, out var parsedStatus))
                    throw new AppException("Status không hợp lệ");

                statusEnum = parsedStatus;
            }

            // Validate GenreId
            if (request.GenreId.HasValue)
            {
                var exists = await _movieRepository.GenreExistsAsync(request.GenreId.Value);

                if (!exists)
                    throw new AppException("GenreId không tồn tại");
            }

            var result = await _movieRepository.GetPagedAsync(
                request.Title,
                request.ReleaseDate,
                statusEnum,
                request.GenreId,
                request.IsMainFeature,
                request.PageNumber,
                request.PageSize);

            return new PagedResult<MovieResponse>
            {
                Items = result.Items.Select(x => new MovieResponse
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    ReleaseDate = x.ReleaseDate,
                    DurationMinutes = x.DurationMinutes,
                    Status = x.Status,
                    PosterUrl = x.PosterUrl,
                    TrailerUrl = x.TrailerUrl,
                    Genres = x.MovieGenres!
                       .Select(g => g.Genre!.Name)
                       .ToList()
                }),
                PageNumber = result.PageNumber,
                PageSize = result.PageSize,
                TotalCount = result.TotalCount,
                TotalPages = result.TotalPages
            };
        }
    }
}
