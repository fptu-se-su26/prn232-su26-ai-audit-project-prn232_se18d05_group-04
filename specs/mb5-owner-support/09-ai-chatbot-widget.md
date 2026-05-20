# VivuCar UI - AI Chatbot Widget

## Mục tiêu component
Chatbot AI hỗ trợ khách thuê trả lời nhanh các câu hỏi trong suốt vòng đời chuyến đi. Component hiển thị dạng widget nổi ở góc phải màn hình.

## File HTML gợi ý
`ai-chatbot-widget.html`

## Vị trí sử dụng
- User home.
- Car detail.
- Booking checkout.
- My bookings.
- Booking detail.
- Payment result.

## Floating button
- id: `chatbotFloatingButton`
- Vị trí: góc phải dưới.
- Text/Icon:
  - `Hỗ trợ`
- Badge nếu có tin nhắn mới.

## Chat window
- id: `chatbotWindow`
- Ẩn mặc định.
- Khi mở:
  - panel rộng 360px desktop.
  - full width trên mobile.

## Header
- Title: `VivuCar Assistant`
- Status:
  - `Online`
- Buttons:
  - Minimize
    - id: `btnMinimizeChatbot`
  - Close
    - id: `btnCloseChatbot`

## Message area
Container:
- id: `chatbotMessages`

Message types:
- Bot message.
- User message.
- System message.
- Quick reply.

## Welcome message
Bot gửi:
- `Xin chào, tôi có thể hỗ trợ bạn tìm xe, đặt xe, thanh toán cọc hoặc xử lý vấn đề sau chuyến đi.`

## Quick replies
Buttons:
- `Tôi muốn tìm xe`
- `Cách thanh toán tiền cọc`
- `Tôi muốn hủy đơn`
- `Tôi cần trả xe`
- `Gặp người hỗ trợ`

## Input area
Fields:
- Text input:
  - id: `chatbotInput`
  - placeholder: `Nhập câu hỏi của bạn...`
- Button:
  - id: `btnSendChatbotMessage`
  - text: `Gửi`
- Button:
  - id: `btnEscalateToHuman`
  - text: `Gặp người hỗ trợ`

## Typing indicator
- id: `botTypingIndicator`
- Text: `VivuCar Assistant đang nhập...`

## AI answer mock rules
Frontend mock có thể trả lời theo keyword:
- `đặt xe`, `booking`:
  - hướng dẫn chọn xe và thanh toán cọc.
- `hủy đơn`:
  - hướng dẫn vào đơn thuê và bấm hủy.
- `trả xe`:
  - hướng dẫn gửi yêu cầu trả xe.
- `thanh toán`, `cọc`:
  - giải thích VNPay/MoMo/ZaloPay.
- không hiểu:
  - đề xuất chuyển live chat.

## Escalation
Khi click `Gặp người hỗ trợ`:
- Tạo support ticket status `WAITING_OWNER`.
- Hiển thị message:
  - `Tôi đã chuyển cuộc trò chuyện cho người cho thuê hoặc bộ phận hỗ trợ.`
- Disable AI auto-answer hoặc đổi mode sang live chat.

## JS state
```js
const chatbotState = {
  isOpen: false,
  mode: "AI",
  conversationId: null,
  messages: []
};
```

## JS cần có
- Open/close/minimize chatbot.
- Render messages.
- Send message bằng Enter.
- Quick replies.
- Typing indicator.
- Mock AI response.
- Escalate to live chat.
- Lưu messages vào localStorage để demo lịch sử.

## API gợi ý
```txt
POST /api/support/conversations
POST /api/support/conversations/{conversationId}/messages
POST /api/support/conversations/{conversationId}/escalate
```

## Lưu ý UI
- Không làm widget che button quan trọng trên mobile.
- Tin nhắn dài cần wrap.
- Message area auto scroll xuống cuối.
