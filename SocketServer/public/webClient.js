var WebClient = function()
{
	this.$inputBox = $('.js-message-input');
	this.$sendButton = $('.js-send-message');
	this.$clientList = $('.js-client-list');
	this.$messageList = $('.js-messages');

	this.socket = io.connect('http://localhost');

	this.bindEvents();
}

var proto = WebClient.prototype;
proto.constructor = WebClient;

proto.bindEvents = function()
{
	this.$sendButton.on('click', this.buttonClick.bind(this));
	this.socket.on('serverData', this.handleServerData.bind(this));
}

proto.handleServerData = function(data)
{
	switch(data.event)
	{
		case "AvailableClients" : this.listAvailableClients(data.data); break;
		case "ClientId" : this.setClientId(data.data); break;
		case "Message" : this.handleMessage(data.data); break;
		default : console.log("error handling server for event: "+data.event);
	}
}

proto.handleMessage = function(message)
{
	this.$messageList.append("<li>"+message.client+": " +message.text+"</li>");
}

proto.setClientId = function(id)
{
	$('.js-welcome').text("Welcome "+id+"!");
}

proto.sendServerData = function(event, data)
{
	this.socket.emit('clientData', { event: event, data: data  });
}

proto.listAvailableClients = function(clients)
{
	this.$clientList.html("");

	clients.forEach(function(client)
	{
		this.$clientList.append("<option value='"+client.id+"'>"+client.id+"</option>");
	}.bind(this));
}

proto.buttonClick = function(e)
{
	this.sendServerData('Message',{
		text: this.$inputBox.val(),
		clientId: this.$clientList.val()
	});
}


$(function() {
	var webClient = new WebClient();
});