(function () {
  const DB = window.VivuCarDB;
  const C = window.VivuCarConstants;
  const U = window.VivuCarUtils;
  const Auth = window.VivuCarAuth;

  function activeType() { return document.querySelector("input[name='reportType']:checked").value; }
  function cell(value) { return `<td>${value ?? ""}</td>`; }

  function previewRows(type) {
    if (type === "payments") {
      return { headers: ["ID", "Booking", "Phương thức", "Số tiền", "Trạng thái", "Mã GD", "Thanh toán lúc"], rows: DB.payments.map((p) => [p.id, p.booking_id, C.PAYMENT_METHOD_LABELS[p.method], U.formatVnd(p.amount), C.PAYMENT_STATUS_LABELS[p.status], p.transaction_code, p.paid_at ? U.formatDateTime(p.paid_at) : ""]) };
    }
    if (type === "users") {
      return { headers: ["ID", "Họ tên", "Email", "Vai trò", "Khóa", "Ngày tạo"], rows: DB.users.map((u) => [u.id, u.full_name, u.email, C.USER_ROLE_LABELS[u.role], u.is_blocked ? "Đã khóa" : "Hoạt động", U.formatDate(u.created_at)]) };
    }
    if (type === "cars") {
      return { headers: ["Biển số", "Hãng", "Dòng", "Chủ xe", "Giá/ngày", "Trạng thái"], rows: DB.cars.map((c) => [c.license_plate, c.brand, c.model, DB.users.find((u) => u.id === c.owner_id)?.full_name, U.formatVnd(c.price_per_day), C.CAR_STATUS_LABELS[c.status]]) };
    }
    return { headers: ["Ngày", "Tổng đơn", "Hoàn tất", "Đã hủy", "Gross", "Net"], rows: DB.daily_revenue_snapshots.map((r) => [r.snapshot_date, r.total_bookings, r.completed_bookings, r.cancelled_bookings, U.formatVnd(r.gross_revenue), U.formatVnd(r.net_revenue)]) };
  }

  function renderPreview() {
    const data = previewRows(activeType());
    document.getElementById("reportPreview").innerHTML = `<table><thead><tr>${data.headers.map((h) => `<th>${h}</th>`).join("")}</tr></thead><tbody>${data.rows.map((row) => `<tr>${row.map(cell).join("")}</tr>`).join("")}</tbody></table>`;
  }

  function renderJobs() {
    document.getElementById("exportJobsBody").innerHTML = DB.export_jobs.map((job) => `<tr><td class="mono">${job.id}</td><td>${job.export_type}</td><td>${U.statusBadge(job.status === "done" ? "success" : job.status === "failed" ? "danger" : "warning", job.status, C.EXPORT_STATUS_LABELS[job.status])}</td><td>${job.file_url || ""}</td><td>${U.formatDateTime(job.created_at)}</td></tr>`).join("");
  }

  function createJob(format) {
    const job = { id: DB.export_jobs.length + 1, requested_by: Auth.getCurrentUser().id, export_type: `${activeType()}_${format}`, params: { from: document.getElementById("exportDateFrom").value, to: document.getElementById("exportDateTo").value }, status: "processing", file_url: null, error_message: null, created_at: new Date().toISOString(), completed_at: null };
    DB.export_jobs.unshift(job);
    renderJobs();
    setTimeout(() => {
      job.status = "done";
      job.file_url = `mock://${job.export_type}-${job.id}.${format === "excel" ? "xls" : "pdf"}`;
      job.completed_at = new Date().toISOString();
      U.downloadBlob(new Blob([`${job.export_type} export demo`], { type: "text/plain" }), `vivucar-${job.export_type}.${format === "excel" ? "xls" : "pdf"}`);
      U.showToast("Xuất file thành công.");
      renderJobs();
    }, 550);
  }

  document.getElementById("btnPreviewReport").addEventListener("click", renderPreview);
  document.getElementById("reportTypes").addEventListener("change", renderPreview);
  document.getElementById("btnExportExcel").addEventListener("click", () => createJob("excel"));
  document.getElementById("btnExportPdf").addEventListener("click", () => U.openModal("pdfExportModal"));
  document.getElementById("btnCreatePdf").addEventListener("click", () => { U.closeModal("pdfExportModal"); createJob("pdf"); });
  renderPreview();
  renderJobs();
})();

