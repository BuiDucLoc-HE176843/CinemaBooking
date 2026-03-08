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

        // DbSet
        public DbSet<User> Users { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Theater> Theaters { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Showtime> Showtimes { get; set; }
        public DbSet<ShowtimeSeat> ShowtimeSeats { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<BookingSeat> BookingSeats { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<MovieGenre> MovieGenres { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //------------------------------------
            // BOOKING SEAT
            //------------------------------------

            modelBuilder.Entity<BookingSeat>()
                .HasKey(bs => new { bs.BookingId, bs.ShowtimeSeatId });

            modelBuilder.Entity<BookingSeat>()
                .HasOne(bs => bs.Booking)
                .WithMany(b => b.BookingSeats)
                .HasForeignKey(bs => bs.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BookingSeat>()
                .HasOne(bs => bs.ShowtimeSeat)
                .WithMany(ss => ss.BookingSeats)
                .HasForeignKey(bs => bs.ShowtimeSeatId)
                .OnDelete(DeleteBehavior.Restrict);

            //------------------------------------
            // SHOWTIME SEAT
            //------------------------------------

            modelBuilder.Entity<ShowtimeSeat>()
                .HasOne(ss => ss.Showtime)
                .WithMany(s => s.ShowtimeSeats)
                .HasForeignKey(ss => ss.ShowtimeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<ShowtimeSeat>()
                .HasOne(ss => ss.Seat)
                .WithMany(s => s.ShowtimeSeats)
                .HasForeignKey(ss => ss.SeatId)
                .OnDelete(DeleteBehavior.Restrict);

            // tránh duplicate seat trong showtime
            modelBuilder.Entity<ShowtimeSeat>()
                .HasIndex(ss => new { ss.ShowtimeId, ss.SeatId })
                .IsUnique();

            //------------------------------------
            // USER RELATIONS
            //------------------------------------

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            //------------------------------------
            // MOVIE RELATIONS
            //------------------------------------

            modelBuilder.Entity<Showtime>()
                .HasOne(s => s.Movie)
                .WithMany(m => m.Showtimes)
                .HasForeignKey(s => s.MovieId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Movie)
                .WithMany(m => m.Reviews)
                .HasForeignKey(r => r.MovieId)
                .OnDelete(DeleteBehavior.Restrict);

            //------------------------------------
            // ROOM RELATIONS
            //------------------------------------

            modelBuilder.Entity<Room>()
                .HasOne(r => r.Theater)
                .WithMany(t => t.Rooms)
                .HasForeignKey(r => r.TheaterId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Seat>()
                .HasOne(s => s.Room)
                .WithMany(r => r.Seats)
                .HasForeignKey(s => s.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Showtime>()
                .HasOne(s => s.Room)
                .WithMany(r => r.Showtimes)
                .HasForeignKey(s => s.RoomId)
                .OnDelete(DeleteBehavior.Restrict);

            //------------------------------------
            // SHOWTIME → BOOKING
            //------------------------------------

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Showtime)
                .WithMany(s => s.Bookings)
                .HasForeignKey(b => b.ShowtimeId)
                .OnDelete(DeleteBehavior.Restrict);

            //------------------------------------
            // CITY → THEATER
            //------------------------------------

            modelBuilder.Entity<Theater>()
                .HasOne(t => t.City)
                .WithMany(c => c.Theaters)
                .HasForeignKey(t => t.CityId)
                .OnDelete(DeleteBehavior.Restrict);

            //------------------------------------
            // MOVIE ↔ GENRE (Many-to-many)
            //------------------------------------

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

            //------------------------------------
            // INDEXES
            //------------------------------------

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
