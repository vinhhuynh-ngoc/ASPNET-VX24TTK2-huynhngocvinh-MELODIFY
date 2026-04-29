# Audio Playback, SPA Navigation, and Playback Bar Fixes Walkthrough

This document details the diagnostic, resolution, and enhancement steps implemented to ensure a modern and smooth music-streaming user experience on Melodify (localhost:5103).

## Issues Solved

### 1. Playback Crash on Navigation (UX Enhancement)
- **Problem**: When a user was listening to a song and clicked on links (like Search, Home, Library) or album cards, it triggered a full browser page reload. This destroyed the HTML5 `<audio>` player state, immediately interrupting and stopping the audio playback.
- **Solution**: Implemented a lightweight client-side Single Page Application (SPA) router (`spa.js`):
  - Intercepts all internal navigation link clicks (`a[href]`) and programmatic redirects.
  - Dynamically fetches the destination HTML page via AJAX (`$.ajax` GET).
  - Updates only the `.main-content` container (which includes the dynamic view body and the `top-bar`), keeping the left sidebar and bottom player bar fully untouched.
  - Uses the HTML5 History API (`history.pushState` and `popstate` events) to update the browser URL and support back/forward button navigation.
  - Safely extracts and executes page-specific scripts for views without duplicating DOM elements or causing conflicts.

### 2. Global Variable Redeclaration Conflicts
- **Problem**: Page-specific scripts in Razor views declared track arrays globally using `const` (e.g., `const albumTracks = ...`). When navigating between views dynamically, executing these scripts repeatedly threw a javascript error: `Uncaught SyntaxError: Identifier 'albumTracks' has already been declared`.
- **Solution**: Converted top-level script variables from `const` to `var` in all Razor views. Since `var` allows redeclaration in JavaScript, this prevents any compilation/runtime errors during consecutive navigation cycles.

### 3. Case-insensitive Unicode Search (Vietnamese Accents)
- **Problem**: Searching for tracks using Vietnamese accented characters (e.g. searching for "Lễ") did not yield results under SQLite if there was a case mismatch (e.g. typed lowercase "lễ" vs capitalized title "Lễ Đường"). SQLite's default `LIKE` and `Contains` operations are case-sensitive for Unicode/non-ASCII characters.
- **Solution**: Refactored `SearchAsync` in [TrackRepository.cs](file:///Users/plxg/workspace/MELODIFY/Repositories/TrackRepository.cs) to retrieve tracks and perform a case-insensitive in-memory comparison via `.Contains(..., StringComparison.OrdinalIgnoreCase)`.

### 4. Playback Bar Heart (Like) Button Issues
- **Problem**: The heart button on the persistent bottom playback bar (`#player-like-btn`) was bound inside `playlist.js`, which was only loaded on detail views. This made the heart button completely non-responsive on the Home view. Furthermore, each transition to a detail view appended another duplicate click handler to the persistent heart button, causing multiple AJAX requests to trigger on a single click.
- **Solution**:
  - Moved the `toggleLike` function and the `#player-like-btn` click event listener registration into [player.js](file:///Users/plxg/workspace/MELODIFY/wwwroot/js/player.js), which is loaded once globally and remains persistent.
  - Cleaned up [playlist.js](file:///Users/plxg/workspace/MELODIFY/wwwroot/js/playlist.js) to keep only the detail-view specific playlist handlers.

### 5. PascalCase Key Serialization Mismatch
- **Problem**: Razor views serialized track lists to JSON without camelCase naming policies, causing property naming mismatches (e.g., `track.AudioUrl` in JSON vs `track.audioUrl` in `player.js`). This resulted in `undefined` audio URLs and 404 errors.
- **Solution**: Configured `JsonSerializerOptions` with `JsonNamingPolicy.CamelCase` for all view-based JSON serializations.

### 6. Missing HTTP Range Request Support
- **Problem**: Dynamically uploaded MP3 files under `wwwroot/uploads/` failed to play in modern browsers, returning a `416 Range Not Satisfiable` error, because the server did not register static file middleware to process range requests.
- **Solution**: Added `app.UseStaticFiles()` middleware in `Program.cs`.

---

## Implemented Changes

### 1. SPA Router Script
- Created [spa.js](file:///Users/plxg/workspace/MELODIFY/wwwroot/js/spa.js) containing click interception, dynamic content loading, browser history syncing, and script execution logic.
- Included it in [_Layout.cshtml](file:///Users/plxg/workspace/MELODIFY/Views/Shared/_Layout.cshtml).

### 2. View and Script Updates
- Changed top-level `const` declarations to `var` in the following files:
  - **Home View**: [Index.cshtml](file:///Users/plxg/workspace/MELODIFY/Views/Home/Index.cshtml)
  - **Album View**: [Detail.cshtml](file:///Users/plxg/workspace/MELODIFY/Views/Album/Detail.cshtml)
  - **Playlist View**: [Detail.cshtml](file:///Users/plxg/workspace/MELODIFY/Views/Playlist/Detail.cshtml)
  - **Artist View**: [Detail.cshtml](file:///Users/plxg/workspace/MELODIFY/Views/Artist/Detail.cshtml)
  - **Library View**: [Index.cshtml](file:///Users/plxg/workspace/MELODIFY/Views/Library/Index.cshtml)
- Updated card `onclick` event handlers from `location.href = ...` to `navigateTo(...)` in `Home/Index.cshtml`, `Artist/Detail.cshtml`, and `Library/Index.cshtml`.

### 3. Middleware and Code Configurations
- Added `app.UseStaticFiles()` right after `app.UseHttpsRedirection()` in [Program.cs](file:///Users/plxg/workspace/MELODIFY/Program.cs).
- Refactored `SearchAsync` inside [TrackRepository.cs](file:///Users/plxg/workspace/MELODIFY/Repositories/TrackRepository.cs) for case-insensitive Unicode searches.

## Verification and Testing

- Recompiled and restarted the `dotnet` server.
- Executed browser verification flows via the browser subagent:
  1. Authenticated successfully.
  2. Searched for track queries containing case-insensitive accented letters, returning results immediately.
  3. Played a song from the search results, loading properly into the player bar.
  4. Licked the heart icon on the player bar, confirming the liked status gets stored via AJAX (turning green and changing class to `liked`) without multiple duplicate requests or missing handlers on the Home view.
