var PORT_NUMBER = 8008;
var app = require('http').createServer(handler)
  , io = require('socket.io').listen(app)
  , fs = require('fs')

app.listen(PORT_NUMBER);
console.log("Listening on port " + PORT_NUMBER + ".");
console.log("To test the web-based socket.io client, navigate to http://localhost:" + PORT_NUMBER + "/index.html");

function handler (req, res) {
  fs.readFile(__dirname + '/index.html',
  function (err, data) {
    if (err) {
      res.writeHead(500);
      return res.end('Error loading index.html');
    }

    res.writeHead(200);
    res.end(data);
  });
}

io.sockets.on('connection', function (socket) {
  socket.emit('news', { hello: 'world' });
  socket.on('my other event', function (data) {
    console.log(data);
  });
  socket.on('checkin', function(data){
  	console.log("check in from " + data.checkin);
  });
});
