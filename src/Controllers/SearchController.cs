using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;
using Melodify.Services;

namespace Melodify.Controllers
{
    [Authorize]
    public class SearchController : Controller
    {
        private readonly ITrackService _trackService;

        public SearchController(ITrackService trackService)
        {
            _trackService = trackService;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Results(string q, int page = 1)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _trackService.SearchTracksAsync(q, userId, page, 20);
            return Json(result);
        }
    }
}
