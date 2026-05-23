window.navigateTo = function(url, addToHistory = true) {
    $.ajax({
        url: url,
        method: 'GET',
        success: function(html) {
            const parser = new DOMParser();
            const doc = parser.parseFromString(html, 'text/html');
            const mainContent = doc.querySelector('.main-content');
            if (mainContent) {
                $('.main-content').html(mainContent.innerHTML);
                document.title = doc.title;
                if (addToHistory) {
                    history.pushState(null, '', url);
                }
                
                document.querySelectorAll('.dynamic-script').forEach(el => el.remove());

                const scripts = doc.querySelectorAll('script');
                scripts.forEach(script => {
                    if (script.src && (script.src.includes('jquery') || script.src.includes('bootstrap') || script.src.includes('player.js') || script.src.includes('spa.js'))) {
                        return;
                    }
                    const newScript = document.createElement('script');
                    newScript.classList.add('dynamic-script');
                    if (script.src) {
                        newScript.src = script.src;
                    } else {
                        newScript.textContent = script.textContent;
                    }
                    document.body.appendChild(newScript);
                });

                $('.main-content').scrollTop(0);
                updateActiveSidebarLink(url);
            }
        },
        error: function() {
            window.location.href = url;
        }
    });
};

function updateActiveSidebarLink(url) {
    const path = url.split('?')[0];
    $('.sidebar-nav-item, .playlist-item').removeClass('active');
    $(`.sidebar-nav-item[href="${path}"], .playlist-item[href="${url}"]`).addClass('active');
}

$(document).on('click', 'a', function(e) {
    const href = $(this).attr('href');
    if (!href) return;
    
    if (href.startsWith('#') || href.startsWith('javascript:')) return;
    
    const isExternal = href.startsWith('http://') || href.startsWith('https://') || href.startsWith('//');
    if (isExternal && !href.includes(window.location.host)) return;
    
    if (href.includes('/Admin') || href.includes('/Account/Logout') || href.includes('/Account/Login') || href.includes('/Account/Register')) {
        return;
    }
    
    e.preventDefault();
    window.navigateTo(href);
});

window.addEventListener('popstate', function() {
    window.navigateTo(window.location.pathname + window.location.search, false);
});

$(document).ready(function() {
    updateActiveSidebarLink(window.location.pathname + window.location.search);
});
