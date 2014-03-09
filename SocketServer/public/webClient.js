var WebClient = function()
{
	this.$inputBox = $('.js-message-input');
	this.$sendButton = $('.js-send-message');
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
	console.log(data);
}

proto.buttonClick = function(e)
{
	this.socket.emit('clientData',this.$inputBox.val());
}


$(function() {
	var webClient = new WebClient();
});