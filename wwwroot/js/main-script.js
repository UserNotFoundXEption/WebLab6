$(document).ready(function () {
    const socket = new WebSocket("ws://127.0.0.1:2137/ws");

    socket.onopen = function () {
        console.log("Connected to WebSocket server.");
        if (window.location.pathname == '/admin') {
            sendMessageFetchChat(1);
            sendMessageFetchChat(2);
            sendMessageFetchChat(3);
            sendMessageFetchReports(0);
        }
        else {
            sendMessageFetchChat(chatGetUserId());
            sendMessageFetchReports(chatGetUserId());
        }
    };

    socket.onmessage = function (event) {
        const message = JSON.parse(event.data);
        const data = message.data;
        switch (message.type) {
            case "sendOnChat":
                chatSendMessage(data.senderId, data.receiverId, data.message);
                break;
            case "fetchChat":
                data.forEach((chatMessage) => {
                    chatSendMessage(chatMessage.senderId, chatMessage.receiverId, chatMessage.message);
                });
                break;
            case "addReport":
                if (window.location.pathname == '/admin') {
                    reportsAddReportAdmin(data.reportId, data.userId, data.report, data.status)
                }
                else {
                    reportsAddReportUser(data.reportId, data.report, data.status);
                }
                break;
            case "deleteReport":
                reportsDeleteReport(data.reportId);
                break;
            case "fetchReports":
                if (window.location.pathname == '/admin') {
                    data.forEach((report) => {
                        reportsAddReportAdmin(report.reportId, report.userId, report.report, report.status);
                    });
                }
                else {
                    data.forEach((report) => {
                        reportsAddReportUser(report.reportId, report.report, report.status);
                    });
                }
                break;
            case "editReport":
                reportsChangeStatus(data.reportId, data.status);
                break;
            
        }
    };

    socket.onclose = function () {
        console.log("Disconnected from WebSocket server.");
    };

    socket.onerror = function (error) {
        console.error("WebSocket error: ", error);
    };

    function sendMessage(message) {
        socket.send(JSON.stringify(message));
    }

    window.sendMessageAddReport = function (userId, report) {
        sendMessage({ type: "addReport", userId, report });
    }

    window.sendMessageDeleteReport = function (reportId) {
        sendMessage({ type: "deleteReport", reportId });
    }

    window.sendMessageEditReport = function (reportId, status) {
        sendMessage({ type: "editReport", reportId, status });
    }

    window.sendMessageFetchReports = function (userId) {
        sendMessage({ type: "fetchReports", userId });
    }

    window.sendMessageChat = function (senderId, receiverId, message) {
        sendMessage({ type: "sendOnChat", senderId, receiverId, message });
    }

    window.sendMessageFetchChat = function (chatId) {
        sendMessage({ type: "fetchChat", chatId });
    }
});