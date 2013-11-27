var PORT_NUMBER = 8008;

var //app = require('http').createServer(handler),
   //io = require('socket.io').listen(app),
   fs = require('fs'),
   os = require('os'),
   util = require('util'),
   Models = require('./Models');

// testing clients
var newClient = new Models.Client("client1");
var notClient = new Models.Body("body1");

Models.Clients.addClient(newClient);
Models.Clients.addClient(notClient);
Models.Clients.removeClient("client1");

// testing vectors
var vector1 = new Models.Vector2(2.5,2);
console.log(vector1);
vector1.normalize();
console.log(vector1);
console.log('---');
var vector2 = new Models.Vector2(2,2.5);
console.log(vector2);
vector2.normalize();
console.log(vector2);
console.log('---');
var vector3 = new Models.Vector2(1,1);
console.log(vector3);
vector3.normalize();
console.log(vector3);

// io.set('log level', 1); // reduce logging

// app.listen(PORT_NUMBER);
// console.log("Listening on port " + PORT_NUMBER + ".");
// console.log("To test the web-based socket.io client, navigate to http://localhost:" + PORT_NUMBER + "/index.html");

// console.log("This server may also be available at the following IP Addresses:");
// var interfaces =os.networkInterfaces();
// for (var dev in interfaces) {
//   var alias=0;
//   interfaces[dev].forEach(function(details){
//     if (details.family=='IPv4') {
//       console.log(details.address + ":" + PORT_NUMBER + "\t (" + dev+ ((alias) ? (":"+alias) : '') + ")");
//       ++alias;
//     }
//   });
// }


// function handler (req, res) {
//   fs.readFile(__dirname + '/index.html',
//   function (err, data) {
//     if (err) {
//       res.writeHead(500);
//       return res.end('Error loading index.html');
//     }

//     res.writeHead(200);
//     res.end(data);
//   });
// }


// var currentX = 0, currentY = 0;

// io.sockets.on('connection', function (socket) {
//   console.log("\r\n");
//   console.log("\r\n");
//   socket.emit('news', { hello: 'world' });
//   socket.on('my other event', function (data) {
//     console.log(data);
//   });
//   socket.on('checkin', function(data){
//     console.log("check in from " + data.checkin + " (client: " + socket.id + ")");
//     socket.emit('news', {status: "success"});
//   });
//   socket.on('positionChange', function(data){
//     socket.broadcast.emit('acceptPosition', {x: data.x, y: data.y});
//   });
// });
