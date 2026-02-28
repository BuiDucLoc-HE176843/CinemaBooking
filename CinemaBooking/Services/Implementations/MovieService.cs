using CinemaBooking.Configuration;
using CinemaBooking.DTOs.Requests;
using CinemaBooking.DTOs.Responses;
using CinemaBooking.Enums;
using CinemaBooking.Models;
using CinemaBooking.Repositories.Interfaces;
using CinemaBooking.Services.Interfaces;

namespace CinemaBooking.Services.Implementations
{
    public class MovieService : IMovieService
    {
        private readonly IMovieRepository _movieRepository;
        private readonly IWebHostEnvironment _environment;

        public MovieService(IMovieRepository movieRepository, IWebHostEnvironment environment)
        {
            _movieRepository = movieRepository;
            _environment = environment;
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

            // Validate Id
            if (request.Id.HasValue)
            {
                var exists = await _movieRepository.IdExistsAsync(request.Id.Value);

                if (!exists)
                    throw new AppException("Id không tồn tại");
            }

            // Validate GenreId
            if (request.GenreId.HasValue)
            {
                var exists = await _movieRepository.GenreExistsAsync(request.GenreId.Value);

                if (!exists)
                    throw new AppException("GenreId không tồn tại");
            }

            var result = await _movieRepository.GetPagedAsync(
                request.Id,
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

        public async Task UpdateMovieAsync(int id, UpdateMovieRequest request)
        {
            var movie = await _movieRepository.GetByIdWithGenresAsync(id);

            if (movie == null)
                throw new AppException("Phim không tồn tại");

            // Validate Status
            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                if (!Enum.TryParse<MovieStatus>(request.Status, true, out var parsedStatus))
                    throw new AppException("Status không hợp lệ");

                movie.Status = parsedStatus;
            }

            // Validate GenreIds
            if (request.GenreIds != null && request.GenreIds.Any())
            {
                var valid = await _movieRepository.GenresExistAsync(request.GenreIds);

                if (!valid)
                    throw new AppException("Một hoặc nhiều GenreId không tồn tại");

                // Clear genre cũ
                movie.MovieGenres!.Clear();

                // Add lại
                movie.MovieGenres = request.GenreIds
                    .Select(gid => new MovieGenre
                    {
                        MovieId = movie.Id,
                        GenreId = gid
                    })
                    .ToList();
            }

            // Update field cơ bản
            movie.Title = request.Title;
            movie.Description = request.Description;
            movie.ReleaseDate = request.ReleaseDate;
            movie.DurationMinutes = request.DurationMinutes;
            // Update Poster
            if (request.PosterFile != null)
            {
                // Xóa file cũ
                DeleteFile(movie.PosterUrl);

                // Lưu file mới
                var posterPath = await SaveFileAsync(
                    request.PosterFile,
                    "Posters");

                movie.PosterUrl = posterPath;
            }

            // Update Trailer
            if (request.TrailerFile != null)
            {
                DeleteFile(movie.TrailerUrl);

                var trailerPath = await SaveFileAsync(
                    request.TrailerFile,
                    "Trailers");

                movie.TrailerUrl = trailerPath;
            }
            movie.UpdatedAt = DateTime.UtcNow;

            await _movieRepository.UpdateAsync(movie);
        }

        private async Task<string> SaveFileAsync(IFormFile file, string folderName)
        {
            var uploadsFolder = Path.Combine(
                _environment.WebRootPath,
                folderName);

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";

            var filePath = Path.Combine(uploadsFolder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return $"/{folderName}/{fileName}";
        }

        private void DeleteFile(string? relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
                return;

            var fullPath = Path.Combine(
                _environment.WebRootPath,
                relativePath.TrimStart('/'));

            if (File.Exists(fullPath))
                File.Delete(fullPath);
        }
    }
}
