using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Melodify.Models;
using Melodify.Models.Entities;
using Melodify.Services;

namespace Melodify.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ITrackService _trackService;
        private readonly IAlbumService _albumService;
        private readonly IArtistService _artistService;
        private readonly IPlaylistService _playlistService;
        private readonly UserManager<User> _userManager;

        public HomeController(
            ITrackService trackService,
            IAlbumService albumService,
            IArtistService artistService,
            IPlaylistService playlistService,
            UserManager<User> userManager)
        {
            _trackService = trackService;
            _albumService = albumService;
            _artistService = artistService;
            _playlistService = playlistService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);
            var fullName = !string.IsNullOrWhiteSpace(user?.FullName) ? user.FullName : (user?.Email ?? "Người dùng");

            // Using the optimized DB-level limited queries
            var recentAlbums = (await _albumService.GetRecentAlbumsAsync(6)).ToList();
            var featuredTracks = (await _trackService.GetFeaturedTracksAsync(5, userId)).ToList();
            var suggestedArtists = (await _artistService.GetSuggestedArtistsAsync(5, userId)).ToList();
            var featuredPlaylists = (await _playlistService.GetUserPlaylistsAsync(userId)).Take(5).ToList();

            var viewModel = new HomeViewModel
            {
                Greeting = $"Xin chào, <span class=\"text-lime\">{fullName}</span>",
                RecentAlbums = recentAlbums,
                FeaturedTracks = featuredTracks,
                SuggestedArtists = suggestedArtists,
                FeaturedPlaylists = featuredPlaylists
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> GetTracks(int page = 1)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            // Search with empty query will return all tracks paginated
            var result = await _trackService.SearchTracksAsync("", userId, page, 20);
            return Json(result);
        }

        [HttpPost]
        public async Task<IActionResult> IncrementPlayCount(int trackId)
        {
            await _trackService.IncrementPlayCountAsync(trackId);
            return Ok();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
