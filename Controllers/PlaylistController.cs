using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Melodify.Services;

namespace Melodify.Controllers
{
    [Authorize]
    public class PlaylistController : Controller
    {
        private readonly IPlaylistService _playlistService;

        public PlaylistController(IPlaylistService playlistService)
        {
            _playlistService = playlistService;
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var playlist = await _playlistService.GetPlaylistDetailsAsync(id, userId);
            if (playlist == null)
            {
                return NotFound();
            }
            if (playlist.UserId != userId)
            {
                return Forbid();
            }
            return View(playlist);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string name)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrWhiteSpace(name))
            {
                await _playlistService.CreatePlaylistAsync(userId, name);
            }
            return Redirect(Request.Headers["Referer"].ToString() ?? "/");
        }

        [HttpPost]
        public async Task<IActionResult> AddTrack(int playlistId, int trackId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var playlist = await _playlistService.GetPlaylistByIdAsync(playlistId);
            if (playlist == null || playlist.UserId != userId)
            {
                return Forbid();
            }
            await _playlistService.AddTrackToPlaylistAsync(playlistId, trackId);
            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> RemoveTrack(int playlistId, int trackId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var playlist = await _playlistService.GetPlaylistByIdAsync(playlistId);
            if (playlist == null || playlist.UserId != userId)
            {
                return Forbid();
            }
            await _playlistService.RemoveTrackFromPlaylistAsync(playlistId, trackId);
            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int playlistId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var playlist = await _playlistService.GetPlaylistByIdAsync(playlistId);
            if (playlist == null || playlist.UserId != userId)
            {
                return Forbid();
            }
            await _playlistService.DeletePlaylistAsync(playlistId);
            return RedirectToAction("Index", "Library");
        }
    }
}
