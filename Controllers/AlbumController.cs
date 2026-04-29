using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Melodify.Services;

namespace Melodify.Controllers
{
    [Authorize]
    public class AlbumController : Controller
    {
        private readonly IAlbumService _albumService;
        private readonly IPlaylistService _playlistService;

        public AlbumController(IAlbumService albumService, IPlaylistService playlistService)
        {
            _albumService = albumService;
            _playlistService = playlistService;
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var album = await _albumService.GetAlbumDetailsAsync(id, userId);
            if (album == null)
            {
                return NotFound();
            }

            var playlists = await _playlistService.GetUserPlaylistsAsync(userId);
            ViewBag.UserPlaylists = playlists;

            return View(album);
        }
    }
}
