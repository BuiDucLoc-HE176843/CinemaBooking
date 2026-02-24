using CinemaBooking.Models;
using Microsoft.EntityFrameworkCore;

namespace CinemaBooking.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet cho từng entity
        public DbSet<User> Users { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Theater> Theaters { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Showtime> Showtimes { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingSeat> BookingSeats { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<MovieGenre> MovieGenres { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Cấu hình composite key cho bảng trung gian BookingSeat
            modelBuilder.Entity<BookingSeat>()
                .HasKey(bs => new { bs.BookingId, bs.SeatId });

            // 2. Cấu hình mối quan hệ Booking ↔ BookingSeat
            modelBuilder.Entity<BookingSeat>()
                .HasOne(bs => bs.Booking)
                .WithMany(b => b.BookingSeats)
                .HasForeignKey(bs => bs.BookingId)
                .OnDelete(DeleteBehavior.Cascade);  // khi xóa booking thì xóa luôn các bookingseat

            modelBuilder.Entity<BookingSeat>()
                .HasOne(bs => bs.Seat)
                .WithMany(s => s.BookingSeats)
                .HasForeignKey(bs => bs.SeatId)
                .OnDelete(DeleteBehavior.Restrict);  // không cho xóa ghế nếu đang có booking

            // 3. Cấu hình các mối quan hệ khác (nếu EF Core chưa tự infer đúng)

            // User → Bookings
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // User → Reviews
            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // Movie → Showtimes
            modelBuilder.Entity<Showtime>()
                .HasOne(s => s.Movie)
                .WithMany(m => m.Showtimes)
                .HasForeignKey(s => s.MovieId)
                .OnDelete(DeleteBehavior.Restrict);

            // Movie → Reviews
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Movie)
                .WithMany(m => m.Reviews)
                .HasForeignKey(r => r.MovieId)
                .OnDelete(DeleteBehavior.Restrict);

            // Theater → Rooms
            modelBuilder.Entity<Room>()
                .HasOne(r => r.Theater)
                .WithMany(t => t.Rooms)
                .HasForeignKey(r => r.TheaterId)
                .OnDelete(DeleteBehavior.Cascade);

            // Room → Seats
            modelBuilder.Entity<Seat>()
                .HasOne(s => s.Room)
                .WithMany(r => r.Seats)
                .HasForeignKey(s => s.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            // Room → Showtimes
            modelBuilder.Entity<Showtime>()
                .HasOne(s => s.Room)
                .WithMany(r => r.Showtimes)
                .HasForeignKey(s => s.RoomId)
                .OnDelete(DeleteBehavior.Restrict);

            // Showtime → Bookings
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Showtime)
                .WithMany(s => s.Bookings)
                .HasForeignKey(b => b.ShowtimeId)
                .OnDelete(DeleteBehavior.Restrict);

            // City → Theater
            modelBuilder.Entity<Theater>()
                .HasOne(t => t.City)
                .WithMany(c => c.Theaters)
                .HasForeignKey(t => t.CityId)
                .OnDelete(DeleteBehavior.Restrict);

            // Many-to-many Movie ↔ Genre qua MovieGenre
            modelBuilder.Entity<MovieGenre>()
                .HasKey(mg => new { mg.MovieId, mg.GenreId });

            modelBuilder.Entity<MovieGenre>()
                .HasOne(mg => mg.Movie)
                .WithMany(m => m.MovieGenres)
                .HasForeignKey(mg => mg.MovieId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MovieGenre>()
                .HasOne(mg => mg.Genre)
                .WithMany(g => g.MovieGenres)
                .HasForeignKey(mg => mg.GenreId)
                .OnDelete(DeleteBehavior.Cascade);

            // (Các enum sẽ được map tự động thành int trong database)

            // Optional: Index để tăng tốc truy vấn phổ biến
            modelBuilder.Entity<Showtime>()
                .HasIndex(s => s.ShowDateTime);

            modelBuilder.Entity<Seat>()
                .HasIndex(s => new { s.RoomId, s.RowNumber, s.ColumnNumber })
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<City>()
                .HasIndex(c => c.Name)
                .IsUnique();

            modelBuilder.Entity<Genre>()
                .HasIndex(g => g.Name)
                .IsUnique();
        }
    }
}
