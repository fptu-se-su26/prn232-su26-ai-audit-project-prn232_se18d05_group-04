const VivuCarChatbot = (function() {
    let state = {
        isOpen: false,
        sessionId: null,
        messages: [],
        connection: null
    };

    const apiBase = "http://localhost:7005/api";
    const hubUrl = "http://localhost:7005/hubs/chat";

    function getToken() {
        return localStorage.getItem("vivucar_token") || "";
    }

    function getUserIdFromToken() {
        const token = getToken();
        if (!token) return null;
        try {
            let base64 = token.split(".")[1].replace(/-/g, "+").replace(/_/g, "/");
            while (base64.length % 4) base64 += "=";
            const payload = JSON.parse(atob(base64));
            return payload.sub ? parseInt(payload.sub, 10) : null;
        } catch {
            return null;
        }
    }

    function getAnonymousSessionId() {
        return localStorage.getItem("vivucar_anonymous_session_id");
    }

    function setAnonymousSessionId(id) {
        localStorage.setItem("vivucar_anonymous_session_id", id);
    }

    async function initSession() {
        if (state.sessionId) return;

        try {
            let token = getToken();
            const res = await fetch(`${apiBase}/support/conversations`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    ...(token ? { 'Authorization': `Bearer ${token}` } : {})
                },
                body: JSON.stringify({})
            });

            if (res.ok) {
                const data = await res.json();
                state.sessionId = data.id;
                state.messages = data.messages || [];

                if (!token) setAnonymousSessionId(state.sessionId);

                renderMessages();

                if (state.messages.length === 0) {
                    appendSystemMessage("Xin chào, tôi có thể hỗ trợ bạn tìm xe, đặt xe, thanh toán cọc hoặc xử lý vấn đề sau chuyến đi.");
                }

                await connectSignalR();
            } else {
                appendSystemMessage("Không thể khởi tạo phiên trò chuyện.");
            }
        } catch (e) {
            console.error("Failed to init chat session", e);
            appendSystemMessage("Không thể kết nối đến máy chủ hỗ trợ lúc này.");
        }
    }

    async function connectSignalR() {
        if (state.connection) return;

        state.connection = new signalR.HubConnectionBuilder()
            .withUrl(hubUrl, {
                accessTokenFactory: () => getToken()
            })
            .withAutomaticReconnect()
            .build();

        state.connection.on("ReceiveMessage", (message) => {
            if (!state.messages.find(m => m.id === message.id)) {
                state.messages.push(message);
                renderMessages();
            }
        });

        try {
            await state.connection.start();
            await state.connection.invoke("JoinSession", state.sessionId.toString());
        } catch (err) {
            console.error("SignalR Connection Error: ", err);
        }
    }

    function toggleChat() {
        state.isOpen = !state.isOpen;
        const panel = document.getElementById("chatbotPanel");
        const overlay = document.getElementById("chatbotOverlay");
        const badge = document.getElementById("chatbotBadge");

        if (state.isOpen) {
            panel.classList.add("active");
            overlay.classList.add("active");
            badge.style.display = "none";
            initSession();
        } else {
            panel.classList.remove("active");
            overlay.classList.remove("active");
        }
    }

    function closeChat() {
        state.isOpen = false;
        document.getElementById("chatbotPanel").classList.remove("active");
        document.getElementById("chatbotOverlay").classList.remove("active");
    }

    function renderMessages() {
        const container = document.getElementById("chatbotMessages");
        if (!container) return;

        container.innerHTML = "";

        state.messages.forEach(msg => {
            const div = document.createElement("div");
            let cssClass = 'bot';
            if (msg.role === 'user') cssClass = 'user';
            else if (msg.role === 'system') cssClass = 'system';
            else if (msg.role === 'assistant') cssClass = 'bot';

            div.className = `chat-msg ${cssClass}`;
            div.innerHTML = msg.content.replace(/\n/g, "<br>");
            container.appendChild(div);
        });

        scrollToBottom();
    }

    function appendMessage(role, content) {
        state.messages.push({ role, content, createdAt: new Date() });
        renderMessages();
    }

    function appendSystemMessage(content) {
        appendMessage('system', content);
    }

    async function sendMessage(content = null) {
        if (!state.sessionId) return;

        const inputEl = document.getElementById("chatbotInput");
        const text = content || inputEl.value.trim();

        if (!text) return;

        inputEl.value = "";

        try {
            if (state.connection && state.connection.state === signalR.HubConnectionState.Connected) {
                const senderId = getUserIdFromToken();
                await state.connection.invoke("SendMessage", state.sessionId.toString(), text, senderId, "user");
            } else {
                appendSystemMessage("Kết nối bị gián đoạn, không thể gửi tin nhắn.");
            }
        } catch (e) {
            console.error("Failed to send message", e);
            appendSystemMessage("Lỗi khi gửi tin nhắn.");
        }
    }

    function handleEnter(e) {
        if (e.key === 'Enter') {
            sendMessage();
        }
    }

    function sendQuickReply(text) {
        sendMessage(text);
    }

    function scrollToBottom() {
        const container = document.getElementById("chatbotMessages");
        if (container) {
            container.scrollTop = container.scrollHeight;
        }
    }

    return {
        toggleChat,
        closeChat,
        sendMessage,
        handleEnter,
        sendQuickReply
    };
})();
