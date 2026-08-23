// Login page - LearnMate-style auth
(function () {
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

  const form = document.getElementById('loginForm');
  const emailInput = document.getElementById('email');
  const passwordInput = document.getElementById('password');
  const emailError = document.getElementById('emailError');
  const passwordError = document.getElementById('passwordError');
  const globalError = document.getElementById('globalError');
  const submitBtn = document.getElementById('loginBtn');
  const submitText = document.getElementById('loginBtnText');
  const spinner = document.getElementById('loginSpinner');
  const eyeBtn = document.getElementById('passwordEye');
  const forgotLink = document.getElementById('forgotLink');

  // Password toggle
  if (eyeBtn) {
    eyeBtn.addEventListener('click', function () {
      const isPassword = passwordInput.type === 'password';
      passwordInput.type = isPassword ? 'text' : 'password';
      eyeBtn.innerHTML = isPassword
        ? '<svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M9.88 9.88a3 3 0 1 0 4.24 4.24"/><path d="M10.73 5.08A10.43 10.43 0 0 1 12 5c7 0 10 7 10 7a13.16 13.16 0 0 1-1.67 2.68"/><path d="M6.61 6.61A13.526 13.526 0 0 0 2 12s3 7 10 7a9.74 9.74 0 0 0 5.39-1.61"/><line x1="2" x2="22" y1="2" y2="22"/></svg>'
        : '<svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M2 12s3-7 10-7 10 7 10 7-3 7-10 7-10-7-10-7Z"/><circle cx="12" cy="12" r="3"/></svg>';
    });
  }

  // Clear inline errors on input
  emailInput.addEventListener('input', function () {
    emailError.textContent = '';
    emailInput.classList.remove('lm-input--error');
    globalError.textContent = '';
    globalError.style.display = 'none';
  });
  passwordInput.addEventListener('input', function () {
    passwordError.textContent = '';
    passwordInput.classList.remove('lm-input--error');
    globalError.textContent = '';
    globalError.style.display = 'none';
  });

  // Submit
  form.addEventListener('submit', async function (e) {
    e.preventDefault();
    let hasError = false;

    emailError.textContent = '';
    passwordError.textContent = '';
    emailInput.classList.remove('lm-input--error');
    passwordInput.classList.remove('lm-input--error');
    globalError.textContent = '';
    globalError.style.display = 'none';

    const email = emailInput.value.trim();
    const password = passwordInput.value;

    if (!email) {
      emailError.textContent = 'Email is required.';
      emailInput.classList.add('lm-input--error');
      hasError = true;
    } else if (!emailRegex.test(email)) {
      emailError.textContent = 'Please enter a valid email address.';
      emailInput.classList.add('lm-input--error');
      hasError = true;
    }

    if (!password) {
      passwordError.textContent = 'Password is required.';
      passwordInput.classList.add('lm-input--error');
      hasError = true;
    }

    if (hasError) return;

    // Loading state
    submitBtn.disabled = true;
    submitText.style.display = 'none';
    spinner.style.display = 'inline-block';

    try {
      const resp = await fetch('/api/auth/login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email, password })
      });
      const result = await resp.json();

      if (resp.ok) {
        const u = result.user || {};
        sessionStorage.setItem('vc_email', u.email || email);
        sessionStorage.setItem('vc_role', u.role || 'user');
        sessionStorage.setItem('vc_token', result.accessToken || '');
        sessionStorage.setItem('vc_userid', u.id || '');
        if (u.fullName) sessionStorage.setItem('vc_fullname', u.fullName);

        // Also persist to localStorage for auth-service.js (admin/owner layouts)
        if (result.accessToken) {
          localStorage.setItem('vivucar_token', result.accessToken);
        }
        localStorage.setItem('vivucar_token_exp', result.expiresAt || '');

        const role = (u.role || '').toLowerCase();
        const dest = role === 'admin' ? '/Admin/Dashboard'
          : (role === 'car_owner' || role === 'carowner') ? '/Owner/Dashboard'
          : '/home';
        window.location.href = dest;
      } else {
        globalError.textContent = result.message || 'Invalid email or password.';
        globalError.style.display = 'block';
      }
    } catch (err) {
      globalError.textContent = 'Connection error. Please try again.';
      globalError.style.display = 'block';
    } finally {
      submitBtn.disabled = false;
      submitText.style.display = 'inline';
      spinner.style.display = 'none';
    }
  });

  // Avatar modal
  const avatarBtns = document.querySelectorAll('.lm-avatar-btn[data-email]');
  const modalOverlay = document.getElementById('accountModal');
  const modalClose = document.getElementById('modalClose');
  const modalName = document.getElementById('modalName');
  const modalEmail = document.getElementById('modalEmail');
  const modalAvatar = document.getElementById('modalAvatar');
  const modalPass = document.getElementById('modalPass');
  const modalPassError = document.getElementById('modalPassError');
  const modalContinue = document.getElementById('modalContinue');
  const modalContinueText = document.getElementById('modalContinueText');
  const modalContinueSpin = document.getElementById('modalContinueSpin');
  const modalEye = document.getElementById('modalEye');
  const notYouBtn = document.getElementById('notYouBtn');
  const useOtherBtn = document.getElementById('useOtherBtn');

  let selectedAccount = null;

  function maskEmail(email) {
    const at = email.indexOf('@');
    if (at <= 2) return email;
    return email.slice(0, 2) + '*'.repeat(at - 2) + email.slice(at);
  }

  avatarBtns.forEach(function (btn) {
    btn.addEventListener('click', function () {
      selectedAccount = {
        name: btn.dataset.name,
        email: btn.dataset.email,
        avatar: btn.dataset.avatar
      };
      modalName.textContent = selectedAccount.name;
      modalEmail.textContent = maskEmail(selectedAccount.email);
      modalAvatar.src = selectedAccount.avatar;
      modalPass.value = '';
      modalPassError.textContent = '';
      modalPass.classList.remove('lm-modal-input--error');
      modalOverlay.style.display = 'flex';
    });
  });

  function closeModal() {
    modalOverlay.style.display = 'none';
    selectedAccount = null;
  }

  if (modalClose) modalClose.addEventListener('click', closeModal);
  if (notYouBtn) notYouBtn.addEventListener('click', closeModal);
  if (useOtherBtn) useOtherBtn.addEventListener('click', closeModal);
  modalOverlay.addEventListener('click', function (e) {
    if (e.target === modalOverlay) closeModal();
  });

  if (modalEye) {
    modalEye.addEventListener('click', function () {
      const isPass = modalPass.type === 'password';
      modalPass.type = isPass ? 'text' : 'password';
      modalEye.innerHTML = isPass
        ? '<svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M9.88 9.88a3 3 0 1 0 4.24 4.24"/><path d="M10.73 5.08A10.43 10.43 0 0 1 12 5c7 0 10 7 10 7a13.16 13.16 0 0 1-1.67 2.68"/><path d="M6.61 6.61A13.526 13.526 0 0 0 2 12s3 7 10 7a9.74 9.74 0 0 0 5.39-1.61"/><line x1="2" x2="22" y1="2" y2="22"/></svg>'
        : '<svg xmlns="http://www.w3.org/2000/svg" width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M2 12s3-7 10-7 10 7 10 7-3 7-10 7-10-7-10-7Z"/><circle cx="12" cy="12" r="3"/></svg>';
    });
  }

  modalContinue.addEventListener('click', async function () {
    const pass = modalPass.value;
    if (!pass) {
      modalPassError.textContent = 'Password is required.';
      modalPass.classList.add('lm-modal-input--error');
      return;
    }
    modalPassError.textContent = '';
    modalPass.classList.remove('lm-modal-input--error');

    modalContinue.disabled = true;
    modalContinueText.style.display = 'none';
    modalContinueSpin.style.display = 'inline-block';

    try {
      const resp = await fetch('/api/auth/login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email: selectedAccount.email, password: pass })
      });
      const result = await resp.json();

      if (resp.ok) {
        const u = result.user || {};
        sessionStorage.setItem('vc_email', u.email || selectedAccount.email);
        sessionStorage.setItem('vc_role', u.role || 'user');
        sessionStorage.setItem('vc_token', result.accessToken || '');
        sessionStorage.setItem('vc_userid', u.id || '');
        if (u.fullName) sessionStorage.setItem('vc_fullname', u.fullName);

        // Also persist to localStorage for auth-service.js (admin/owner layouts)
        if (result.accessToken) {
          localStorage.setItem('vivucar_token', result.accessToken);
        }
        localStorage.setItem('vivucar_token_exp', result.expiresAt || '');

        const role = (u.role || '').toLowerCase();
        const dest = role === 'admin' ? '/Admin/Dashboard'
          : (role === 'car_owner' || role === 'carowner') ? '/Owner/Dashboard'
          : '/home';
        window.location.href = dest;
      } else {
        modalPassError.textContent = result.message || 'Incorrect password.';
        modalPass.classList.add('lm-modal-input--error');
      }
    } catch (err) {
      modalPassError.textContent = 'Connection error. Please try again.';
      modalPass.classList.add('lm-modal-input--error');
    } finally {
      modalContinue.disabled = false;
      modalContinueText.style.display = 'inline';
      modalContinueSpin.style.display = 'none';
    }
  });

  // Enter key for modal
  modalPass.addEventListener('keydown', function (e) {
    if (e.key === 'Enter') modalContinue.click();
  });
})();
