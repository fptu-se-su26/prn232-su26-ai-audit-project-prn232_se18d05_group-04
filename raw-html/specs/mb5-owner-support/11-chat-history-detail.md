# VivuCar UI - Chat History Detail

## Mục tiêu trang
Lưu trữ và xem chi tiết lịch sử hội thoại hỗ trợ giữa khách, chatbot AI và người cho thuê. Dùng cho tra cứu sau khi xử lý xong hoặc khi có tranh chấp.

## File HTML gợi ý
`chat-history-detail.html`

## URL gợi ý
```txt
chat-history-detail.html?conversationId=CV001
```

## Layout
- Owner header.
- Owner sidebar.
- Header conversation.
- Metadata cards.
- Message timeline.
- Export actions.

## Header
- Title: `Lịch sử hội thoại`
- Subtitle: `Tra cứu toàn bộ tin nhắn giữa khách, chatbot và người hỗ trợ.`

## Conversation metadata
Cards:
- Mã hội thoại.
- Khách hàng.
- Booking liên quan.
- Xe liên quan.
- Trạng thái hội thoại.
- Người tiếp quản.
- Thời gian bắt đầu.
- Thời gian xử lý xong.

## Summary section
Fields:
- Chủ đề chính:
  - id: `conversationTopic`
- Kết quả xử lý:
  - id: `resolutionSummary`
- Mức độ ưu tiên:
  - id: `conversationPriority`
- Có chuyển người hỗ trợ không:
  - Có/Không

## Message timeline
Container:
- id: `chatHistoryTimeline`

Message item:
- Sender:
  - Customer
  - AI Assistant
  - Owner
  - System
- Timestamp.
- Message content.
- Attachment nếu có.
- Badge:
  - `AI`
  - `Live chat`
  - `System`

## Filters trong timeline
- Checkbox:
  - id: `showCustomerMessages`
  - label: `Khách hàng`
- Checkbox:
  - id: `showAiMessages`
  - label: `AI`
- Checkbox:
  - id: `showOwnerMessages`
  - label: `Người hỗ trợ`
- Checkbox:
  - id: `showSystemMessages`
  - label: `Hệ thống`

## Search trong hội thoại
- Input:
  - id: `chatHistorySearch`
  - placeholder: `Tìm trong nội dung tin nhắn`

## Export actions
Buttons:
- `Xuất TXT`
  - id: `btnExportChatTxt`
- `Xuất PDF`
  - id: `btnExportChatPdf`
- `Quay lại hộp thư`
  - id: `btnBackSupportInbox`

## Empty state
Nếu không có message:
- Text: `Không có tin nhắn trong hội thoại này.`

## JS state
```js
const chatHistoryState = {
  conversationId: null,
  keyword: "",
  visibleSenders: ["CUSTOMER", "AI", "OWNER", "SYSTEM"],
  messages: []
};
```

## JS cần có
- Lấy conversationId từ URL.
- Load metadata và messages.
- Filter theo sender.
- Search nội dung tin nhắn.
- Export TXT bằng Blob.
- PDF có thể dùng `window.print()` hoặc gọi API backend.
- Back về:
  - `owner-support-inbox.html`

## API gợi ý
```txt
GET /api/owner/support/conversations/{conversationId}/history
GET /api/owner/support/conversations/{conversationId}/export/pdf
```

## CSS print
```css
@media print {
  .owner-sidebar,
  .owner-header,
  .chat-history-actions,
  .chat-history-filters {
    display: none;
  }

  body {
    background: white;
  }
}
```
