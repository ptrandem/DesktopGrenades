var Client = function(socket)
{
	this.id = socket.id;
	this.socket = socket;
}

var proto = Client.prototype;
proto.constructor = Client;


module.exports = Client;