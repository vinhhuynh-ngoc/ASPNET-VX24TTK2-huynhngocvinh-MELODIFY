using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Melodify.Services;

namespace Melodify.Controllers
{
    [Authorize]
    public class LibraryController : Controller
    {
        private readonly IPlaylistService _playlistService;
        private readonly ILikeService _likeService;

        public LibraryController(IPlaylistService playlistService, ILikeService _likeService)
        {
            _playlistService = playlistService;
            this._likeService = _likeService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string tab = "playlists")
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewBag.ActiveTab = tab.ToLower();

            var playlists = await _playlistService.GetUserPlaylistsAsync(userId);
            var likedTracks = await _likeService.GetLikedTracksAsync(userId);

            ViewBag.Playlists = playlists;
            ViewBag.LikedTracks = likedTracks;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Like(int trackId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _likeService.LikeTrackAsync(userId, trackId);
            return Json(new { success = true, liked = true });
        }

        [HttpPost]
        public async Task<IActionResult> Unlike(int trackId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            await _likeService.UnlikeTrackAsync(userId, trackId);
            return Json(new { success = true, liked = false });
        }

        [HttpGet]
        public async Task<IActionResult> IsLiked(int trackId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var likedTracks = await _likeService.GetLikedTracksAsync(userId);
            var isLiked = likedTracks.Any(t => t.TrackId == trackId);
            return Json(new { liked = isLiked });
        }
    }
}
