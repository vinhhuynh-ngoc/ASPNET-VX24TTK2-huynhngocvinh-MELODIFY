let audioPlayer = new Audio();
let currentTrackId = null;
let playlistTracks = [];
let currentPlaylistIndex = -1;
let isShuffle = false;
let isRepeat = false;

$(document).ready(function() {
    audioPlayer.volume = 0.5;
    updateVolumeUI(0.5);

    $('#play-pause-btn').on('click', function() {
        togglePlay();
    });

    $('#player-like-btn').on('click', function() {
        if (currentTrackId) {
            window.toggleLike(this, currentTrackId);
        }
    });

    $('#prev-btn').on('click', function() {
        prevTrack();
    });

    $('#next-btn').on('click', function() {
        nextTrack();
    });

    $('#shuffle-btn').on('click', function() {
        isShuffle = !isShuffle;
        $(this).toggleClass('active', isShuffle);
    });

    $('#repeat-btn').on('click', function() {
        isRepeat = !isRepeat;
        $(this).toggleClass('active', isRepeat);
    });

    $('.progress-bar-wrapper').first().on('click', function(e) {
        if (!audioPlayer.src) return;
        const rect = this.getBoundingClientRect();
        const clickX = e.clientX - rect.left;
        const percentage = clickX / rect.width;
        audioPlayer.currentTime = percentage * audioPlayer.duration;
    });

    $('.volume-container .progress-bar-wrapper').on('click', function(e) {
        const rect = this.getBoundingClientRect();
        const clickX = e.clientX - rect.left;
        let volume = clickX / rect.width;
        volume = Math.max(0, Math.min(1, volume));
        audioPlayer.volume = volume;
        updateVolumeUI(volume);
    });

    audioPlayer.addEventListener('timeupdate', function() {
        if (isNaN(audioPlayer.duration)) return;
        const current = audioPlayer.currentTime;
        const total = audioPlayer.duration;
        const percentage = (current / total) * 100;
        $('#progress-bar-fill-track').css('width', percentage + '%');
        $('#time-elapsed').text(formatTime(current));
        $('#time-duration').text(formatTime(total));
    });

    audioPlayer.addEventListener('ended', function() {
        if (isRepeat) {
            audioPlayer.currentTime = 0;
            audioPlayer.play();
        } else {
            nextTrack();
        }
    });
});

window.playTrack = function(trackId, audioUrl, title, artist, coverUrl, isLiked) {
    currentTrackId = trackId;
    window.currentTrackId = trackId;
    audioPlayer.src = audioUrl;
    audioPlayer.play();

    $('#player-bar-cover').attr('src', coverUrl || '/uploads/covers/default.png');
    $('#player-bar-title').text(title);
    $('#player-bar-artist').text(artist);
    $('#play-pause-btn').html('<i class="bi bi-pause-fill"></i>');

    $('.track-row').removeClass('playing');
    $(`.track-row[data-track-id="${trackId}"]`).addClass('playing');

    updateLikeButton(trackId, isLiked);
    incrementPlayCount(trackId);
};

window.setPlaylist = function(tracks, startIndex) {
    playlistTracks = tracks;
    currentPlaylistIndex = startIndex;
    if (playlistTracks.length > 0 && startIndex >= 0) {
        const track = playlistTracks[startIndex];
        playTrack(track.trackId, track.audioUrl, track.title, track.artistName, track.coverImage, track.isLiked);
    }
};

function togglePlay() {
    if (!audioPlayer.src) return;
    if (audioPlayer.paused) {
        audioPlayer.play();
        $('#play-pause-btn').html('<i class="bi bi-pause-fill"></i>');
    } else {
        audioPlayer.pause();
        $('#play-pause-btn').html('<i class="bi bi-play-fill"></i>');
    }
}

function nextTrack() {
    if (playlistTracks.length === 0) return;
    if (isShuffle) {
        currentPlaylistIndex = Math.floor(Math.random() * playlistTracks.length);
    } else {
        currentPlaylistIndex = (currentPlaylistIndex + 1) % playlistTracks.length;
    }
    const track = playlistTracks[currentPlaylistIndex];
    playTrack(track.trackId, track.audioUrl, track.title, track.artistName, track.coverImage, track.isLiked);
}

function prevTrack() {
    if (playlistTracks.length === 0) return;
    currentPlaylistIndex = (currentPlaylistIndex - 1 + playlistTracks.length) % playlistTracks.length;
    const track = playlistTracks[currentPlaylistIndex];
    playTrack(track.trackId, track.audioUrl, track.title, track.artistName, track.coverImage, track.isLiked);
}

function updateVolumeUI(volume) {
    const percentage = volume * 100;
    $('#volume-bar-fill').css('width', percentage + '%');
    if (volume === 0) {
        $('#volume-icon').removeClass().addClass('bi bi-volume-mute-fill');
    } else if (volume < 0.5) {
        $('#volume-icon').removeClass().addClass('bi bi-volume-down-fill');
    } else {
        $('#volume-icon').removeClass().addClass('bi bi-volume-up-fill');
    }
}

function formatTime(secs) {
    const minutes = Math.floor(secs / 60) || 0;
    const seconds = Math.floor(secs % 60) || 0;
    return minutes + ':' + (seconds < 10 ? '0' : '') + seconds;
}

function updateLikeButton(trackId, isLiked) {
    if (isLiked !== undefined && isLiked !== null) {
        if (isLiked) {
            $('#player-like-btn').addClass('liked').html('<i class="bi bi-heart-fill"></i>');
        } else {
            $('#player-like-btn').removeClass('liked').html('<i class="bi bi-heart"></i>');
        }
    } else {
        const rowHeart = $(`.btn-like-track[data-track-id="${trackId}"]`);
        if (rowHeart.length > 0) {
            const hasLiked = rowHeart.hasClass('text-success') || rowHeart.hasClass('liked');
            if (hasLiked) {
                $('#player-like-btn').addClass('liked').html('<i class="bi bi-heart-fill"></i>');
            } else {
                $('#player-like-btn').removeClass('liked').html('<i class="bi bi-heart"></i>');
            }
        } else {
            $.get('/Library/IsLiked', { trackId }, function(data) {
                if (data.liked) {
                    $('#player-like-btn').addClass('liked').html('<i class="bi bi-heart-fill"></i>');
                } else {
                    $('#player-like-btn').removeClass('liked').html('<i class="bi bi-heart"></i>');
                }
            });
        }
    }
}

function incrementPlayCount(trackId) {
    $.post('/Home/IncrementPlayCount', { trackId });
}

window.toggleLike = function(btn, trackId) {
    const isLiked = $(btn).hasClass('text-success') || $(btn).hasClass('liked');
    const url = isLiked ? '/Library/Unlike' : '/Library/Like';

    $.post(url, { trackId }, function(data) {
        if (data.success) {
            if (isLiked) {
                $(`.btn-like-track[data-track-id="${trackId}"]`).removeClass('text-success').find('i').removeClass('bi-heart-fill').addClass('bi-heart');
                if (currentTrackId == trackId) {
                    $('#player-like-btn').removeClass('liked').html('<i class="bi bi-heart"></i>');
                }
            } else {
                $(`.btn-like-track[data-track-id="${trackId}"]`).addClass('text-success').find('i').removeClass('bi-heart').addClass('bi-heart-fill');
                if (currentTrackId == trackId) {
                    $('#player-like-btn').addClass('liked').html('<i class="bi bi-heart-fill"></i>');
                }
            }
        }
    });
};
