# Layout Upgrade to Modern Premium Design

Modify the layout of the Melodify music player to match the mockup HTML/CSS layout exactly, while maintaining all existing backend APIs, C# models, and interactivity.

## User Review Required

> [!IMPORTANT]
> **Tailwind CSS Integration Method**
> The provided mockup uses Tailwind CSS classes (e.g., `flex-col`, `bg-sidebar`, `size-9`, `gap-2`, etc.). We propose loading Tailwind CSS via the official Play CDN (`https://cdn.tailwindcss.com`) in `_Layout.cshtml` and configuring it with the required custom design tokens (`bg-background`, `bg-sidebar`, `bg-lime`, etc.) to exactly match the look of the mock.
> If you prefer a different layout technology (like raw Bootstrap or manual CSS compilation), please let us know.

> [!WARNING]
> **Strict Coding Rules**
> In compliance with your global rules:
> 1. No code comments will be added to any C#, CSS, JS, or CSHTML files.
> 2. No emojis/icons will be added in code comments or logs.
> 3. Standard clean architecture, low coupling, high cohesion, and SOLID principles will be followed.

## Open Questions

- **Tailwind CSS Version**: Do you have a specific Tailwind CSS version preference? By default, the Play CDN loads Tailwind CSS v3.
- **Search bar and User Profile logic**: In the mockup header, there is a navigation button set, an empty space, and a user profile indicator ("MK"). We plan to map the search input container to this header space when `ViewData["ShowSearch"]` is true, and show the initials of the currently logged-in user in the avatar bubble. Is this aligned with your expectations?

## Proposed Changes

### Front-end Layout & Styling

#### [MODIFY] [_Layout.cshtml](file:///Users/plxg/workspace/MELODIFY/Views/Shared/_Layout.cshtml)
- Integrate Tailwind CSS via CDN with customized configuration mapping to variables:
  - `background`: `#09090b`
  - `foreground`: `#fafafa`
  - `sidebar`: `{ DEFAULT: '#18181b', border: '#27272a', accent: '#27272a', foreground: '#fafafa' }`
  - `lime`: `{ DEFAULT: '#adfa1d', foreground: '#09090b' }`
- Refactor the core shell:
  - Top header layout: navigation buttons, search container (if `ShowSearch` is active), and user profile badge/logout form.
  - Page content container with dynamic scroll.
  - Keeps modal `#createPlaylistModal` and references to necessary JS scripts (`player.js`, `spa.js`).

#### [MODIFY] [_Sidebar.cshtml](file:///Users/plxg/workspace/MELODIFY/Views/Shared/_Sidebar.cshtml)
- Redesign the sidebar structure according to the mockup:
  - Melodify logo with custom styling.
  - Navigation links: Home, Search, Library with icons and exact hover classes.
  - "Playlist của bạn" section header, "Tạo playlist" button triggering the modal, and "Bài đã thích" link.
  - Render actual playlists dynamically using the view component.

#### [MODIFY] [Default.cshtml](file:///Users/plxg/workspace/MELODIFY/Views/Shared/Components/SidebarPlaylists/Default.cshtml)
- Update CSS classes of output links to match the exact list layout of the sidebar playlists from the mockup.

#### [MODIFY] [_PlayerBar.cshtml](file:///Users/plxg/workspace/MELODIFY/Views/Shared/_PlayerBar.cshtml)
- Rebuild the bottom player bar using the grid layout of the mockup:
  - Left column: Track image, metadata (title, artist), and like heart button.
  - Center column: Control buttons (shuffle, back, play/pause, forward, repeat) and seek bar container.
  - Right column: Volume button and volume progress bar container.
  - Maintain all IDs and click bindings to match `player.js` logic.

#### [MODIFY] [Index.cshtml](file:///Users/plxg/workspace/MELODIFY/Views/Home/Index.cshtml)
- Recreate the main home screen content matching the mock layout:
  - Welcome Banner: gradient background, blur blobs, greeting text, filter chips ("Tất cả", "Nhạc", "Podcast").
  - "Mới phát gần đây": grid containing the user's recent albums.
  - "Khám phá hôm nay": grid containing featured tracks with hover play buttons.
  - "Nghệ sĩ bạn có thể thích": list of suggested artists in a row.
  - "Playlist nổi bật": grid of featured playlists.
- Use current C# Models instead of hardcoded/mocked data.

#### [MODIFY] [site.css](file:///Users/plxg/workspace/MELODIFY/wwwroot/css/site.css)
- Add standard styles to handle base fonts, dark mode scrollbar styling, and ensure compatibility for existing pages (e.g. search, details, etc.) under the new Tailwind layouts.

## Verification Plan

### Automated Tests
- Build and run the ASP.NET Core project:
  `dotnet run`

### Manual Verification
- Visual inspection of the upgraded UI locally.
- Test routing and page navigation using the SPA scripts.
- Validate player functionality: play, pause, progress bar click, volume adjustment, and liking a track.
