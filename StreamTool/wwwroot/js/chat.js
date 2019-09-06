"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/ChatMessageHub").build();

// Delete chat message
function deleteMessage() {
    connection.invoke("DeleteMessage", this.id).catch(function (err) {
        return console.error(err.toString());
    });
}

// Delete all chat messages
function deleteMessages() {
    connection.invoke("DeleteMessages").catch(function (err) {
        return console.error(err.toString());
    });
}

// Append chat message
function appendMessage(message) {
    var id = message.msgId;
    var sender = message.sender;
    var msg = message.message;

    // Get template
    var template = document.querySelector('#messageTempalte');

    // Copy template content (true = deep copy)
    var messageContainer = document.importNode(template.content, true);

    // Add unique container id
    messageContainer.querySelector('#messageContainer').id = "msg_" + id;

    // Add username
    messageContainer.querySelector('#usernameTemplate').innerText = sender + ": ";

    // Add message text
    messageContainer.querySelector('#textTemplate').innerText = msg;

    // Add delete functionality
    var deleteTemplate = messageContainer.querySelector('#deleteTemplate');
    deleteTemplate.id = id;
    deleteTemplate.onclick = deleteMessage;

    document.getElementById("messageList").appendChild(messageContainer);
}

// Connection established
connection.start().then(function () {
    console.log("Connected successfully")
    var deleteAllButton = document.getElementById("clearAll");
    deleteAllButton.disabled = false;
    deleteAllButton.onclick = deleteMessages;
}).catch(function (err) {
    return console.error(err.toString());
});

// Recived initial messages
connection.on("AllMessages", function (messages) {
    for (var i in messages) {
        appendMessage(messages[i]);
    }
});

// Delete message
connection.on("DeleteMessage", function (id) {
    var messageDiv = document.getElementById("msg_" + id);
    messageDiv.remove();
});

// Delete all message
connection.on("DeleteMessages", function (ids) {
    for (var i in ids) {
        var messageDiv = document.getElementById("msg_" + ids[i]);
        messageDiv.remove();
    }
});

// Recived new message
connection.on("AddMessage", function (message) {
    appendMessage(message);
});
