import { authService } from "./auth-service.js";

document.addEventListener("DOMContentLoaded", async () => {
    // Active nav link
    const path = window.location.pathname.toLowerCase();
    document.querySelectorAll('.md-flex a').forEach(a => {
        const href = a.getAttribute('href').toLowerCase();
        if (path.startsWith(href)) {
            a.style.color = '#16a34a';
            a.style.fontWeight = '600';
        } else {
            a.style.color = '#52525b';
            a.style.fontWeight = '500';
        }
    });

    // User menu dropdown toggle
    const menuBtn = document.getElementById('navUserMenuBtn');
    const dropdown = document.querySelector('.nav-dropdown');
    if (menuBtn && dropdown) {
        menuBtn.addEventListener('click', (e) => {
            e.stopPropagation();
            const isOpen = dropdown.style.opacity === '1';
            dropdown.style.opacity = isOpen ? '0' : '1';
            dropdown.style.visibility = isOpen ? 'hidden' : 'visible';
        });
        document.addEventListener('click', () => {
            dropdown.style.opacity = '0';
            dropdown.style.visibility = 'hidden';
        });
    }

    // Auth
    try {
        const session = await authService.refresh();
        const user = session?.user;
        if (user) {
            const userState = document.getElementById('navUserState');
            const guestState = document.getElementById('navGuestState');
            if (userState) userState.classList.remove('hidden');
            if (guestState) guestState.classList.add('hidden');

            const nameEl = document.getElementById('navUserName');
            if (nameEl) nameEl.textContent = user.fullName;

            const avatar = document.getElementById('navUserAvatar');
            const initials = document.getElementById('navUserInitials');
            if (user.avatarUrl && avatar && initials) {
                avatar.src = user.avatarUrl;
                avatar.classList.remove('hidden');
                initials.classList.add('hidden');
            } else if (initials) {
                initials.textContent = (user.fullName?.[0] || 'U').toUpperCase();
            }

            const logoutBtn = document.getElementById('navLogoutBtn');
            if (logoutBtn) {
                logoutBtn.addEventListener('click', async () => {
                    await authService.logout();
                    window.location.href = "/";
                });
            }
        } else {
            const userState = document.getElementById('navUserState');
            const guestState = document.getElementById('navGuestState');
            if (userState) userState.classList.add('hidden');
            if (guestState) guestState.classList.remove('hidden');
        }
    } catch(e) {
        console.error("Navbar Auth error", e);
    }
});
