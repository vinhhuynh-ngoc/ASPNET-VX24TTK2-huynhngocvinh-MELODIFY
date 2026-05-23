using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;
using Melodify.Models.Entities;

namespace Melodify.Data
{
    public class SeedConfig
    {
        public List<SeedArtist> Artists { get; set; } = new();
        public List<SeedTrack> Tracks { get; set; } = new();
    }

    public class SeedArtist
    {
        public string Name { get; set; } = string.Empty;
        public string Bio { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public int MonthlyListeners { get; set; }
    }

    public class SeedTrack
    {
        public string Title { get; set; } = string.Empty;
        public string Artist { get; set; } = string.Empty;
        public string Album { get; set; } = string.Empty;
        public int Duration { get; set; }
        public string AudioUrl { get; set; } = string.Empty;
        public string Genre { get; set; } = string.Empty;
    }

    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<AppDbContext>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();

            await context.Database.MigrateAsync();

            string[] roleNames = { "Admin", "User" };
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            var adminEmail = configuration["SeedSettings:AdminEmail"] ?? "admin@melodify.com";
            var adminPassword = configuration["SeedSettings:AdminPassword"] ?? "Admin123";
            var adminFullName = configuration["SeedSettings:AdminFullName"] ?? "Admin Melodify";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var admin = new User
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = adminFullName,
                    CreatedAt = DateTime.UtcNow,
                    EmailConfirmed = true
                };
                var createPowerUser = await userManager.CreateAsync(admin, adminPassword);
                if (createPowerUser.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }

            var testEmail = configuration["SeedSettings:TestUserEmail"] ?? "user@melodify.com";
            var testPassword = configuration["SeedSettings:TestUserPassword"] ?? "User123";
            var testFullName = configuration["SeedSettings:TestUserFullName"] ?? "Minh Khôi";

            var testUser = await userManager.FindByEmailAsync(testEmail);
            if (testUser == null)
            {
                var user = new User
                {
                    UserName = testEmail,
                    Email = testEmail,
                    FullName = testFullName,
                    CreatedAt = DateTime.UtcNow,
                    EmailConfirmed = true
                };
                var createTestUser = await userManager.CreateAsync(user, testPassword);
                if (createTestUser.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "User");
                }
            }

            var env = serviceProvider.GetRequiredService<Microsoft.AspNetCore.Hosting.IWebHostEnvironment>();
            var jsonPath = Path.Combine(env.WebRootPath, "tracks.json");
            if (File.Exists(jsonPath) && !context.Artists.Any())
            {
                var jsonString = await File.ReadAllTextAsync(jsonPath);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var seedConfig = JsonSerializer.Deserialize<SeedConfig>(jsonString, options);

                if (seedConfig != null)
                {
                    var artistMap = new Dictionary<string, Artist>();
                    foreach (var art in seedConfig.Artists)
                    {
                        var artist = new Artist
                        {
                            Name = art.Name,
                            Bio = art.Bio,
                            ImageUrl = art.ImageUrl,
                            MonthlyListeners = art.MonthlyListeners
                        };
                        context.Artists.Add(artist);
                        artistMap[art.Name] = artist;
                    }
                    await context.SaveChangesAsync();

                    var albumMap = new Dictionary<string, Album>();
                    foreach (var trk in seedConfig.Tracks)
                    {
                        var artist = artistMap.GetValueOrDefault(trk.Artist);
                        if (artist == null) continue;

                        var albumKey = $"{trk.Artist}_{trk.Album}";
                        if (!albumMap.ContainsKey(albumKey))
                        {
                            var album = new Album
                            {
                                Title = trk.Album,
                                ArtistId = artist.ArtistId,
                                ReleaseYear = 2024,
                                CoverImage = artist.ImageUrl
                            };
                            context.Albums.Add(album);
                            albumMap[albumKey] = album;
                        }
                    }
                    await context.SaveChangesAsync();

                    foreach (var trk in seedConfig.Tracks)
                    {
                        var artist = artistMap.GetValueOrDefault(trk.Artist);
                        if (artist == null) continue;

                        var albumKey = $"{trk.Artist}_{trk.Album}";
                        var album = albumMap.GetValueOrDefault(albumKey);

                        var track = new Track
                        {
                            Title = trk.Title,
                            ArtistId = artist.ArtistId,
                            AlbumId = album?.AlbumId,
                            Genre = trk.Genre ?? "V-Pop",
                            Duration = trk.Duration,
                            AudioUrl = trk.AudioUrl,
                            CoverImage = album?.CoverImage ?? artist.ImageUrl,
                            PlayCount = 0
                        };
                        context.Tracks.Add(track);
                    }
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
