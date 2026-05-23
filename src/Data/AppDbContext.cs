using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Melodify.Models.Entities;

namespace Melodify.Data
{
    public class AppDbContext : IdentityDbContext<User>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Artist> Artists { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Track> Tracks { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<PlaylistTrack> PlaylistTracks { get; set; }
        public DbSet<LikedTrack> LikedTracks { get; set; }
        public DbSet<FollowArtist> FollowedArtists { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PlaylistTrack>()
                .HasKey(pt => new { pt.PlaylistId, pt.TrackId });

            modelBuilder.Entity<PlaylistTrack>()
                .HasOne(pt => pt.Playlist)
                .WithMany(p => p.PlaylistTracks)
                .HasForeignKey(pt => pt.PlaylistId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PlaylistTrack>()
                .HasOne(pt => pt.Track)
                .WithMany()
                .HasForeignKey(pt => pt.TrackId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LikedTrack>()
                .HasKey(lt => new { lt.UserId, lt.TrackId });

            modelBuilder.Entity<LikedTrack>()
                .HasOne(lt => lt.User)
                .WithMany(u => u.LikedTracks)
                .HasForeignKey(lt => lt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<LikedTrack>()
                .HasOne(lt => lt.Track)
                .WithMany()
                .HasForeignKey(lt => lt.TrackId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FollowArtist>()
                .HasKey(fa => new { fa.UserId, fa.ArtistId });

            modelBuilder.Entity<FollowArtist>()
                .HasOne(fa => fa.User)
                .WithMany(u => u.FollowedArtists)
                .HasForeignKey(fa => fa.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FollowArtist>()
                .HasOne(fa => fa.Artist)
                .WithMany()
                .HasForeignKey(fa => fa.ArtistId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Album>()
                .HasOne(a => a.Artist)
                .WithMany(ar => ar.Albums)
                .HasForeignKey(a => a.ArtistId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Track>()
                .HasOne(t => t.Artist)
                .WithMany(ar => ar.Tracks)
                .HasForeignKey(t => t.ArtistId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Track>()
                .HasOne(t => t.Album)
                .WithMany(al => al.Tracks)
                .HasForeignKey(t => t.AlbumId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(m => m.Id).HasMaxLength(128);
                entity.Property(m => m.NormalizedUserName).HasMaxLength(128);
                entity.Property(m => m.NormalizedEmail).HasMaxLength(128);
            });
            modelBuilder.Entity<IdentityRole>(entity =>
            {
                entity.Property(m => m.Id).HasMaxLength(128);
                entity.Property(m => m.NormalizedName).HasMaxLength(128);
            });
            modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
            {
                entity.Property(m => m.LoginProvider).HasMaxLength(128);
                entity.Property(m => m.ProviderKey).HasMaxLength(128);
                entity.Property(m => m.UserId).HasMaxLength(128);
            });
            modelBuilder.Entity<IdentityUserRole<string>>(entity =>
            {
                entity.Property(m => m.UserId).HasMaxLength(128);
                entity.Property(m => m.RoleId).HasMaxLength(128);
            });
            modelBuilder.Entity<IdentityUserToken<string>>(entity =>
            {
                entity.Property(m => m.UserId).HasMaxLength(128);
                entity.Property(m => m.LoginProvider).HasMaxLength(128);
                entity.Property(m => m.Name).HasMaxLength(128);
            });
        }
    }
}
