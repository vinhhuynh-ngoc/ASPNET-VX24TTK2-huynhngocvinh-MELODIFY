using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Melodify.Models.Entities;
using Melodify.Services;

namespace Melodify.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class AlbumsController : Controller
    {
        private readonly IAlbumService _albumService;
        private readonly IArtistService _artistService;
        private readonly IFileService _fileService;

        public AlbumsController(IAlbumService albumService, IArtistService artistService, IFileService fileService)
        {
            _albumService = albumService;
            _artistService = artistService;
            _fileService = fileService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, string search = "")
        {
            int pageSize = 10;
            var pagedResult = await _albumService.GetPagedAdminAlbumsAsync(page, pageSize, search);
            ViewBag.Search = search;
            return View(pagedResult);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var artists = await _artistService.GetAllArtistsAsync(null);
            ViewBag.Artists = new SelectList(artists, "ArtistId", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Album album, IFormFile? coverFile)
        {
            if (ModelState.IsValid)
            {
                if (coverFile != null)
                {
                    var extension = Path.GetExtension(coverFile.FileName).ToLower();
                    string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
                    if (allowedExtensions.Contains(extension) && coverFile.Length <= 5 * 1024 * 1024)
                    {
                        album.CoverImage = await _fileService.SaveFileAsync(coverFile, "covers");
                    }
                }
                await _albumService.AddAlbumAsync(album);
                TempData["SuccessMessage"] = "Đã thêm album thành công!";
                return RedirectToAction(nameof(Index));
            }

            var artists = await _artistService.GetAllArtistsAsync(null);
            ViewBag.Artists = new SelectList(artists, "ArtistId", "Name", album.ArtistId);
            return View(album);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var album = await _albumService.GetAlbumByIdAsync(id);
            if (album == null)
            {
                return NotFound();
            }
            var entity = new Album
            {
                AlbumId = album.AlbumId,
                Title = album.Title,
                ArtistId = album.ArtistId,
                CoverImage = album.CoverImage,
                ReleaseYear = album.ReleaseYear
            };

            var artists = await _artistService.GetAllArtistsAsync(null);
            ViewBag.Artists = new SelectList(artists, "ArtistId", "Name", album.ArtistId);
            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Album album, IFormFile? newCoverFile)
        {
            if (id != album.AlbumId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var original = await _albumService.GetAlbumByIdAsync(id);
                if (original == null)
                {
                    return NotFound();
                }

                var entity = new Album
                {
                    AlbumId = album.AlbumId,
                    Title = album.Title,
                    ArtistId = album.ArtistId,
                    CoverImage = original.CoverImage,
                    ReleaseYear = album.ReleaseYear
                };

                if (newCoverFile != null)
                {
                    var extension = Path.GetExtension(newCoverFile.FileName).ToLower();
                    string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
                    if (allowedExtensions.Contains(extension) && newCoverFile.Length <= 5 * 1024 * 1024)
                    {
                        if (!string.IsNullOrEmpty(original.CoverImage))
                        {
                            _fileService.DeleteFile(original.CoverImage);
                        }
                        entity.CoverImage = await _fileService.SaveFileAsync(newCoverFile, "covers");
                    }
                }

                await _albumService.UpdateAlbumAsync(entity);
                TempData["SuccessMessage"] = "Đã cập nhật album thành công!";
                return RedirectToAction(nameof(Index));
            }

            var artists = await _artistService.GetAllArtistsAsync(null);
            ViewBag.Artists = new SelectList(artists, "ArtistId", "Name", album.ArtistId);
            return View(album);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var album = await _albumService.GetAlbumByIdAsync(id);
            if (album == null)
            {
                return NotFound();
            }
            if (!string.IsNullOrEmpty(album.CoverImage))
            {
                _fileService.DeleteFile(album.CoverImage);
            }
            await _albumService.DeleteAlbumAsync(id);
            TempData["SuccessMessage"] = "Đã xóa album thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}
