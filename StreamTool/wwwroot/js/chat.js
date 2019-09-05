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

    // Setup message container
    var msgDiv = document.createElement('div');
    msgDiv.className = "chat-message center-block";
    msgDiv.id = "msg_" + id;

    // Setup sender and delete icon container
    var titelContainer = document.createElement('div');
    titelContainer.className = "row";

    var senderContainer = document.createElement('div');
    senderContainer.className = "col-xs-8 col-md-6";

    var deleteContainer = document.createElement('div');
    deleteContainer.className = "col-xs-4 col-md-6 text-right";

    // Setup sender content
    var senderContent = document.createElement('h4');
    senderContent.className = "message-name";
    senderContent.innerText = sender

    // Setup delete content
    /*
     * Font awesome <i> tags get converted to <svg> elements.
     * For that reason, we have to add the onclick event to a surrounding <span> tag.
     * 
     */ 
    var deleteContent = document.createElement('span');
    deleteContent.innerHTML = "<i class=\"fa fa-trash fa-fw\" aria-hidden=\"true\"></i>";
    deleteContent.id = id;
    deleteContent.onclick = deleteMessage;

    // Setup message text container
    var messageContainer = document.createElement('div');
    messageContainer.className = "row message-text";
    messageContainer.innerText = msg;

    // Put everything into msg div
    senderContainer.appendChild(senderContent);
    deleteContainer.appendChild(deleteContent);

    titelContainer.appendChild(senderContainer);
    titelContainer.appendChild(deleteContainer);

    msgDiv.appendChild(titelContainer);
    msgDiv.appendChild(messageContainer);

    document.getElementById("messageList").appendChild(msgDiv);
}

// Connection established
connection.start().then(function () {
    console.log("Connected successfully")
    var deleteAllButton = document.getElementById("clearAll");

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
