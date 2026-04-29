using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Melodify.Models.DTOs;
using Melodify.Models.Entities;
using Melodify.Services;

namespace Melodify.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class TracksController : Controller
    {
        private readonly ITrackService _trackService;
        private readonly IArtistService _artistService;
        private readonly IAlbumService _albumService;
        private readonly IFileService _fileService;

        public TracksController(
            ITrackService trackService,
            IArtistService artistService,
            IAlbumService albumService,
            IFileService fileService)
        {
            _trackService = trackService;
            _artistService = artistService;
            _albumService = albumService;
            _fileService = fileService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1, string search = "", int? artistId = null, string sortBy = "playCount_desc")
        {
            int pageSize = 10;
            var pagedResult = await _trackService.GetPagedAdminTracksAsync(page, pageSize, search, artistId, sortBy);

            var artists = await _artistService.GetAllArtistsAsync(null);
            ViewBag.Artists = new SelectList(artists, "ArtistId", "Name", artistId);
            ViewBag.Search = search;
            ViewBag.ArtistId = artistId;
            ViewBag.SortBy = sortBy;

            return View(pagedResult);
        }

        [HttpGet]
        public async Task<IActionResult> Upload()
        {
            var artists = await _artistService.GetAllArtistsAsync(null);
            var albums = await _albumService.GetAllAlbumsAsync();

            ViewBag.Artists = new SelectList(artists, "ArtistId", "Name");
            ViewBag.Albums = new SelectList(albums, "AlbumId", "Title");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(UploadTrackDto dto)
        {
            if (ModelState.IsValid)
            {
                var audioExt = Path.GetExtension(dto.AudioFile.FileName).ToLower();
                if (audioExt != ".mp3")
                {
                    ModelState.AddModelError("AudioFile", "Chỉ hỗ trợ tải lên tệp âm thanh định dạng .mp3");
                }
                if (dto.AudioFile.Length > 50 * 1024 * 1024)
                {
                    ModelState.AddModelError("AudioFile", "Kích thước tệp âm thanh không được vượt quá 50MB");
                }

                string? coverUrl = null;
                if (dto.CoverFile != null)
                {
                    var coverExt = Path.GetExtension(dto.CoverFile.FileName).ToLower();
                    string[] allowedCoverExts = { ".jpg", ".jpeg", ".png", ".webp" };
                    if (!allowedCoverExts.Contains(coverExt))
                    {
                        ModelState.AddModelError("CoverFile", "Định dạng ảnh bìa không hợp lệ. Chỉ chấp nhận .jpg, .jpeg, .png, .webp");
                    }
                    if (dto.CoverFile.Length > 5 * 1024 * 1024)
                    {
                        ModelState.AddModelError("CoverFile", "Kích thước ảnh bìa không được vượt quá 5MB");
                    }
                }

                if (ModelState.ErrorCount == 0)
                {
                    var audioUrl = await _fileService.SaveFileAsync(dto.AudioFile, "songs");
                    if (dto.CoverFile != null)
                    {
                        coverUrl = await _fileService.SaveFileAsync(dto.CoverFile, "covers");
                    }

                    var randomDuration = new Random().Next(180, 300);

                    var track = new Track
                    {
                        Title = dto.Title,
                        ArtistId = dto.ArtistId,
                        AlbumId = dto.AlbumId,
                        Genre = dto.Genre,
                        Duration = randomDuration,
                        AudioUrl = audioUrl,
                        CoverImage = coverUrl,
                        PlayCount = 0
                    };

                    await _trackService.AddTrackAsync(track);
                    TempData["SuccessMessage"] = "Tải lên bài hát thành công!";
                    return RedirectToAction(nameof(Index));
                }
            }

            var artists = await _artistService.GetAllArtistsAsync(null);
            var albums = await _albumService.GetAllAlbumsAsync();
            ViewBag.Artists = new SelectList(artists, "ArtistId", "Name", dto.ArtistId);
            ViewBag.Albums = new SelectList(albums, "AlbumId", "Title", dto.AlbumId);

            return View(dto);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var track = await _trackService.GetTrackByIdAsync(id, null);
            if (track == null)
            {
                return NotFound();
            }

            var dto = new EditTrackDto
            {
                TrackId = track.TrackId,
                Title = track.Title,
                ArtistId = track.ArtistId,
                AlbumId = track.AlbumId,
                Genre = track.Genre
            };

            var artists = await _artistService.GetAllArtistsAsync(null);
            var albums = await _albumService.GetAllAlbumsAsync();

            ViewBag.Artists = new SelectList(artists, "ArtistId", "Name", track.ArtistId);
            ViewBag.Albums = new SelectList(albums, "AlbumId", "Title", track.AlbumId);
            ViewBag.CoverImage = track.CoverImage;

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditTrackDto dto)
        {
            if (id != dto.TrackId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var trackEntity = await _trackService.GetTrackByIdAsync(id, null);
                if (trackEntity == null)
                {
                    return NotFound();
                }

                var entity = new Track
                {
                    TrackId = trackEntity.TrackId,
                    Title = dto.Title,
                    ArtistId = dto.ArtistId,
                    AlbumId = dto.AlbumId,
                    Genre = dto.Genre,
                    Duration = trackEntity.Duration,
                    AudioUrl = trackEntity.AudioUrl,
                    CoverImage = trackEntity.CoverImage,
                    PlayCount = trackEntity.PlayCount
                };

                if (dto.NewAudioFile != null)
                {
                    var audioExt = Path.GetExtension(dto.NewAudioFile.FileName).ToLower();
                    if (audioExt == ".mp3" && dto.NewAudioFile.Length <= 50 * 1024 * 1024)
                    {
                        _fileService.DeleteFile(trackEntity.AudioUrl);
                        entity.AudioUrl = await _fileService.SaveFileAsync(dto.NewAudioFile, "songs");
                    }
                }

                if (dto.NewCoverFile != null)
                {
                    var coverExt = Path.GetExtension(dto.NewCoverFile.FileName).ToLower();
                    string[] allowedCoverExts = { ".jpg", ".jpeg", ".png", ".webp" };
                    if (allowedCoverExts.Contains(coverExt) && dto.NewCoverFile.Length <= 5 * 1024 * 1024)
                    {
                        if (!string.IsNullOrEmpty(trackEntity.CoverImage))
                        {
                            _fileService.DeleteFile(trackEntity.CoverImage);
                        }
                        entity.CoverImage = await _fileService.SaveFileAsync(dto.NewCoverFile, "covers");
                    }
                }

                await _trackService.UpdateTrackAsync(entity);
                TempData["SuccessMessage"] = "Cập nhật bài hát thành công!";
                return RedirectToAction(nameof(Index));
            }

            var artists = await _artistService.GetAllArtistsAsync(null);
            var albums = await _albumService.GetAllAlbumsAsync();
            ViewBag.Artists = new SelectList(artists, "ArtistId", "Name", dto.ArtistId);
            ViewBag.Albums = new SelectList(albums, "AlbumId", "Title", dto.AlbumId);

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var track = await _trackService.GetTrackByIdAsync(id, null);
            if (track == null)
            {
                return NotFound();
            }

            _fileService.DeleteFile(track.AudioUrl);
            if (!string.IsNullOrEmpty(track.CoverImage))
            {
                _fileService.DeleteFile(track.CoverImage);
            }

            await _trackService.DeleteTrackAsync(id);
            TempData["SuccessMessage"] = "Xóa bài hát thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}
