using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using Melodify.Models.Entities;
using Melodify.Services;

namespace Melodify.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ArtistsController : Controller
    {
        private readonly IArtistService _artistService;
        private readonly IFileService _fileService;

        public ArtistsController(IArtistService artistService, IFileService fileService)
        {
            _artistService = artistService;
            _fileService = fileService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, string search = "")
        {
            int pageSize = 10;
            var pagedResult = await _artistService.GetPagedAdminArtistsAsync(page, pageSize, search);
            ViewBag.Search = search;
            return View(pagedResult);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Artist artist, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null)
                {
                    var extension = Path.GetExtension(imageFile.FileName).ToLower();
                    string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
                    if (allowedExtensions.Contains(extension) && imageFile.Length <= 5 * 1024 * 1024)
                    {
                        artist.ImageUrl = await _fileService.SaveFileAsync(imageFile, "artists");
                    }
                }
                artist.MonthlyListeners = 0;
                await _artistService.AddArtistAsync(artist);
                TempData["SuccessMessage"] = "Đã thêm nghệ sĩ thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(artist);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var artist = await _artistService.GetArtistByIdAsync(id, null);
            if (artist == null)
            {
                return NotFound();
            }
            var entity = new Artist
            {
                ArtistId = artist.ArtistId,
                Name = artist.Name,
                ImageUrl = artist.ImageUrl,
                MonthlyListeners = artist.MonthlyListeners,
                Bio = artist.Bio
            };
            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Artist artist, IFormFile? newImageFile)
        {
            if (id != artist.ArtistId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var original = await _artistService.GetArtistByIdAsync(id, null);
                if (original == null)
                {
                    return NotFound();
                }

                var entity = new Artist
                {
                    ArtistId = artist.ArtistId,
                    Name = artist.Name,
                    ImageUrl = original.ImageUrl,
                    MonthlyListeners = original.MonthlyListeners,
                    Bio = artist.Bio
                };

                if (newImageFile != null)
                {
                    var extension = Path.GetExtension(newImageFile.FileName).ToLower();
                    string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
                    if (allowedExtensions.Contains(extension) && newImageFile.Length <= 5 * 1024 * 1024)
                    {
                        if (!string.IsNullOrEmpty(original.ImageUrl))
                        {
                            _fileService.DeleteFile(original.ImageUrl);
                        }
                        entity.ImageUrl = await _fileService.SaveFileAsync(newImageFile, "artists");
                    }
                }

                await _artistService.UpdateArtistAsync(entity);
                TempData["SuccessMessage"] = "Đã cập nhật nghệ sĩ thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(artist);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var artist = await _artistService.GetArtistByIdAsync(id, null);
            if (artist == null)
            {
                return NotFound();
            }
            if (!string.IsNullOrEmpty(artist.ImageUrl))
            {
                _fileService.DeleteFile(artist.ImageUrl);
            }
            await _artistService.DeleteArtistAsync(id);
            TempData["SuccessMessage"] = "Đã xóa nghệ sĩ thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}
