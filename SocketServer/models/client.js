var Client = function(id)
{
	this.id = id;
}

var proto = Client.prototype;
proto.constructor = Client;


module.exports = Client;