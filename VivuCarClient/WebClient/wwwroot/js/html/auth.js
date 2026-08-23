(function () {
  const DB = window.VivuCarDB;
  const C = window.VivuCarConstants;
  const U = window.VivuCarUtils;

  const roleRoutes = {
    user: "home.html",
    car_owner: "owner-dashboard.html",
    admin: "admin-dashboard.html"
  };

  function getCurrentUser() {
    const id = Number(localStorage.getItem("vivucar_current_user_id"));
    return DB.users.find((user) => user.id === id) || null;
  }

  function login(email, password) {
    // POST /api/auth/login
    return new Promise((resolve, reject) => {
      setTimeout(() => {
        const user = DB.users.find((item) => item.email === email);
        if (!user || password !== "123456") return reject(new Error("Email hoặc mật khẩu không đúng."));
        if (user.is_blocked) return reject(new Error("Tài khoản của bạn đã bị khóa. Vui lòng liên hệ quản trị viên."));
        localStorage.setItem("vivucar_current_user_id", user.id);
        localStorage.setItem("vivucar_current_user_role", user.role);
        resolve(user);
      }, 420);
    });
  }

  function logout() {
    // POST /api/auth/logout
    localStorage.removeItem("vivucar_current_user_id");
    localStorage.removeItem("vivucar_current_user_role");
    location.href = "/Login";
  }

  function requireAuth(allowedRoles) {
    const role = localStorage.getItem("vivucar_current_user_role");
    const user = getCurrentUser();
    if (!user || user.is_blocked) {
      location.href = "/Login";
      return null;
    }
    if (allowedRoles?.length && !allowedRoles.includes(role)) {
      location.href = "/Login";
      return null;
    }
    return user;
  }

  window.VivuCarAuth = { roleRoutes, getCurrentUser, login, logout, requireAuth };
})();

