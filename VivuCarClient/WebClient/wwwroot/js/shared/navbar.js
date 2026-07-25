import { authService } from "./auth-service.js";

document.addEventListener("DOMContentLoaded", async () => {
    try {
        const session = await authService.refresh();
        const user = session?.user;
        if (user) {
            document.getElementById('navUserState').classList.remove('hidden');
            document.getElementById('navGuestState').classList.add('hidden');
            
            document.getElementById('navUserName').textContent = user.fullName;
            if (user.avatarUrl) {
                document.getElementById('navUserAvatar').src = user.avatarUrl;
                document.getElementById('navUserAvatar').classList.remove('hidden');
                document.getElementById('navUserInitials').classList.add('hidden');
            } else {
                document.getElementById('navUserInitials').textContent = (user.fullName[0] || 'U').toUpperCase();
            }

            document.getElementById('navLogoutBtn').addEventListener('click', async () => {
                await authService.logout();
                window.location.href = "/";
            });
        } else {
            document.getElementById('navUserState').classList.add('hidden');
            document.getElementById('navGuestState').classList.remove('hidden');
        }
    } catch(e) {
        console.error("Navbar Auth error", e);
    }
});
