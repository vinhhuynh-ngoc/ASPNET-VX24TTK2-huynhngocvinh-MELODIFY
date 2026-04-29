# Melodify Project Walkthrough

This document outlines the changes made to develop the **Melodify** music streaming application using ASP.NET Core MVC.

## Architectural Design

The project is structured following clean coding principles with separation of concerns:
- **Presentation Layer (MVC)**: Razor Views, ViewComponents, and Controllers. Handles routing and UI rendering.
- **Service Layer**: Manages business logic, file upload validations, and database flow operations.
- **Repository Layer**: Abstracts database operations using EF Core.
- **Data Layer (DbContext)**: Configures SQLite database connections and entity relationships.
- **Data Transfer Objects (DTOs)**: AutoMapper translates complex database entities into lightweight DTOs for safe view transport.

## Completed Features

### 1. Database & Seeding (Phase 0)
- Configured SQLite database connection (`Melodify.db`) to ensure platform-independent development on macOS/Windows.
- Applied EF Core migrations.
- Set up database seeding (`SeedData.cs`) with 5 verification-ready Artists, 5 Albums, 9 Tracks with public audio streams, and pre-configured User/Admin accounts.

### 2. Layout & Styling (Phase 1)
- Implemented AppShell (`_Layout.cshtml`) splitting the layout into a sticky sidebar and bottom player bar.
- Configured HTML5 Audio player in `player.js` supporting play/pause toggles, skip forward/backward, shuffle, repeat, seek progress, and volume changes.
- Designed premium dark UI in `site.css` aligning with target project mockups.

### 3. Authentication (Phase 2)
- Integrated ASP.NET Core Identity with customizable password settings.
- Designed login, registration, and access denied views.

### 4. Core Streaming Views (Phases 3 to 7)
- **Home View**: Dynamic time-based greetings, showing recently played items, verified artist suggestions, and user playlists.
- **Search View**: Categorized genre grids and instant search query tables with a 300ms debounce buffer.
- **Detail Views**: Full-featured detail layouts for Album and Playlist, supporting playback queue loading, AJAX liking, and playlist addition.
- **Artist View**: Dynamic verify check badge, top 5 popular tracks by play count, bio information, and follow/unfollow AJAX handlers.
- **Library View**: Tabbed layout showing playlists and liked tracks.

### 5. Admin Area (Phase 8)
- Designed separate dashboard layouts under the `/Admin` route.
- Track CRUD: Secure track creation with size restrictions (Audio <= 50MB, Image <= 5MB), mock progress bars, and metadata updating.
- Artist CRUD & Album CRUD: Complete management forms supporting thumbnail uploads and file updates.
