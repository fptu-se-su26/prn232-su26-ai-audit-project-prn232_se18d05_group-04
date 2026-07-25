// Register page - LearnMate-style 4-step wizard
(function () {
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

  // ── State ──
  let currentStep = 1;
  let isGoingBack = false;
  let isLoading = false;
  let isSubmitting = false;

  // Step 1
  let email = '';

  // Step 2
  let fullName = '';
  let dobDay = '';
  let dobMonth = '';
  let dobYear = '';

  // Step 3
  let password = '';
  let showPassword = false;

  // Step 4
  let otpDigits = ['', '', '', '', '', ''];

  // ── DOM refs ──
  const stepContainer = document.getElementById('stepContainer');
  const globalError = document.getElementById('globalError');
  const googleSection = document.getElementById('googleSection');
  const stepperEl = document.getElementById('stepperEl');

  // ── Helpers ──
  function showError(msg) {
    if (!globalError) return;
    globalError.textContent = msg;
    globalError.style.display = 'block';
  }

  function hideError() {
    if (!globalError) return;
    globalError.textContent = '';
    globalError.style.display = 'none';
  }

  function setLoading(loading, btnId, textId, spinId) {
    const btn = document.getElementById(btnId);
    const txt = document.getElementById(textId);
    const spin = document.getElementById(spinId);
    if (btn) btn.disabled = loading;
    if (txt) txt.style.display = loading ? 'none' : '';
    if (spin) spin.style.display = loading ? 'inline-block' : 'none';
  }

  // ── Stepper ──
  function renderStepper() {
    if (!stepperEl) return;
    const labels = ['Enter your email', 'Provide basic info', 'Create your password'];
    const total = labels.length;
    const pct = total <= 1 ? 0 : Math.min(100, (currentStep - 1) / (total - 1) * 100);

    let dotsHtml = '';
    for (let i = 1; i <= total; i++) {
      const state = i < currentStep ? 'done' : i === currentStep ? 'active' : 'todo';
      let inner = '';
      if (state === 'done') {
        inner = '<svg width="8" height="8" viewBox="0 0 12 10" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><polyline points="1 5 4 8 11 1" /></svg>';
      }
      dotsHtml += `<div class="lm-step-dot lm-step-dot--${state}">${inner}</div>`;
    }

    let labelsHtml = '';
    for (let i = 0; i < labels.length; i++) {
      const stepNum = i + 1;
      const state = stepNum < currentStep ? 'done' : stepNum === currentStep ? 'active' : 'todo';
      labelsHtml += `<span class="lm-step-label lm-step-label--${state}">${labels[i]}</span>`;
    }

    stepperEl.innerHTML = `
      <div class="lm-stepper-track">
        <div class="lm-stepper-track-fill" style="width: ${pct}%"></div>
      </div>
      <div class="lm-stepper-dots">${dotsHtml}</div>
      <div class="lm-step-labels">${labelsHtml}</div>
    `;
  }

  // ── Render current step ──
  function renderStep() {
    if (!stepContainer) return;
    const cls = isGoingBack ? 'lm-step-content lm-step-content--back' : 'lm-step-content';
    let html = '';

    if (currentStep === 1) {
      html = `
        <div class="lm-field">
          <label class="lm-label" for="regEmail">What's your email?</label>
          <div class="lm-input-wrapper">
            <input id="regEmail" type="email" class="lm-input" placeholder="Enter your email address"
              value="${escapeHtml(email)}" autocomplete="email" />
          </div>
        </div>
        <button class="lm-btn-next" id="step1Next" disabled>Next</button>
      `;
    } else if (currentStep === 2) {
      const days = getDays();
      const months = [
        { value: 1, label: 'January' }, { value: 2, label: 'February' }, { value: 3, label: 'March' },
        { value: 4, label: 'April' }, { value: 5, label: 'May' }, { value: 6, label: 'June' },
        { value: 7, label: 'July' }, { value: 8, label: 'August' }, { value: 9, label: 'September' },
        { value: 10, label: 'October' }, { value: 11, label: 'November' }, { value: 12, label: 'December' }
      ];
      const years = getYears();

      html = `
        <div class="lm-field">
          <label class="lm-label" for="regName">Your name</label>
          <div class="lm-input-wrapper">
            <input id="regName" type="text" class="lm-input" placeholder="Enter your full name"
              value="${escapeHtml(fullName)}" autocomplete="name" />
          </div>
        </div>
        <div class="lm-field">
          <label class="lm-label">Date of birth</label>
          <div class="lm-dob-wrapper">
            <svg class="lm-dob-icon" xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round">
              <rect width="18" height="18" x="3" y="4" rx="2" ry="2" /><line x1="16" x2="16" y1="2" y2="6" /><line x1="8" x2="8" y1="2" y2="6" /><line x1="3" x2="21" y1="10" y2="10" />
            </svg>
            <div class="lm-dob-selects">
              <select class="lm-select ${!dobDay ? 'lm-select--placeholder' : ''}" id="dobDay">
                <option value="">Day</option>
                ${days.map(d => `<option value="${d}" ${dobDay == d ? 'selected' : ''}>${d}</option>`).join('')}
              </select>
              <select class="lm-select ${!dobMonth ? 'lm-select--placeholder' : ''}" id="dobMonth">
                <option value="">Month</option>
                ${months.map(m => `<option value="${m.value}" ${dobMonth == m.value ? 'selected' : ''}>${m.label}</option>`).join('')}
              </select>
              <select class="lm-select ${!dobYear ? 'lm-select--placeholder' : ''}" id="dobYear">
                <option value="">Year</option>
                ${years.map(y => `<option value="${y}" ${dobYear == y ? 'selected' : ''}>${y}</option>`).join('')}
              </select>
            </div>
          </div>
          <p class="lm-hint">You must be between 13 and 100 years old.</p>
        </div>
        <div class="lm-step-nav lm-step-nav--between">
          <button class="lm-btn-step-back" id="step2Back">Back</button>
          <button class="lm-btn-step-primary" id="step2Next" disabled>Next</button>
        </div>
      `;
    } else if (currentStep === 3) {
      const passType = showPassword ? 'text' : 'password';
      const eyeHtml = showPassword
        ? '<svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M9.88 9.88a3 3 0 1 0 4.24 4.24"/><path d="M10.73 5.08A10.43 10.43 0 0 1 12 5c7 0 10 7 10 7a13.16 13.16 0 0 1-1.67 2.68"/><path d="M6.61 6.61A13.526 13.526 0 0 0 2 12s3 7 10 7a9.74 9.74 0 0 0 5.39-1.61"/><line x1="2" x2="22" y1="2" y2="22"/></svg>'
        : '<svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M2 12s3-7 10-7 10 7 10 7-3 7-10 7-10-7-10-7Z"/><circle cx="12" cy="12" r="3"/></svg>';

      const strengthHtml = password ? renderStrength(password) : '';

      html = `
        <div class="lm-field">
          <label class="lm-label" for="regPassword">Create a password</label>
          <div class="lm-input-wrapper">
            <input id="regPassword" type="${passType}" class="lm-input lm-input--icon-right"
              placeholder="••••••••" autocomplete="new-password" value="${escapeHtml(password)}" />
            <button type="button" class="lm-eye-btn" id="regEyeBtn" tabindex="-1" aria-label="Toggle password visibility">
              ${eyeHtml}
            </button>
          </div>
          <div id="strengthContainer">${strengthHtml}</div>
        </div>
        <div class="lm-step-nav lm-step-nav--between">
          <button class="lm-btn-step-back" id="step3Back">Back</button>
          <button class="lm-btn-step-primary" id="step3Next" disabled>
            <span id="step3Text">Create account</span>
            <span class="lm-spinner" id="step3Spin" style="display:none" aria-label="Creating account..."></span>
          </button>
        </div>
      `;
    } else if (currentStep === 4) {
      let otpBoxes = '';
      for (let i = 0; i < 6; i++) {
        const filled = otpDigits[i] ? 'lm-otp-box--filled' : '';
        const autocomplete = i === 0 ? 'one-time-code' : 'off';
        otpBoxes += `<input type="text" class="lm-otp-box ${filled}" maxlength="1" inputmode="numeric"
          value="${escapeHtml(otpDigits[i])}" autocomplete="${autocomplete}" data-idx="${i}" />`;
      }

      html = `
        <div class="lm-field">
          <label class="lm-label">Enter the verification code</label>
          <div class="lm-otp-boxes">${otpBoxes}</div>
          <p class="lm-otp-hint">A 6-digit code has been sent to <b>${escapeHtml(email)}</b>. Please check your inbox.</p>
        </div>
        <div class="lm-step-nav lm-step-nav--between">
          <button class="lm-btn-step-back" id="step4Back">Back</button>
          <button class="lm-btn-step-primary" id="step4Confirm" disabled>
            <span id="step4Text">Confirm</span>
            <span class="lm-spinner" id="step4Spin" style="display:none" aria-label="Confirming..."></span>
          </button>
        </div>
      `;
    }

    stepContainer.innerHTML = `<div class="${cls}" data-step="${currentStep}">${html}</div>`;
    renderStepper();

    // Google section visibility
    if (googleSection) {
      googleSection.style.display = currentStep <= 2 ? '' : 'none';
    }

    // Bind step events
    bindStepEvents();
  }

  // ── Bind events per step ──
  function bindStepEvents() {
    if (currentStep === 1) {
      const input = document.getElementById('regEmail');
      const next = document.getElementById('step1Next');
      if (input) {
        input.value = email;
        input.addEventListener('input', function () {
          email = input.value.trim();
          next.disabled = !emailRegex.test(email);
          hideError();
        });
        next.disabled = !emailRegex.test(email);
        next.addEventListener('click', function () {
          hideError();
          goNext();
        });
        input.focus();
      }
    } else if (currentStep === 2) {
      const nameInput = document.getElementById('regName');
      const daySel = document.getElementById('dobDay');
      const monthSel = document.getElementById('dobMonth');
      const yearSel = document.getElementById('dobYear');
      const backBtn = document.getElementById('step2Back');
      const nextBtn = document.getElementById('step2Next');

      function updateDob() {
        dobDay = daySel.value;
        dobMonth = monthSel.value;
        dobYear = yearSel.value;
        daySel.classList.toggle('lm-select--placeholder', !dobDay);
        monthSel.classList.toggle('lm-select--placeholder', !dobMonth);
        yearSel.classList.toggle('lm-select--placeholder', !dobYear);
        updateStep2Valid();
      }

      function updateStep2Valid() {
        const name = (nameInput.value || '').trim();
        fullName = name;
        const d = parseInt(dobDay), m = parseInt(dobMonth), y = parseInt(dobYear);
        let valid = name.length > 0 && d && m && y;
        if (valid) {
          const today = new Date();
          const birth = new Date(y, m - 1, d);
          let age = today.getFullYear() - birth.getFullYear();
          const mDiff = today.getMonth() - birth.getMonth();
          if (mDiff < 0 || (mDiff === 0 && today.getDate() < birth.getDate())) age--;
          valid = age >= 13 && age <= 100;
        }
        nextBtn.disabled = !valid;
      }

      if (nameInput) {
        nameInput.value = fullName;
        nameInput.addEventListener('input', updateStep2Valid);
      }
      if (daySel) {
        monthSel.addEventListener('change', function () {
          // Clamp days
          const m = parseInt(monthSel.value);
          const y = parseInt(yearSel.value);
          if (m && y) {
            const maxDays = new Date(y, m, 0).getDate();
            const currentDays = Array.from(daySel.options).map(o => parseInt(o.value)).filter(v => !isNaN(v));
            const newMax = Math.max(...currentDays);
            if (maxDays !== newMax) {
              const selected = parseInt(daySel.value);
              const currentHtml = daySel.innerHTML;
              let days = '';
              for (let d = 1; d <= maxDays; d++) {
                days += `<option value="${d}" ${selected === d ? 'selected' : ''}>${d}</option>`;
              }
              daySel.innerHTML = days;
              if (selected > maxDays) daySel.value = maxDays;
            }
          }
          updateDob();
        });
        yearSel.addEventListener('change', function () {
          const m = parseInt(monthSel.value);
          const y = parseInt(yearSel.value);
          if (m && y) {
            const maxDays = new Date(y, m, 0).getDate();
            const selected = parseInt(daySel.value);
            let days = '';
            for (let d = 1; d <= maxDays; d++) {
              days += `<option value="${d}" ${selected === d ? 'selected' : ''}>${d}</option>`;
            }
            daySel.innerHTML = days;
            if (selected > maxDays) daySel.value = maxDays;
          }
          updateDob();
        });
        daySel.addEventListener('change', updateDob);
      }
      updateDob();

      if (backBtn) backBtn.addEventListener('click', goBack);
      if (nextBtn) nextBtn.addEventListener('click', goNext);
    } else if (currentStep === 3) {
      const passInput = document.getElementById('regPassword');
      const eyeBtn = document.getElementById('regEyeBtn');
      const backBtn = document.getElementById('step3Back');
      const nextBtn = document.getElementById('step3Next');

      if (passInput) {
        passInput.value = password;
        passInput.addEventListener('input', function () {
          password = passInput.value;
          updateStrength(password);
          hideError();
        });
        passInput.focus();
      }

      if (eyeBtn) {
        eyeBtn.addEventListener('click', function () {
          showPassword = !showPassword;
          passInput.type = showPassword ? 'text' : 'password';
          const isOpen = showPassword;
          eyeBtn.innerHTML = isOpen
            ? '<svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M9.88 9.88a3 3 0 1 0 4.24 4.24"/><path d="M10.73 5.08A10.43 10.43 0 0 1 12 5c7 0 10 7 10 7a13.16 13.16 0 0 1-1.67 2.68"/><path d="M6.61 6.61A13.526 13.526 0 0 0 2 12s3 7 10 7a9.74 9.74 0 0 0 5.39-1.61"/><line x1="2" x2="22" y1="2" y2="22"/></svg>'
            : '<svg xmlns="http://www.w3.org/2000/svg" width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><path d="M2 12s3-7 10-7 10 7 10 7-3 7-10 7-10-7-10-7Z"/><circle cx="12" cy="12" r="3"/></svg>';
        });
      }

      if (backBtn) backBtn.addEventListener('click', goBack);
      if (nextBtn) {
        updateNextBtnState();
        nextBtn.addEventListener('click', submitRegistration);
      }
    } else if (currentStep === 4) {
      const boxes = document.querySelectorAll('.lm-otp-box');
      const backBtn = document.getElementById('step4Back');
      const confirmBtn = document.getElementById('step4Confirm');

      boxes.forEach(function (box) {
        box.addEventListener('input', function () {
          const idx = parseInt(box.dataset.idx);
          const raw = box.value;
          const digit = raw.replace(/\D/g, '').slice(-1);
          box.value = digit;
          otpDigits[idx] = digit;
          box.classList.toggle('lm-otp-box--filled', !!digit);

          if (digit && idx < 5) {
            boxes[idx + 1].focus();
          }
          updateOtpState();
        });

        box.addEventListener('keydown', function (e) {
          const idx = parseInt(box.dataset.idx);
          if (e.key === 'Backspace' && !otpDigits[idx] && idx > 0) {
            otpDigits[idx - 1] = '';
            boxes[idx - 1].value = '';
            boxes[idx - 1].classList.remove('lm-otp-box--filled');
            boxes[idx - 1].focus();
            updateOtpState();
          }
        });

        box.addEventListener('focus', function () {
          box.select();
        });
      });

      if (boxes[0]) boxes[0].focus();

      if (backBtn) backBtn.addEventListener('click', goBack);
      if (confirmBtn) confirmBtn.addEventListener('click', confirmOtp);
      updateOtpState();
    }
  }

  // ── Password strength ──
  function checkStrength(pwd) {
    const checks = {
      len: pwd.length >= 8,
      upper: /[A-Z]/.test(pwd),
      lower: /[a-z]/.test(pwd),
      digit: /\d/.test(pwd),
      special: /[^a-zA-Z0-9]/.test(pwd)
    };
    const score = (checks.len ? 1 : 0) + (checks.upper ? 1 : 0) + (checks.lower ? 1 : 0) + (checks.digit ? 1 : 0) + (checks.special ? 1 : 0);
    return { checks, score };
  }

  function getStrengthLabel(score) {
    if (score <= 1) return { label: 'Too weak', cls: 'tooweak' };
    if (score === 2) return { label: 'Weak', cls: 'weak' };
    if (score === 3) return { label: 'Fair', cls: 'fair' };
    if (score === 4) return { label: 'Strong', cls: 'strong' };
    return { label: 'Very strong', cls: 'verystrong' };
  }

  function renderStrength(pwd) {
    const { checks, score } = checkStrength(pwd);
    const info = getStrengthLabel(score);

    let bars = '';
    for (let i = 1; i <= 5; i++) {
      const active = i <= score ? `lm-strength-seg--${info.cls}` : '';
      bars += `<div class="lm-strength-seg ${active}"></div>`;
    }

    const reqs = [
      { text: '8+ characters', pass: checks.len },
      { text: 'Contains number', pass: checks.digit },
      { text: 'Uppercase letter', pass: checks.upper },
      { text: 'Special character', pass: checks.special },
      { text: 'Lowercase letter', pass: checks.lower }
    ];

    let reqHtml = '';
    reqs.forEach(function (r) {
      const cls = r.pass ? 'lm-req--pass' : '';
      const icon = r.pass
        ? '<svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2.5" stroke-linecap="round" stroke-linejoin="round"><path d="M22 11.08V12a10 10 0 1 1-5.93-9.14"/><polyline points="22 4 12 14.01 9 11.01"/></svg>'
        : '<svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" stroke-width="2" stroke-linecap="round" stroke-linejoin="round"><circle cx="12" cy="12" r="10"/></svg>';
      reqHtml += `<div class="lm-req ${cls}">${icon}<span>${r.text}</span></div>`;
    });

    return `
      <div class="lm-strength">
        <div class="lm-strength-header">
          <span class="lm-strength-label lm-strength--${info.cls}">${info.label}</span>
        </div>
        <div class="lm-strength-bars">${bars}</div>
        <div class="lm-requirements">${reqHtml}</div>
      </div>
    `;
  }

  function updateStrength(pwd) {
    const container = document.getElementById('strengthContainer');
    if (!container) return;
    if (pwd) {
      container.innerHTML = renderStrength(pwd);
    } else {
      container.innerHTML = '';
    }
    updateNextBtnState();
  }

  function passwordAllPass() {
    if (!password) return false;
    const { checks } = checkStrength(password);
    return checks.len && checks.upper && checks.lower && checks.digit && checks.special;
  }

  function updateNextBtnState() {
    const nextBtn = document.getElementById('step3Next');
    if (!nextBtn) return;
    nextBtn.disabled = !passwordAllPass() || isLoading;
  }

  function updateOtpState() {
    const confirmBtn = document.getElementById('step4Confirm');
    if (!confirmBtn) return;
    const code = otpDigits.join('');
    confirmBtn.disabled = code.length < 6 || isLoading;
  }

  // ── Navigation ──
  function goNext() {
    isGoingBack = false;
    hideError();
    currentStep++;
    renderStep();
  }

  function goBack() {
    isGoingBack = true;
    hideError();
    currentStep--;
    renderStep();
  }

  // ── Submit registration (step 3 -> 4) ──
  async function submitRegistration() {
    isLoading = true;
    isSubmitting = true;
    setLoading(true, 'step3Next', 'step3Text', 'step3Spin');
    hideError();

    try {
      const dob = dobDay && dobMonth && dobYear
        ? `${dobYear}-${String(dobMonth).padStart(2, '0')}-${String(dobDay).padStart(2, '0')}`
        : null;

      const resp = await fetch('/api/auth/register', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          email: email,
          password: password,
          fullName: fullName,
          dateOfBirth: dob
        })
      });
      const result = await resp.json();

      if (!resp.ok) {
        showError(result.message || 'Registration failed. Please try again.');
        return;
      }
    } catch (err) {
      showError('Connection error. Please try again.');
      return;
    } finally {
      isLoading = false;
      isSubmitting = false;
      setLoading(false, 'step3Next', 'step3Text', 'step3Spin');
    }

    goNext();
  }

  // ── Confirm OTP (step 4) ──
  async function confirmOtp() {
    isLoading = true;
    setLoading(true, 'step4Confirm', 'step4Text', 'step4Spin');
    hideError();

    try {
      const code = otpDigits.join('');
      const resp = await fetch('/api/auth/verify-otp', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email, code })
      });
      const result = await resp.json();

      if (!resp.ok) {
        showError(result.message || 'Invalid verification code. Please try again.');
        return;
      }

      window.location.href = '/Login';
    } catch (err) {
      showError('Connection error. Please try again.');
    } finally {
      isLoading = false;
      setLoading(false, 'step4Confirm', 'step4Text', 'step4Spin');
    }
  }

  // ── Utilities ──
  function escapeHtml(str) {
    if (!str) return '';
    return str.replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;');
  }

  function getDays() {
    let maxDays = 31;
    if (dobMonth && dobYear) {
      maxDays = new Date(parseInt(dobYear), parseInt(dobMonth), 0).getDate();
    }
    const days = [];
    for (let d = 1; d <= maxDays; d++) days.push(d);
    return days;
  }

  function getYears() {
    const today = new Date();
    const years = [];
    for (let y = today.getFullYear() - 13; y >= today.getFullYear() - 100; y--) {
      years.push(y);
    }
    return years;
  }

  // ── Init ──
  renderStep();
})();
