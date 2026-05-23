function addToPlaylist(playlistId, trackId) {
    $.post('/Playlist/AddTrack', { playlistId, trackId }, function(data) {
        if (data.success) {
            alert('Đã thêm bài hát vào danh sách phát!');
        }
    });
}

function removeFromPlaylist(playlistId, trackId, btn) {
    $.post('/Playlist/RemoveTrack', { playlistId, trackId }, function(data) {
        if (data.success) {
            $(btn).closest('tr').fadeOut(300, function() {
                $(this).remove();
            });
        }
    });
}
