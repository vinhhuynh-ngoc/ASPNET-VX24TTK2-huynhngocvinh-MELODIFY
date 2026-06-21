using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Melodify.Services;

namespace Melodify.Controllers
{
    [Authorize]
    public class ArtistController : Controller
    {
        private readonly IArtistService _artistService;
        private readonly IPlaylistService _playlistService;
        private readonly ITrackService _trackService;
        private readonly IAlbumService _albumService;

        public ArtistController(
            IArtistService artistService,
            IPlaylistService playlistService,
            ITrackService trackService,
            IAlbumService albumService)
        {
            _artistService = artistService;
            _playlistService = playlistService;
            _trackService = trackService;
            _albumService = albumService;
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var artist = await _artistService.GetArtistDetailsAsync(id, userId);
            if (artist == null)
            {
                return NotFound();
            }

            var allTracks = await _trackService.GetAllTracksAsync(userId);
            var popularTracks = allTracks
                .Where(t => t.ArtistId == id)
                .OrderByDescending(t => t.PlayCount)
                .Take(5)
                .ToList();

            var allAlbums = await _albumService.GetAllAlbumsAsync();
            var artistAlbums = allAlbums.Where(a => a.ArtistId == id || (a.Artists != null && a.Artists.Any(ar => ar.ArtistId == id))).ToList();

            ViewBag.PopularTracks = popularTracks;
            ViewBag.Albums = artistAlbums;

            var playlists = await _playlistService.GetUserPlaylistsAsync(userId);
            ViewBag.UserPlaylists = playlists;

            return View(artist);
        }

        [HttpPost]
        public async Task<IActionResult> Follow(int artistId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _artistService.FollowArtistAsync(userId, artistId);
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> Unfollow(int artistId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _artistService.UnfollowArtistAsync(userId, artistId);
            return Json(new { success = true });
        }
    }
}
