var express = require('express'),
	app = express(),
	httpServer = require('http').createServer(app),
	Server = require('./models/server');

httpServer.listen(80);

app.use(express.static(__dirname + '/public'));
app.get('/', function (req, res) {
  res.sendfile(__dirname + '/index.html');
});

var server = new Server(httpServer);
