(function () {
  const Auth = window.VivuCarAuth;
  const U = window.VivuCarUtils;
  const form = U.byId("loginForm");
  const errorBox = U.byId("errorMessage");
  const btn = U.byId("btnLogin");
  const loading = U.byId("loadingState");

  document.querySelectorAll("[data-demo-email]").forEach((item) => {
    item.addEventListener("click", () => {
      U.byId("email").value = item.dataset.demoEmail;
      U.byId("password").value = "123456";
    });
  });

  form.addEventListener("submit", async (event) => {
    event.preventDefault();
    errorBox.classList.add("hidden");
    const email = U.byId("email").value.trim();
    const password = U.byId("password").value;
    if (!/^\S+@\S+\.\S+$/.test(email)) return showError("Email không hợp lệ.");
    if (password.length < 6) return showError("Mật khẩu tối thiểu 6 ký tự.");
    btn.disabled = true;
    btn.textContent = "Đang đăng nhập...";
    loading.classList.remove("hidden");
    try {
      const user = await Auth.login(email, password);
      location.href = Auth.roleRoutes[user.role];
    } catch (error) {
      showError(error.message);
    } finally {
      btn.disabled = false;
      btn.textContent = "Đăng nhập";
      loading.classList.add("hidden");
    }
  });

  function showError(message) {
    errorBox.textContent = message;
    errorBox.classList.remove("hidden");
  }
})();

