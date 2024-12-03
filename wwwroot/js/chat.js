//wywolane z html zeby przeslac do websocketa odpowiednie parametry
function chatNewMessage(inputId) {
    const input = document.getElementById(inputId);
    const message = input.value.trim();
    if (message) {
        var senderId = chatGetUserId();
        var receiverId = 0;
        if (senderId == 0) {
            receiverId = parseInt(inputId.slice(-1), 10);
        }
        sendMessageChat(senderId, receiverId, message);
    }
}

//wszystko nizej wywolane po otrzymaniu wiadomosci od websocketa
function chatSendMessage(senderId, receiverId, message) {
    var userId = chatGetUserId();
    if (message && (userId == senderId || userId == receiverId)) {
        var color = chatGetColor(senderId);
        if (window.location.pathname == '/admin') {
            var id = chatGetChatId(senderId, receiverId)
            var chatBoxId = 'chatBox' + id;
            var inputId = 'chatInput' + id;
            chatAddMessageToGUI(chatBoxId, inputId, color, message);
        }
        else {
            chatAddMessageToGUI("chatBox", "chatInput", color, message);
        }
    }
}

function chatAddMessageToGUI(chatBoxId, inputId, color, message) {
    const messageElement = document.createElement("div");
    messageElement.textContent = message;
    messageElement.classList.add("w3-padding", "w3-round", color);

    const chatBox = document.getElementById(chatBoxId);
    chatBox.appendChild(messageElement);

    chatBox.scrollTop = chatBox.scrollHeight;
    const input = document.getElementById(inputId);
    input.value = '';
}

function chatGetChatId(senderId, receiverId) {
    var id = 0;
    if(senderId > 0){
        id = senderId;
    }
    if (receiverId > 0) {
        id = receiverId;
    }
    return id;
}

function chatGetColor(senderId) {
    if (senderId == 0) {
        return "w3-brown";
    }
    else {
        return "w3-light-blue";
    }
}

function chatGetUserId() {
    var userId = 0;
    if (window.location.pathname == '/user/') {
        userId = window.location.search.slice(-1);
    }
    return userId;
}