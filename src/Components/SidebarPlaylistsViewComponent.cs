using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Melodify.Services;

namespace Melodify.Components
{
    public class SidebarPlaylistsViewComponent : ViewComponent
    {
        private readonly IPlaylistService _playlistService;

        public SidebarPlaylistsViewComponent(IPlaylistService playlistService)
        {
            _playlistService = playlistService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = UserClaimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return View(new List<Models.DTOs.PlaylistDto>());
            }

            var playlists = await _playlistService.GetUserPlaylistsAsync(userId);
            return View(playlists);
        }
    }
}
