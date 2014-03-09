var app = 
	io = require('socket.io'),
 	Client = require('./client');

var Server = function(httpServer)
{
	this.clients = [];

	io = io.listen(httpServer);
    io.sockets.on('connection', this.newConnection.bind(this));
}

var proto = Server.prototype;
proto.constructor = Server;

proto.newConnection = function(socket)
{
	var client = new Client(socket);

	client.socket.on('clientData', function (data) {
		this.handleClientData(client, data);
	}.bind(this));

	this.clients.push(client);
	
	this.sendGlobalData("New user has joined!");

}

proto.handleClientData = function(client, data)
{
	console.log(data);
}

proto.sendGlobalData = function(data)
{
	var i;
	for(i = 0; i < this.clients.length; i++)
	{
		var client = this.clients[i];
		client.socket.emit("serverData", data);
	}
}

module.exports = Server;