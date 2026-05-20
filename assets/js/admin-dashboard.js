(function () {
  const DB = window.VivuCarDB;
  const C = window.VivuCarConstants;
  const U = window.VivuCarUtils;
  let revenueChartInstance = null;

  function fetchDashboardData() {
    // GET /api/admin/reports/revenue?range=7days
    const snapshots = DB.daily_revenue_snapshots;
    const totalRevenue = snapshots.reduce((sum, item) => sum + Number(item.net_revenue), 0);
    const completedOrders = snapshots.reduce((sum, item) => sum + item.completed_bookings, 0);
    const cancelled = snapshots.reduce((sum, item) => sum + item.cancelled_bookings, 0);
    const totalBookings = snapshots.reduce((sum, item) => sum + item.total_bookings, 0);
    return {
      totalRevenue,
      completedOrders,
      averageOrderValue: completedOrders ? totalRevenue / completedOrders : 0,
      cancelRate: totalBookings ? cancelled / totalBookings * 100 : 0,
      chartData: snapshots,
      recentOrders: DB.bookings.slice().reverse().slice(0, 5)
    };
  }

  function renderDashboard() {
    const data = fetchDashboardData();
    document.getElementById("totalRevenue").textContent = U.formatVnd(data.totalRevenue);
    document.getElementById("completedOrders").textContent = data.completedOrders;
    document.getElementById("averageOrderValue").textContent = U.formatVnd(data.averageOrderValue);
    document.getElementById("cancelRate").textContent = `${data.cancelRate.toFixed(1)}%`;
    renderRevenueChart(data.chartData);
    renderRecentOrders(data.recentOrders);
  }

  function renderRevenueChart(items) {
    const chart = document.getElementById("revenueChart");
    const canvas = document.getElementById("revenueChartCanvas");
    const max = Math.max(...items.map((item) => Number(item.net_revenue)), 1);
    const emerald = "#059669";
    document.getElementById("dashboardEmpty").classList.toggle("hidden", items.length > 0);
    if (window.Chart && canvas) {
      chart.classList.add("hidden");
      canvas.classList.remove("hidden");
      if (revenueChartInstance) revenueChartInstance.destroy();
      revenueChartInstance = new Chart(canvas, {
        type: "bar",
        data: {
          labels: items.map((item) => item.snapshot_date.slice(5)),
          datasets: [
            {
              label: "Doanh thu thực nhận",
              data: items.map((item) => Number(item.net_revenue)),
              backgroundColor: emerald,
              hoverBackgroundColor: "#047857",
              borderRadius: 8,
              barPercentage: 0.82,
              categoryPercentage: 0.78,
              maxBarThickness: 62
            }
          ]
        },
        options: {
          responsive: true,
          maintainAspectRatio: false,
          plugins: {
            legend: { labels: { color: "#52525b", boxWidth: 10, boxHeight: 10 } },
            tooltip: {
              callbacks: {
                label(context) {
                  return `${context.dataset.label}: ${U.formatVnd(context.raw)}`;
                }
              }
            }
          },
          scales: {
            x: { grid: { display: false }, ticks: { color: "#71717a" } },
            y: { grid: { color: "#e4e4e7" }, ticks: { color: "#71717a", callback: (value) => `${Number(value) / 1000000}tr` } }
          }
        }
      });
      return;
    }
    if (canvas) canvas.classList.add("hidden");
    chart.classList.remove("hidden");
    chart.innerHTML = items.map((item) => `<div class="bar-item" data-tooltip="${U.formatDate(item.snapshot_date)} · ${U.formatVnd(item.net_revenue)} · ${item.completed_bookings} đơn"><div class="bar" style="height:${Math.max(8, Number(item.net_revenue) / max * 100)}%;background:${emerald}"></div><span>${item.snapshot_date.slice(5)}</span></div>`).join("");
  }

  function renderRecentOrders(orders) {
    document.getElementById("recentOrdersBody").innerHTML = orders.map((booking) => {
      const user = DB.users.find((item) => item.id === booking.user_id);
      const car = DB.cars.find((item) => item.id === booking.car_id);
      const payment = DB.payments.find((item) => item.booking_id === booking.id);
      return `<tr><td class="mono">${booking.id}</td><td>${user?.full_name || ""}</td><td>${car?.brand || ""} ${car?.model || ""}</td><td>${U.formatDateTime(booking.pickup_datetime)}</td><td>${U.formatVnd(booking.total_amount)}</td><td>${U.statusBadge(payment?.status === "success" ? "success" : payment?.status === "refunded" ? "neutral" : "warning", payment?.status, C.PAYMENT_STATUS_LABELS[payment?.status])}</td></tr>`;
    }).join("");
  }

  document.getElementById("revenueRange").addEventListener("change", (event) => {
    document.querySelectorAll(".custom-date").forEach((input) => input.classList.toggle("hidden", event.target.value !== "custom"));
  });
  document.getElementById("btnApplyRevenueFilter").addEventListener("click", () => {
    U.showToast("Đã áp dụng bộ lọc doanh thu.");
    renderDashboard();
  });
  renderDashboard();
})();
