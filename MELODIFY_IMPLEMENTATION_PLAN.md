# 🎵 Melodify — Implementation Plan (ASP.NET Core)

> **Gửi AI agent (Antigravity):** Đây là kế hoạch triển khai toàn bộ ứng dụng web nghe nhạc **Melodify** bằng ASP.NET Core MVC (.NET 8). UI đã được thiết kế sẵn trên Lovable — bạn phải **fetch UI từ một trong hai link sau**, rồi tái hiện đúng pixel/layout đó vào Razor Views, **không dùng công nghệ của Lovable (React/Vite/Tailwind SPA)**, mà dùng đúng stack bài tập lớn ghi bên dưới.

---

## 🔗 UI Reference (BẮT BUỘC fetch trước khi code)

| Môi trường | URL |
|---|---|
| Lovable Editor | `https://lovable.dev/projects/a1e57261-3468-40b2-bc11-ae69a27ae0a8?magic_link=mc_ea932989-0300-41ba-8d9f-291416fdeac2` |
| Live Preview | `https://lyric-lullaby-lane.lovable.app/` |

**Yêu cầu:** Mở live preview, lấy màu sắc, font, layout, spacing, component style → áp dụng vào Bootstrap 5 + custom CSS. Không sử dụng React, Vite, hay bất kỳ SPA framework nào.

---

## 🛠️ Tech Stack (BẮT BUỘC dùng đúng)

| Layer | Công nghệ |
|---|---|
| Web Framework | ASP.NET Core MVC (.NET 8) |
| ORM | Entity Framework Core 8 |
| Database | MySQL |
| Authentication | ASP.NET Core Identity |
| Object Mapping | AutoMapper |
| Frontend | Razor Views + Bootstrap 5 + jQuery |
| Audio | HTML5 `<audio>` API |
| File Upload | `IFormFile` → `wwwroot/uploads/` |
| Search | AJAX + jQuery debounce |

---

## 📁 Cấu trúc Project

```
Melodify/
├── Areas/
│   └── Admin/
│       ├── Controllers/
│       │   ├── TracksController.cs
│       │   ├── ArtistsController.cs
│       │   └── AlbumsController.cs
│       └── Views/
│           ├── Tracks/
│           │   ├── Index.cshtml
│           │   └── Upload.cshtml
│           ├── Artists/
│           └── Albums/
├── Controllers/
│   ├── HomeController.cs
│   ├── SearchController.cs
│   ├── AlbumController.cs
│   ├── PlaylistController.cs
│   ├── ArtistController.cs
│   ├── LibraryController.cs
│   └── AccountController.cs
├── Models/
│   ├── Entities/
│   │   ├── User.cs
│   │   ├── Artist.cs
│   │   ├── Album.cs
│   │   ├── Track.cs
│   │   ├── Playlist.cs
│   │   ├── PlaylistTrack.cs
│   │   ├── LikedTrack.cs
│   │   └── FollowArtist.cs
│   └── DTOs/
│       ├── TrackDto.cs
│       ├── ArtistDto.cs
│       ├── AlbumDto.cs
│       └── PlaylistDto.cs
├── Services/
│   ├── ITrackService.cs / TrackService.cs
│   ├── IArtistService.cs / ArtistService.cs
│   ├── IAlbumService.cs / AlbumService.cs
│   ├── IPlaylistService.cs / PlaylistService.cs
│   ├── ILikeService.cs / LikeService.cs
│   └── IFileService.cs / FileService.cs
├── Repositories/
│   ├── ITrackRepository.cs / TrackRepository.cs
│   ├── IArtistRepository.cs / ArtistRepository.cs
│   ├── IAlbumRepository.cs / AlbumRepository.cs
│   └── IPlaylistRepository.cs / PlaylistRepository.cs
├── Data/
│   ├── AppDbContext.cs
│   └── SeedData.cs
├── Views/
│   ├── Shared/
│   │   ├── _Layout.cshtml          ← AppShell (sidebar + player bar)
│   │   ├── _Sidebar.cshtml
│   │   ├── _PlayerBar.cshtml
│   │   └── _LoginLayout.cshtml     ← Layout riêng cho login/register
│   ├── Home/
│   │   └── Index.cshtml
│   ├── Search/
│   │   └── Index.cshtml
│   ├── Album/
│   │   └── Detail.cshtml
│   ├── Playlist/
│   │   └── Detail.cshtml
│   ├── Artist/
│   │   └── Detail.cshtml
│   ├── Library/
│   │   └── Index.cshtml
│   └── Account/
│       ├── Login.cshtml
│       └── Register.cshtml
├── wwwroot/
│   ├── css/
│   │   └── site.css
│   ├── js/
│   │   ├── player.js
│   │   ├── search.js
│   │   └── playlist.js
│   └── uploads/
│       ├── songs/
│       └── covers/
├── Mappings/
│   └── AutoMapperProfile.cs
└── Program.cs
```

---

## 🗃️ Database Schema (EF Core Entities)

### User (kế thừa IdentityUser)
```csharp
public class User : IdentityUser
{
    public string FullName { get; set; }
    public DateTime CreatedAt { get; set; }
    // Navigation
    public ICollection<Playlist> Playlists { get; set; }
    public ICollection<LikedTrack> LikedTracks { get; set; }
    public ICollection<FollowArtist> FollowedArtists { get; set; }
}
```

### Artist
```csharp
public class Artist
{
    public int ArtistId { get; set; }
    public string Name { get; set; }
    public string? ImageUrl { get; set; }
    public int MonthlyListeners { get; set; }
    public string? Bio { get; set; }
    public ICollection<Track> Tracks { get; set; }
    public ICollection<Album> Albums { get; set; }
}
```

### Album
```csharp
public class Album
{
    public int AlbumId { get; set; }
    public string Title { get; set; }
    public int ArtistId { get; set; }
    public string? CoverImage { get; set; }
    public int ReleaseYear { get; set; }
    public Artist Artist { get; set; }
    public ICollection<Track> Tracks { get; set; }
}
```

### Track
```csharp
public class Track
{
    public int TrackId { get; set; }
    public string Title { get; set; }
    public int ArtistId { get; set; }
    public int? AlbumId { get; set; }
    public string? Genre { get; set; }
    public int Duration { get; set; }      // giây
    public string AudioUrl { get; set; }   // đường dẫn file MP3
    public string? CoverImage { get; set; }
    public int PlayCount { get; set; }
    public Artist Artist { get; set; }
    public Album? Album { get; set; }
}
```

### Playlist
```csharp
public class Playlist
{
    public int PlaylistId { get; set; }
    public string UserId { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public User User { get; set; }
    public ICollection<PlaylistTrack> PlaylistTracks { get; set; }
}
```

### PlaylistTrack (bảng nối)
```csharp
public class PlaylistTrack
{
    public int PlaylistId { get; set; }
    public int TrackId { get; set; }
    public Playlist Playlist { get; set; }
    public Track Track { get; set; }
}
```

### LikedTrack
```csharp
public class LikedTrack
{
    public string UserId { get; set; }
    public int TrackId { get; set; }
    public DateTime LikedAt { get; set; }
    public User User { get; set; }
    public Track Track { get; set; }
}
```

### FollowArtist
```csharp
public class FollowArtist
{
    public string UserId { get; set; }
    public int ArtistId { get; set; }
    public DateTime FollowedAt { get; set; }
    public User User { get; set; }
    public Artist Artist { get; set; }
}
```

---

## 🎨 Design Tokens (áp dụng vào CSS)

```css
:root {
    --primary: #1DB954;          /* Xanh lá Spotify */
    --bg-base: #121212;
    --bg-elevated: #1E1E1E;
    --bg-card: #282828;
    --text-primary: #FFFFFF;
    --text-secondary: #B3B3B3;
    --sidebar-width: 200px;
    --player-height: 68px;
    --card-radius: 8px;
    --track-row-height: 52px;
}
```

---

## 📋 PLAN — Chia theo Phase

---

### PHASE 0 — Project Setup

**Mục tiêu:** Dựng khung project, cấu hình DB, Identity, AutoMapper, Static Files.

**Checklist:**
- [ ] `dotnet new mvc -n Melodify`
- [ ] Cài NuGet: `Pomelo.EntityFrameworkCore.MySql`, `Microsoft.EntityFrameworkCore.Tools`, `Microsoft.AspNetCore.Identity.EntityFrameworkCore`, `AutoMapper.Extensions.Microsoft.DependencyInjection`
- [ ] Tạo `AppDbContext.cs` kế thừa `IdentityDbContext<User>`
- [ ] Cấu hình `Program.cs`:
  - `AddDbContext` với MySQL
  - `AddIdentity<User, IdentityRole>` với cookie auth
  - `AddAutoMapper`
  - `UseStaticFiles`
  - `MapControllerRoute` (Areas + Default)
- [ ] `dotnet ef migrations add Init` → `dotnet ef database update`
- [ ] Tạo `SeedData.cs`: seed role `Admin`/`User`, 1 admin account, vài artist + track mẫu
- [ ] Tạo `AutoMapperProfile.cs`: map Entity → DTO

---

### PHASE 1 — AppShell (Layout)

> **Fetch UI:** `https://lyric-lullaby-lane.lovable.app/` → quan sát sidebar, player bar, màu nền, font chữ.

**Mục tiêu:** Tạo layout dùng chung cho toàn app.

**Files cần tạo:**
- `Views/Shared/_Layout.cshtml` — layout chính (sidebar trái + main content + player bar cố định đáy)
- `Views/Shared/_Sidebar.cshtml` — partial: Logo, nav links, danh sách playlist
- `Views/Shared/_PlayerBar.cshtml` — partial: ảnh bìa + tên bài + controls + seekbar + volume
- `Views/Shared/_LoginLayout.cshtml` — layout riêng không có sidebar/player, dùng cho login/register
- `wwwroot/css/site.css` — toàn bộ custom CSS theo design tokens
- `wwwroot/js/player.js` — HTML5 Audio player logic

**_Layout.cshtml structure:**
```html
<body>
  <div class="app-shell">
    <partial name="_Sidebar" />
    <main class="main-content">@RenderBody()</main>
  </div>
  <partial name="_PlayerBar" />
  @RenderSection("Scripts", required: false)
</body>
```

**player.js — chức năng cần implement:**
- Play/Pause toggle (UC05, UC06)
- Next / Prev track (UC07, UC08)
- Seekbar (timeupdate event)
- Volume control (UC09)
- `playTrack(trackId, audioUrl, title, artist, coverUrl)` — hàm global gọi từ mọi trang

---

### PHASE 2 — Authentication (UC01, UC02, UC03)

> **Fetch UI:** Xem màn hình login/register tại `https://lyric-lullaby-lane.lovable.app/login`

**Files cần tạo:**
- `Controllers/AccountController.cs`
- `Views/Account/Login.cshtml`
- `Views/Account/Register.cshtml`

**AccountController — Actions:**

```csharp
// GET /login
IActionResult Login()

// POST /login
IActionResult Login(LoginDto model)
// → SignInManager.PasswordSignInAsync → redirect Home

// GET /register
IActionResult Register()

// POST /register
IActionResult Register(RegisterDto model)
// → UserManager.CreateAsync → SignInManager.SignInAsync → redirect Home

// POST /logout (UC03)
IActionResult Logout()
// → SignInManager.SignOutAsync → redirect Login
```

**DTOs:**
```csharp
public class LoginDto { public string Email; public string Password; }
public class RegisterDto { public string FullName; public string Email; public string Password; public string ConfirmPassword; }
```

**Views:** Centered card ~400px, dùng `_LoginLayout`, form validation với `asp-validation-for`.

---

### PHASE 3 — Trang chủ (UC04)

> **Fetch UI:** `https://lyric-lullaby-lane.lovable.app/` → quan sát grid "Mới phát gần đây" và section "Khám phá".

**Files:**
- `Controllers/HomeController.cs`
- `Views/Home/Index.cshtml`

**HomeController:**
```csharp
// GET /
[Authorize]
IActionResult Index()
// → lấy 6 playlist/album gần đây
// → lấy tracks nổi bật (order by PlayCount)
// → trả ViewModel gồm RecentItems + FeaturedTracks
```

**Index.cshtml layout:**
```
Greeting: "Chào buổi tối, {FullName}"

Section "Mới phát gần đây":
  Grid 2 cột, chip ngang (ảnh 48×48 + tên), tối đa 6 items

Section "Khám phá":
  Card grid 4-5 cột
  Mỗi card: ảnh bìa vuông + tên track + nghệ sĩ
  Hover → hiện nút Play (gọi playTrack(...))
```

---

### PHASE 4 — Tìm kiếm (UC10)

> **Fetch UI:** `https://lyric-lullaby-lane.lovable.app/search`

**Files:**
- `Controllers/SearchController.cs`
- `Views/Search/Index.cshtml`
- `wwwroot/js/search.js`

**SearchController:**
```csharp
// GET /search
IActionResult Index()

// GET /search/results?q=keyword  (AJAX endpoint)
IActionResult Results(string q)
// → TrackService.Search(q) → LINQ Contains → trả JSON list TrackDto
```

**search.js:**
```javascript
// debounce 300ms
$('#search-input').on('input', debounce(function() {
    const q = $(this).val().trim();
    if (q.length === 0) { showGenreGrid(); return; }
    $.get('/search/results', { q }, function(data) {
        renderTrackList(data);
    });
}, 300));
```

**Index.cshtml:**
- Khi rỗng: Grid thể loại màu (V-Pop, K-Pop, Ballad, R&B, Acoustic, Rap) — hardcode màu sắc
- Khi có kết quả: Table `[Ảnh] [Tên bài] [Nghệ sĩ] [Thời lượng]`, click row → `playTrack(...)`

---

### PHASE 5 — Chi tiết Album & Playlist (UC11, UC12)

> **Fetch UI:** Click vào album bất kỳ trên live preview.

**Files:**
- `Controllers/AlbumController.cs` → `Views/Album/Detail.cshtml`
- `Controllers/PlaylistController.cs` → `Views/Playlist/Detail.cshtml`

**Detail layout (dùng chung):**
```
Hero section:
  Ảnh bìa lớn (220×220) + Tên + Nghệ sĩ/Owner + Năm + Số bài

Action bar: [▶ Play] [⇄ Shuffle] [♥ Like]

Track list table:
  [#] [Tên bài] [Nghệ sĩ] [♥ Like] [⏱ Thời lượng]
  Row hover → Play icon thay số thứ tự
  Row đang phát → tên màu var(--primary)
```

**PlaylistController — thêm Actions:**
```csharp
// POST /playlist/create (UC14)
IActionResult Create(string name)

// POST /playlist/add-track (UC16)
IActionResult AddTrack(int playlistId, int trackId)

// POST /playlist/remove-track (UC17)
IActionResult RemoveTrack(int playlistId, int trackId)

// POST /playlist/delete (UC18)
IActionResult Delete(int playlistId)
```

---

### PHASE 6 — Trang nghệ sĩ (UC13, UC22, UC23)

> **Fetch UI:** Click nghệ sĩ trên live preview.

**Files:**
- `Controllers/ArtistController.cs`
- `Views/Artist/Detail.cshtml`

**Detail.cshtml:**
```
Hero: Ảnh + gradient overlay + Tên + "{X} người nghe hàng tháng"
Action: [▶ Play] [Theo dõi / Hủy theo dõi]
Top 5 bài phổ biến (table đơn giản, order by PlayCount DESC)
Albums của nghệ sĩ (card grid)
```

**ArtistController:**
```csharp
// GET /artist/{id}
IActionResult Detail(int id)

// POST /artist/follow (UC22)
[Authorize]
IActionResult Follow(int artistId)

// POST /artist/unfollow (UC23)
[Authorize]
IActionResult Unfollow(int artistId)
```

---

### PHASE 7 — Thư viện & Yêu thích (UC14–UC21)

> **Fetch UI:** `https://lyric-lullaby-lane.lovable.app/library`

**Files:**
- `Controllers/LibraryController.cs`
- `Views/Library/Index.cshtml`

**Index.cshtml:**
```
Tabs: [Playlist] [Bài đã thích]

Tab Playlist:
  List (ảnh + tên + số bài)
  Nút "+ Tạo playlist mới" → modal hoặc inline form

Tab Bài đã thích:
  Track list table giống màn Album Detail
```

**LibraryController:**
```csharp
// GET /library
[Authorize]
IActionResult Index(string tab = "playlists")
// → lấy playlists của user + liked tracks

// POST /library/like (UC19)
[Authorize]
IActionResult Like(int trackId)  // AJAX → trả JSON {liked: true}

// POST /library/unlike (UC20)
[Authorize]
IActionResult Unlike(int trackId)  // AJAX → trả JSON {liked: false}
```

---

### PHASE 8 — Admin Area (UC24–UC29)

> **Fetch UI:** `https://lyric-lullaby-lane.lovable.app/admin` (nếu có) hoặc tự thiết kế bám sát đặc tả.

**Áp dụng [Authorize(Roles = "Admin")] cho toàn bộ Admin Area.**

#### 8.1 — Quản lý bài hát (UC24, UC25, UC26, UC27)

**Files:**
- `Areas/Admin/Controllers/TracksController.cs`
- `Areas/Admin/Views/Tracks/Index.cshtml`
- `Areas/Admin/Views/Tracks/Upload.cshtml`

**Index.cshtml (UC24):**
```
Sidebar riêng Admin (không có player bar)
Table: [Ảnh 48×48] [Tên bài] [Nghệ sĩ] [Album] [Lượt nghe] [Sửa] [Xóa]
Nút "Upload bài hát mới" → /admin/tracks/upload
Pagination: PageSize = 10, dùng LINQ Skip/Take
```

**Upload.cshtml (UC25):**
```
Drag & drop MP3 → preview tên file + progress bar (fetch API)
Upload ảnh bìa → preview thumbnail
Form:
  Tên bài hát *
  Nghệ sĩ (dropdown từ DB) *
  Album (dropdown từ DB, optional)
  Thể loại (text input)
[Lưu] [Hủy]
```

**TracksController:**
```csharp
// GET /admin/tracks
IActionResult Index(int page = 1)

// GET /admin/tracks/upload
IActionResult Upload()

// POST /admin/tracks/upload (UC25)
IActionResult Upload(UploadTrackDto dto, IFormFile audioFile, IFormFile coverFile)
// → FileService.SaveFile(audioFile, "songs") → lưu đường dẫn vào Track.AudioUrl
// → FileService.SaveFile(coverFile, "covers") → lưu Track.CoverImage

// GET /admin/tracks/edit/{id}
IActionResult Edit(int id)

// POST /admin/tracks/edit/{id} (UC26)
IActionResult Edit(int id, EditTrackDto dto, IFormFile? newCoverFile)

// POST /admin/tracks/delete/{id} (UC27)
IActionResult Delete(int id)
// → xóa file vật lý + xóa DB record
```

**FileService:**
```csharp
public class FileService : IFileService
{
    public async Task<string> SaveFile(IFormFile file, string folder)
    {
        // validate extension (.mp3, .jpg, .png, .webp)
        // generate unique filename: Guid.NewGuid() + ext
        // save to wwwroot/uploads/{folder}/
        // return relative path: /uploads/{folder}/{filename}
    }
    
    public void DeleteFile(string relativePath)
    {
        // xóa file vật lý tại wwwroot + relativePath
    }
}
```

#### 8.2 — Quản lý nghệ sĩ (UC28)

**Files:**
- `Areas/Admin/Controllers/ArtistsController.cs`
- `Areas/Admin/Views/Artists/Index.cshtml` (table + CRUD)
- `Areas/Admin/Views/Artists/Create.cshtml`
- `Areas/Admin/Views/Artists/Edit.cshtml`

#### 8.3 — Quản lý Album (UC29)

**Files:**
- `Areas/Admin/Controllers/AlbumsController.cs`
- `Areas/Admin/Views/Albums/Index.cshtml` (table + CRUD)
- `Areas/Admin/Views/Albums/Create.cshtml`
- `Areas/Admin/Views/Albums/Edit.cshtml`

---

### PHASE 9 — Polish & Integration

- [ ] `SeedData.cs`: thêm đủ dữ liệu mẫu (5 artist, 3 album mỗi artist, 5 track mỗi album, file MP3 mẫu ngắn)
- [ ] Xử lý lỗi: 404 page, 403 page, try-catch trong Service layer
- [ ] Flash messages (TempData): "Đã tạo playlist!", "Đã like bài hát", "Upload thành công"
- [ ] Validation: DataAnnotations trên DTO + `ModelState.IsValid` check
- [ ] `[ValidateAntiForgeryToken]` trên tất cả POST actions
- [ ] Responsive: kiểm tra Bootstrap breakpoints, sidebar collapse trên mobile
- [ ] Kiểm tra lại toàn bộ 29 use case

---

## ✅ Use Case Checklist

| UC | Tên | Phase |
|---|---|---|
| UC01 | Đăng ký | Phase 2 |
| UC02 | Đăng nhập | Phase 2 |
| UC03 | Đăng xuất | Phase 2 |
| UC04 | Xem trang chủ | Phase 3 |
| UC05 | Phát bài hát | Phase 1 (player.js) |
| UC06 | Tạm dừng | Phase 1 |
| UC07 | Next track | Phase 1 |
| UC08 | Prev track | Phase 1 |
| UC09 | Âm lượng | Phase 1 |
| UC10 | Tìm kiếm | Phase 4 |
| UC11 | Chi tiết Album | Phase 5 |
| UC12 | Chi tiết Playlist | Phase 5 |
| UC13 | Trang nghệ sĩ | Phase 6 |
| UC14 | Tạo playlist | Phase 5 |
| UC15 | Xem playlist cá nhân | Phase 7 |
| UC16 | Thêm bài vào playlist | Phase 5 |
| UC17 | Xóa bài khỏi playlist | Phase 5 |
| UC18 | Xóa playlist | Phase 5 |
| UC19 | Like bài hát | Phase 7 |
| UC20 | Bỏ like | Phase 7 |
| UC21 | Xem bài đã thích | Phase 7 |
| UC22 | Theo dõi nghệ sĩ | Phase 6 |
| UC23 | Hủy theo dõi | Phase 6 |
| UC24 | Admin xem danh sách bài | Phase 8 |
| UC25 | Upload bài hát | Phase 8 |
| UC26 | Cập nhật bài hát | Phase 8 |
| UC27 | Xóa bài hát | Phase 8 |
| UC28 | Quản lý nghệ sĩ | Phase 8 |
| UC29 | Quản lý Album | Phase 8 |

---

## 🔑 Ghi chú quan trọng cho Agent

1. **Luôn fetch UI từ `https://lyric-lullaby-lane.lovable.app/` trước** để lấy đúng màu sắc, layout, component style. Tái hiện bằng Bootstrap 5 + CSS custom, không dùng Tailwind hay React component.

2. **Stack cứng:** ASP.NET Core MVC (.NET 8) + Entity Framework Core + SQL Server + Razor Views + Bootstrap 5 + jQuery. Không thêm bất kỳ frontend framework nào khác.

3. **3-Layer Architecture bắt buộc:** Controller → Service → Repository → DbContext. Không để logic DB trong Controller.

4. **AutoMapper bắt buộc:** Mọi nơi truyền data từ Entity ra View phải qua DTO và AutoMapper.

5. **Identity tích hợp:** Dùng `[Authorize]`, `[Authorize(Roles="Admin")]`, `User.Identity.Name`, `UserManager`, `SignInManager` — không tự viết auth.

6. **Player bar luôn hiện** trên mọi trang (trừ login/register và Admin area). State player lưu trong JS biến toàn cục.

7. **AJAX cho:** search (debounce 300ms), like/unlike, add-to-playlist — không reload trang.

8. **File upload:** validate extension + size (MP3 ≤ 50MB, ảnh ≤ 5MB), lưu vào `wwwroot/uploads/songs/` và `wwwroot/uploads/covers/`.

9. **Admin area:** route prefix `/admin/`, sidebar riêng, không có player bar, phân quyền Role = "Admin".

10. **Seed data:** phải có đủ dữ liệu mẫu để demo được, bao gồm file MP3 mẫu nhỏ (có thể dùng URL public) hoặc placeholder.
