var BodyStateEnum = {
	INCLIENT : 0,
	INTRANSITION : 1	
};

function Vector2(x, y) {
	this.x = x;
	this.y = y;
}

/* **********************************
	VECTOR OBJECT
*/
Vector2.prototype.normalize = function() {
	if(this.x >= this.y) {
		var scale = this.y / this.x;
		this.x = 1;
		this.y = 1 * scale;
	} else {
		var scale = this.x / this.y;
		this.y = 1;
		this.x = 1 * scale;
	}
};

/* **********************************
	CLIENT OBJECT
*/
function Client(clientId) {
	this.clientId = clientId;
	this.clientName = "";
	this.northClientId = "";
	this.westClientId = "";
	this.eastClientId = "";
	this.activeBodies = [];
}

Client.prototype.reconfigure = function(clientConfig) {
	this.northClientId = clientConfig.northClientId;
	this.westClientId = clientConfig.westClientId;
	this.eastClientId = clientConfig.eastClientId;
};

Client.prototype.addBody = function(body) {
	this.activeBodies.push(body);
};

Client.prototype.getBodyById = function(bodyId) {
	for(var i = 0; i < this.activeBodies.length; i ++) {
		if(this.activeBodies[i] == bodyId) {
			return this.activeBodies[i];
		} 
		return null;
	}
};

/* **********************************
	BODY OBJECT
*/
function Body(id) {
	this.bodyId = id;
	this.bodyName = "";
	this.CurrentClientId = "";
	this.xPos = 50;
	this.yPos = 50;
	this.bodyState = BodyStateEnum.INCLIENT;
	this.MessageContent = "";
	this.Base64ImageContent = "";
	this.MessageTargetClientId = "";
}

function Transition(ctor) {
	this.sourceClientId = ctor.sourceClientId;
	this.targetClientId = ctor.targetClientId;
	this.bodyId = ctor.bodyId;
	this.xPos = 50;
	this.yPos = 50;
	this.velocity = Vector2(0,0);
	this.torque = 0;
}

/* **********************************
	BODIES COLLECTION OBJECT
*/
function Bodies () {
	this.bodies = [];
};

Bodies.prototype.getAll = function() {
	return this.bodies;
};

Bodies.prototype.getBodyById = function(id) {
	for(var i = 0; i < this.bodies.length; i++) {
		if(this.bodies[i].bodyId == id) {
			return this.bodies[i];
		}
	}
	return null;
};

Bodies.prototype.addBody = function(body) {
	if(body instanceof Body) {
		this.bodies.push(body);
	} else {
		logInvalidObject(body, "Body");
	}
};

Bodies.prototype.attachTextMessage = function(bodyId, messageText, messageTargetClientId) {
	var body = this.getBodyById(bodyId);
	if(body != null) {
		body.MessageContent = messageText;
		if(messageTargetClientId) {
			body.MessageTargetClientId = messageTargetClientId;
		}
	}
}

/* **********************************
	CLIENTS COLLECTION OBJECT
*/

function Clients () {
	this.clients = [];
};

Clients.prototype.addClient = function(client) {
	if(client instanceof Client) {
		this.clients.push(client);
	} else {
		logInvalidObject(client, "Client");
	}
};

Clients.prototype.getAll = function() {
	return this.clients;
};

Clients.prototype.getClientById = function(id){
	for(var i = 0; i < this.clients.length; i++) {
		if(this.clients[i].clientId == id) {
			return this.clients[i];
		}
	}
	return null;
};

Clients.prototype.removeClient = function(clientId) {
	var client = this.getClientById(clientId);
	var index = this.clients.indexOf(client);
	if(index >= 0) {
		this.clients.splice(index, 1);
	}
};

var logInvalidObject = function(object, objectTypeDescription) {
	console.log("A '" + object.constructor.name + "' is not a valid " + objectTypeDescription + ":");
	console.log(object);
	console.log();
}

/* **********************************
	EXPORTS
*/

// Enums
module.exports.BodyStateEnum = BodyStateEnum;

// Helper Types
module.exports.Vector2 = Vector2;

// Main Types
module.exports.Client = Client;
module.exports.Body = Body;
module.exports.Transition = Transition;

// Collections
module.exports.Clients = new Clients();