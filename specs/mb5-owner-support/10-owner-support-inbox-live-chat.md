# VivuCar UI - Owner Support Inbox & Live Chat

## Mục tiêu trang
Người cho thuê xem danh sách hội thoại hỗ trợ, đọc lịch sử chatbot và tiếp quản live chat khi chatbot không xử lý được.

## File HTML gợi ý
`owner-support-inbox.html`

## Layout
- Owner header.
- Owner sidebar.
- Inbox layout 3 cột:
  - Cột trái: danh sách hội thoại.
  - Cột giữa: khung chat.
  - Cột phải: thông tin khách/booking.

## Header trang
- Title: `Hỗ trợ khách hàng`
- Subtitle: `Theo dõi hội thoại và tiếp quản khi khách cần hỗ trợ trực tiếp.`

## Conversation filter
Fields:
- Search:
  - id: `supportSearchInput`
  - placeholder: `Tìm theo tên khách, mã đơn, nội dung`
- Status:
  - id: `conversationStatusFilter`
  - options:
    - `Tất cả`
    - `AI_ONLY`
    - `WAITING_OWNER`
    - `OWNER_JOINED`
    - `RESOLVED`
- Priority:
  - id: `priorityFilter`
  - options:
    - `Tất cả`
    - `LOW`
    - `MEDIUM`
    - `HIGH`
- Button:
  - id: `btnFilterConversations`
  - text: `Lọc`

## Conversation list item
Mỗi item gồm:
- Tên khách.
- Mã đơn nếu có.
- Tin nhắn cuối.
- Thời gian tin nhắn cuối.
- Badge status:
  - AI_ONLY
  - WAITING_OWNER
  - OWNER_JOINED
  - RESOLVED
- Badge priority.
- Unread count.

## Chat header
Hiển thị:
- Tên khách.
- Status conversation.
- Booking liên quan.
- Buttons:
  - `Tiếp quản`
    - id: `btnTakeOverChat`
    - hiện nếu WAITING_OWNER hoặc AI_ONLY.
  - `Đánh dấu đã xử lý`
    - id: `btnResolveConversation`
  - `Xem lịch sử`
    - id: `btnViewChatHistory`

## Chat messages
Container:
- id: `ownerChatMessages`

Message types:
- Khách.
- AI Bot.
- Owner.
- System.

Mỗi message:
- Sender.
- Nội dung.
- Thời gian.
- Badge `AI` nếu do chatbot trả lời.

## Composer
Fields:
- Textarea:
  - id: `ownerChatInput`
  - placeholder: `Nhập tin nhắn trả lời khách...`
- Button:
  - id: `btnSendOwnerMessage`
  - text: `Gửi`
- Quick templates:
  - `Tôi đang kiểm tra thông tin đơn của bạn.`
  - `Bạn vui lòng gửi thêm hình ảnh minh chứng.`
  - `Tôi sẽ phản hồi trong ít phút.`
  - `Cảm ơn bạn đã liên hệ VivuCar.`

Composer disabled nếu:
- Conversation status là `RESOLVED`.
- Owner chưa bấm `Tiếp quản` trong mode AI_ONLY.

## Right panel: Customer context
Hiển thị:
- Tên khách.
- Số điện thoại.
- Email.
- Rating khách nếu có.
- Mã booking liên quan.
- Xe liên quan.
- Trạng thái đơn.
- Buttons:
  - `Xem đơn`
  - `Gọi khách`
  - `Mở chi tiết booking`

## JS state
```js
const supportInboxState = {
  selectedConversationId: null,
  status: "ALL",
  priority: "ALL",
  keyword: "",
  conversations: [],
  messages: []
};
```

## JS cần có
- Render conversation list.
- Chọn conversation để load messages.
- Take over chat.
- Send owner message.
- Quick templates insert vào input hoặc gửi ngay.
- Resolve conversation.
- Auto scroll messages.
- Unread count.
- Mock realtime bằng setInterval hoặc local state.

## API gợi ý
```txt
GET /api/owner/support/conversations?status=WAITING_OWNER
GET /api/owner/support/conversations/{conversationId}
POST /api/owner/support/conversations/{conversationId}/take-over
POST /api/owner/support/conversations/{conversationId}/messages
POST /api/owner/support/conversations/{conversationId}/resolve
```
