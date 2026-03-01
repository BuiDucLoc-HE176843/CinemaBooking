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
                    IsMainFeature = x.IsMainFeature,
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
                throw new AppException("Movie không tồn tại");

            // ===== Update basic fields (nếu có truyền) =====

            if (!string.IsNullOrWhiteSpace(request.Title))
                movie.Title = request.Title;

            if (request.Description != null)
                movie.Description = request.Description;

            if (request.ReleaseDate.HasValue)
                movie.ReleaseDate = request.ReleaseDate;

            if (request.DurationMinutes.HasValue && request.DurationMinutes > 0)
                movie.DurationMinutes = request.DurationMinutes ?? movie.DurationMinutes;

            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                if (!Enum.TryParse<MovieStatus>(request.Status, true, out var parsedStatus))
                    throw new AppException("Status không hợp lệ");

                movie.Status = parsedStatus;
            }

            // ===== Upload Poster =====
            if (request.PosterFile != null)
            {
                var posterFolder = Path.Combine(_environment.WebRootPath, "Poster");

                if (!Directory.Exists(posterFolder))
                    Directory.CreateDirectory(posterFolder);

                // Xóa file cũ
                if (!string.IsNullOrEmpty(movie.PosterUrl))
                {
                    var oldPosterPath = Path.Combine(_environment.WebRootPath, movie.PosterUrl.TrimStart('/'));
                    if (File.Exists(oldPosterPath))
                        File.Delete(oldPosterPath);
                }

                var fileName = $"{Guid.NewGuid()}_{request.PosterFile.FileName}";
                var filePath = Path.Combine(posterFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.PosterFile.CopyToAsync(stream);
                }

                movie.PosterUrl = $"/Poster/{fileName}";
            }

            // ===== Upload Trailer =====
            if (request.TrailerFile != null)
            {
                var trailerFolder = Path.Combine(_environment.WebRootPath, "Trailer");

                if (!Directory.Exists(trailerFolder))
                    Directory.CreateDirectory(trailerFolder);

                // Xóa file cũ
                if (!string.IsNullOrEmpty(movie.TrailerUrl))
                {
                    var oldTrailerPath = Path.Combine(_environment.WebRootPath, movie.TrailerUrl.TrimStart('/'));
                    if (File.Exists(oldTrailerPath))
                        File.Delete(oldTrailerPath);
                }

                var fileName = $"{Guid.NewGuid()}_{request.TrailerFile.FileName}";
                var filePath = Path.Combine(trailerFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.TrailerFile.CopyToAsync(stream);
                }

                movie.TrailerUrl = $"/Trailer/{fileName}";
            }

            // ===== Update Genres =====
            if (request.GenreIds != null && request.GenreIds.Any())
            {
                var valid = await _movieRepository.AllGenresExistAsync(request.GenreIds);

                if (!valid)
                    throw new AppException("Có GenreId không tồn tại");

                // Xóa cũ
                movie.MovieGenres!.Clear();

                // Thêm mới
                movie.MovieGenres = request.GenreIds
                    .Select(gid => new MovieGenre
                    {
                        MovieId = movie.Id,
                        GenreId = gid
                    }).ToList();
            }

            movie.UpdatedAt = DateTime.UtcNow;

            // ===== Handle IsMainFeature =====
            if (request.IsMainFeature == true && movie.IsMainFeature == false)
            {
                var currentMainMovies = await _movieRepository.GetMainFeatureMoviesAsync();

                foreach (var m in currentMainMovies)
                {
                    m.IsMainFeature = false;
                }

                movie.IsMainFeature = true;
            }

            await _movieRepository.UpdateAsync(movie);
        }

        public async Task AddMovieAsync(CreateMovieRequest request)
        {
            // ===== Validate Status =====
            MovieStatus statusEnum = MovieStatus.Upcoming;

            if (!string.IsNullOrWhiteSpace(request.Status))
            {
                if (!Enum.TryParse<MovieStatus>(request.Status, true, out var parsedStatus))
                    throw new AppException("Status không hợp lệ");

                statusEnum = parsedStatus;
            }

            // ===== Validate Genre =====
            if (request.GenreIds != null && request.GenreIds.Any())
            {
                var valid = await _movieRepository.AllGenresExistAsync(request.GenreIds);

                if (!valid)
                    throw new AppException("Có GenreId không tồn tại");
            }

            var movie = new Movie
            {
                Title = request.Title,
                Description = request.Description,
                ReleaseDate = request.ReleaseDate,
                DurationMinutes = request.DurationMinutes,
                Status = statusEnum,
                IsMainFeature = request.IsMainFeature,
                CreatedAt = DateTime.UtcNow
            };

            // ===== Upload Poster =====
            if (request.PosterFile != null)
            {
                var posterFolder = Path.Combine(_environment.WebRootPath, "Poster");

                if (!Directory.Exists(posterFolder))
                    Directory.CreateDirectory(posterFolder);

                var fileName = $"{Guid.NewGuid()}_{request.PosterFile.FileName}";
                var filePath = Path.Combine(posterFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.PosterFile.CopyToAsync(stream);
                }

                movie.PosterUrl = $"/Poster/{fileName}";
            }

            // ===== Upload Trailer =====
            if (request.TrailerFile != null)
            {
                var trailerFolder = Path.Combine(_environment.WebRootPath, "Trailer");

                if (!Directory.Exists(trailerFolder))
                    Directory.CreateDirectory(trailerFolder);

                var fileName = $"{Guid.NewGuid()}_{request.TrailerFile.FileName}";
                var filePath = Path.Combine(trailerFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await request.TrailerFile.CopyToAsync(stream);
                }

                movie.TrailerUrl = $"/Trailer/{fileName}";
            }

            // ===== Add Genres =====
            if (request.GenreIds != null && request.GenreIds.Any())
            {
                movie.MovieGenres = request.GenreIds
                    .Select(gid => new MovieGenre
                    {
                        GenreId = gid
                    }).ToList();
            }

            // ===== Handle IsMainFeature =====
            if (request.IsMainFeature)
            {
                var currentMainMovies = await _movieRepository.GetMainFeatureMoviesAsync();

                foreach (var m in currentMainMovies)
                {
                    m.IsMainFeature = false;
                }
            }

            await _movieRepository.AddAsync(movie);
        }
    }
}
