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
	var client = new Client(socket.id);

	socket.on('clientData', function (data) {
		this.handleClientData(client, data);
	}.bind(this));

	this.clients.push(client);
	this.sendClientData(client, "ClientId", client.id);
	this.sendGlobalData("AvailableClients", this.clients);

}

proto.handleClientData = function(client, data)
{
	switch(data.event)
	{
		case "Message" : this.handleMessage(client, data.data); break;
		default : console.log("error handling server for event: "+data.event);
	}
}

proto.handleMessage = function(client, message)
{
	var destinationClient = this.findClientById(message.clientId);
	if(destinationClient)
	{
		this.sendClientData(destinationClient,"Message", {client:client.id, text:message.text});
	}
}

proto.findClientById = function(id)
{
	var i;
	for(i = 0; i < this.clients.length; i++)
	{
		if(this.clients[i].id == id) return this.clients[i];
	}
	return null;
}

proto.sendClientData = function(client, event, data)
{
	io.sockets.socket(client.id).emit("serverData", {event: event, data: data})
}

proto.sendGlobalData = function(event, data)
{
	var i;
	for(i = 0; i < this.clients.length; i++)
	{
		var client = this.clients[i];
		io.sockets.socket(client.id).emit("serverData", {event: event, data: data})
	}
}

module.exports = Server;