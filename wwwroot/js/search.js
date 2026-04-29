var searchTimeout = null;
var currentSearchResults = [];
var currentPage = 1;
var currentQuery = "";
var totalPages = 1;

$(document).ready(function() {
    // Initial load of all tracks
    performSearch("", 1, false);
});

// Use event delegation for SPA compatibility
$(document).off('input', '#search-input').on('input', '#search-input', function() {
    const query = $(this).val().trim();

    if (searchTimeout) {
        clearTimeout(searchTimeout);
    }

    currentQuery = query;

    searchTimeout = setTimeout(function() {
        performSearch(query, 1, false);
    }, 1000); // 1s debounce
});

$(document).off('click', '#load-more-btn').on('click', '#load-more-btn', function() {
    if (currentPage < totalPages) {
        performSearch(currentQuery, currentPage + 1, true);
    }
});

function performSearch(query, page, append) {
    $.get('/Search/Results', { q: query, page: page }, function(data) {
        currentPage = data.page;
        totalPages = data.totalPages;
        
        if (append) {
            currentSearchResults = currentSearchResults.concat(data.items);
        } else {
            currentSearchResults = data.items;
        }
        
        renderResults(data.items, append);
    });
}

function renderResults(tracks, append) {
    const tbody = $('#search-results-list');
    
    if (!append) {
        tbody.empty();
    }

    if (!append && tracks.length === 0) {
        $('#no-results').text('Không tìm thấy kết quả phù hợp.').show();
        $('.track-table-container').hide();
        $('#load-more-container').hide();
        return;
    }

    $('#no-results').hide();
    $('.track-table-container').show();

    const startIndex = append ? currentSearchResults.length - tracks.length : 0;

    tracks.forEach((track, index) => {
        const globalIndex = startIndex + index;
        const row = $(`
            <tr class="track-row hover:bg-white/5 transition cursor-pointer" data-track-id="${track.trackId}">
                <td class="px-4 py-3 text-muted-foreground">${globalIndex + 1}</td>
                <td class="px-4 py-3">
                    <div class="flex items-center gap-3">
                        <img src="${track.coverImage || '/uploads/covers/default.png'}" onerror="this.src='https://images.unsplash.com/photo-1514525253161-7a46d19cd819?w=100&h=100&fit=crop'" class="size-10 rounded object-cover" alt="" />
                        <div class="flex flex-col">
                            <span class="font-medium text-foreground track-title-cell">${track.title}</span>
                            <span class="text-xs text-muted-foreground">${track.artistName}</span>
                        </div>
                    </div>
                </td>
                <td class="px-4 py-3 text-muted-foreground">${track.genre || ''}</td>
                <td class="px-4 py-3 text-right text-muted-foreground">${formatTime(track.duration)}</td>
            </tr>
        `);

        row.on('click', function() {
            setPlaylist(currentSearchResults, globalIndex);
        });

        tbody.append(row);
    });

    if (typeof window.currentTrackId !== 'undefined' && window.currentTrackId) {
        $(`.track-row[data-track-id="${window.currentTrackId}"]`).addClass('playing');
        $(`.track-row[data-track-id="${window.currentTrackId}"] .track-title-cell`).addClass('text-lime').removeClass('text-foreground');
    }

    if (currentPage < totalPages) {
        if ($('#load-more-container').length === 0) {
            $('.track-table-container').after(`
                <div id="load-more-container" class="mt-4 flex justify-center">
                    <button id="load-more-btn" class="px-6 py-2 rounded-full border border-border/60 hover:bg-white/5 transition font-medium text-sm">Xem thêm</button>
                </div>
            `);
        }
        $('#load-more-container').show();
    } else {
        $('#load-more-container').hide();
    }
}

function formatTime(secs) {
    const minutes = Math.floor(secs / 60) || 0;
    const seconds = Math.floor(secs % 60) || 0;
    return minutes + ':' + (seconds < 10 ? '0' : '') + seconds;
}
