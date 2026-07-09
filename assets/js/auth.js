(function () {
  const DB = window.VivuCarDB;
  const C = window.VivuCarConstants;
  const U = window.VivuCarUtils;

  const roleRoutes = {
    user: "home.html",
    car_owner: "owner-booking-requests.html",
    admin: "admin-dashboard.html"
  };

  function getCurrentUser() {
    const userStr = localStorage.getItem("vivucar_current_user");
    if (!userStr) return null;
    try {
      return JSON.parse(userStr);
    } catch {
      return null;
    }
  }

  async function login(email, password) {
    const res = await fetch(`${C.API_BASE_URL}/auth/login`, {
      method: "POST",
      headers: { "Content-Type": "application/json" },
      body: JSON.stringify({ email, password })
    });
    const data = await res.json();
    if (!res.ok) throw new Error(data.message || "Đăng nhập thất bại.");
    
    // Lưu token và user info
    localStorage.setItem("vivucar_access_token", data.accessToken);
    localStorage.setItem("vivucar_current_user", JSON.stringify(data.user));
    localStorage.setItem("vivucar_current_user_role", data.user.role);
    return data.user;
  }

  async function logout() {
    try {
      await fetchWithAuth(`${C.API_BASE_URL}/auth/logout`, { method: "POST" });
    } catch (err) {
      console.warn("Logout error:", err);
    }
    localStorage.removeItem("vivucar_access_token");
    localStorage.removeItem("vivucar_current_user");
    localStorage.removeItem("vivucar_current_user_role");
    location.href = "login.html";
  }

  function requireAuth(allowedRoles) {
    const role = localStorage.getItem("vivucar_current_user_role");
    const user = getCurrentUser();
    if (!user) {
      location.href = "login.html";
      return null;
    }
    if (allowedRoles?.length && !allowedRoles.includes(role)) {
      location.href = "login.html";
      return null;
    }
    return user;
  }

  async function fetchWithAuth(url, options = {}) {
    const token = localStorage.getItem("vivucar_access_token");
    const headers = new Headers(options.headers || {});
    if (token) {
      headers.set("Authorization", `Bearer ${token}`);
    }
    const config = {
      ...options,
      headers
    };
    const response = await fetch(url, config);
    if (response.status === 401) {
      // Token hết hạn hoặc không hợp lệ -> Đăng xuất
      localStorage.removeItem("vivucar_access_token");
      localStorage.removeItem("vivucar_current_user");
      localStorage.removeItem("vivucar_current_user_role");
      location.href = "login.html";
      throw new Error("Phiên đăng nhập đã hết hạn.");
    }
    return response;
  }

  window.VivuCarAuth = { roleRoutes, getCurrentUser, login, logout, requireAuth, fetchWithAuth };
})();
