const fs = require('fs');
const file = 'VivuCarClient/WebClient/wwwroot/js/html/layout.js';
let content = fs.readFileSync(file, 'utf8');

const map = {
  'NgÆ°á» i dÃ¹ng': 'Người dùng',
  'PhÆ°Æ¡ng tiá»‡n': 'Phương tiện',
  'Xuáº¥t bÃ¡o cÃ¡o': 'Xuất báo cáo',
  'ThÃ¹ng rÃ¡c': 'Thùng rác',
  'Kiá»ƒm duyá»‡t ná»™i dung': 'Kiểm duyệt nội dung',
  'Kiá»ƒm duyá»‡t GPLX': 'Kiểm duyệt GPLX',
  'Dashboard doanh thu': 'Dashboard doanh thu',
  'ThuÃª xe': 'Thuê xe',
  'Ä\u008EÆ¡n thuÃª cá»§a tÃ´i': 'Đơn thuê của tôi',
  'Xe cá»§a tÃ´i': 'Xe của tôi',
  'Ä\u008EÆ¡n Ä\u0091áº·t xe': 'Đơn đặt xe',
  'BÃ\u00A0n giao & tráº£ xe': 'Bàn giao & trả xe',
  'Há»— trá»£': 'Hỗ trợ',
  'Há»“ sÆ¡': 'Hồ sơ',
  'Ä\u008EÃ¡nh giÃ¡ cá»§a tÃ´i': 'Đánh giá của tôi',
  'Quáº£n lÃ½ xe': 'Quản lý xe',
  'YÃªu cáº§u Ä\u0091áº·t xe': 'Yêu cầu đặt xe',
  'BÃ\u00A0n giao & Tráº£ xe': 'Bàn giao & Trả xe',
  'Há»— trá»£ khÃ¡ch hÃ\u00A0ng': 'Hỗ trợ khách hàng',
  'Ä\u008EÄƒng xuáº¥t': 'Đăng xuất',
  'Ä\u008EÄƒng nháº\u00ADp': 'Đăng nhập',
  'Trang chá»§': 'Trang chủ',
  'TÃ¬m xe': 'Tìm xe',
  'Xe ná»•i báº­t': 'Xe nổi bật',
  'ThÃ´ng tin cÃ¡ nhÃ¢n': 'Thông tin cá nhân',
  'Chá»‰nh sá»\u00ADa há»“ sÆ¡': 'Chỉnh sửa hồ sơ',
  'Giáº¥y phÃ©p lÃ¡i xe': 'Giấy phép lái xe',
  'XÃ¡c nháº­n Ä\u0091Äƒng xuáº¥t': 'Xác nhận đăng xuất',
  'Báº¡n cÃ³ cháº¯c cháº¯n muá»‘n Ä\u0091Äƒng xuáº¥t khá»\u008Di VivuCar?': 'Bạn có chắc chắn muốn đăng xuất khỏi VivuCar?',
  'Há»§y': 'Hủy',
  'Ä\u008Dang Ä\u0091Äƒng xuáº¥t...': 'Đang đăng xuất...',
  'Thu gá»\u008Dn sidebar': 'Thu gọn sidebar',
  'Má»Ÿ rá»™ng sidebar': 'Mở rộng sidebar',
  'Má»Ÿ menu': 'Mở menu',
  'Ä\u008EÆ¡n thuÃª': 'Đơn thuê',
  'Ä\u008EÆ¡n Ä\u0091áº·t': 'Đơn đặt'
};

for (const [bad, good] of Object.entries(map)) {
  content = content.split(bad).join(good);
}

// Second pass for literals exactly as they appear
const rawMap = {
  'ThuÃª xe': 'Thuê xe',
  'Ä Æ¡n thuÃª cá»§a tÃ´i': 'Đơn thuê của tôi',
  'Ä Æ¡n thuÃª': 'Đơn thuê',
  'Ä Æ¡n Ä‘áº·t xe': 'Đơn đặt xe',
  'BÃ n giao & tráº£ xe': 'Bàn giao & trả xe',
  'BÃ n giao & Tráº£ xe': 'Bàn giao & Trả xe',
  'Há»— trá»£ khÃ¡ch hÃ ng': 'Hỗ trợ khách hàng',
  'Ä Äƒng xuáº¥t': 'Đăng xuất',
  'Ä Äƒng nháº­p': 'Đăng nhập',
  'Trang chá»§': 'Trang chủ',
  'TÃ¬m xe': 'Tìm xe',
  'Xe ná»•i báº­t': 'Xe nổi bật',
  'ThÃ´ng tin cÃ¡ nhÃ¢n': 'Thông tin cá nhân',
  'Chá»‰nh sá»­a há»“ sÆ¡': 'Chỉnh sửa hồ sơ',
  'Giáº¥y phÃ©p lÃ¡i xe': 'Giấy phép lái xe',
  'XÃ¡c nháº­n Ä‘Äƒng xuáº¥t': 'Xác nhận đăng xuất',
  'Báº¡n cÃ³ cháº¯c cháº¯n muá»‘n Ä‘Äƒng xuáº¥t khá» i VivuCar?': 'Bạn có chắc chắn muốn đăng xuất khỏi VivuCar?',
  'Há»§y': 'Hủy',
  'Ä ang Ä‘Äƒng xuáº¥t...': 'Đang đăng xuất...',
  'Thu gá» n sidebar': 'Thu gọn sidebar',
  'Má»Ÿ rá»™ng sidebar': 'Mở rộng sidebar',
  'Má»Ÿ menu': 'Mở menu'
};

for (const [bad, good] of Object.entries(rawMap)) {
  content = content.split(bad).join(good);
}

fs.writeFileSync(file, content, 'utf8');
