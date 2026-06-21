using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Melodify.Data;
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
        private readonly ITrackService _trackService;
        private readonly IFileService _fileService;
        private readonly AppDbContext _context;

        public AlbumsController(
            IAlbumService albumService,
            IArtistService artistService,
            ITrackService trackService,
            IFileService fileService,
            AppDbContext context)
        {
            _albumService = albumService;
            _artistService = artistService;
            _trackService = trackService;
            _fileService = fileService;
            _context = context;
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
            var allTracks = await _trackService.GetAllTracksAsync("");
            ViewBag.AllTracks = allTracks;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Album album, IFormFile? coverFile, List<int>? SelectedTrackIds)
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

                if (SelectedTrackIds != null && SelectedTrackIds.Any())
                {
                    var firstTrackId = SelectedTrackIds.First();
                    var firstTrack = await _context.Tracks.FirstOrDefaultAsync(t => t.TrackId == firstTrackId);
                    if (firstTrack != null)
                    {
                        album.ArtistId = firstTrack.ArtistId;
                    }
                }

                await _albumService.AddAlbumAsync(album);

                if (SelectedTrackIds != null && SelectedTrackIds.Any())
                {
                    var tracks = await _context.Tracks
                        .Where(t => SelectedTrackIds.Contains(t.TrackId))
                        .ToListAsync();
                    foreach (var track in tracks)
                    {
                        track.AlbumId = album.AlbumId;
                    }
                    await _context.SaveChangesAsync();
                }

                TempData["SuccessMessage"] = "Đã thêm album thành công!";
                return RedirectToAction(nameof(Index));
            }

            var allTracks = await _trackService.GetAllTracksAsync("");
            ViewBag.AllTracks = allTracks;
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

            var allTracks = await _trackService.GetAllTracksAsync("");
            ViewBag.AllTracks = allTracks;

            var currentTrackIds = await _context.Tracks
                .Where(t => t.AlbumId == id)
                .Select(t => t.TrackId)
                .ToListAsync();
            ViewBag.CurrentTrackIds = currentTrackIds;

            return View(entity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Album album, IFormFile? newCoverFile, List<int>? SelectedTrackIds)
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

                int? computedArtistId = null;
                if (SelectedTrackIds != null && SelectedTrackIds.Any())
                {
                    var firstTrackId = SelectedTrackIds.First();
                    var firstTrack = await _context.Tracks.FirstOrDefaultAsync(t => t.TrackId == firstTrackId);
                    if (firstTrack != null)
                    {
                        computedArtistId = firstTrack.ArtistId;
                    }
                }

                var entity = new Album
                {
                    AlbumId = album.AlbumId,
                    Title = album.Title,
                    ArtistId = computedArtistId,
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

                var previousTracks = await _context.Tracks
                    .Where(t => t.AlbumId == id)
                    .ToListAsync();
                foreach (var track in previousTracks)
                {
                    track.AlbumId = null;
                }

                if (SelectedTrackIds != null && SelectedTrackIds.Any())
                {
                    var newTracks = await _context.Tracks
                        .Where(t => SelectedTrackIds.Contains(t.TrackId))
                        .ToListAsync();
                    foreach (var track in newTracks)
                    {
                        track.AlbumId = id;
                    }
                }

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Đã cập nhật album thành công!";
                return RedirectToAction(nameof(Index));
            }

            var allTracksReload = await _trackService.GetAllTracksAsync("");
            ViewBag.AllTracks = allTracksReload;
            var currentTrackIds = await _context.Tracks
                .Where(t => t.AlbumId == id)
                .Select(t => t.TrackId)
                .ToListAsync();
            ViewBag.CurrentTrackIds = currentTrackIds;
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
