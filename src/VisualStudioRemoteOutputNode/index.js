var http = require('http'),
    qs = require('querystring'),
    logger = require('./logger');

var log = new logger();


var server = http.createServer(function(req, res) {
  
  req.on('error', function(e){
      console.log("Error: " + e);
    });
  
  if (req.method === 'POST' && req.url === '/') {
    var body = '';
    req.on('data', function(chunk) {
      body += chunk;
    });
    
    req.on('end', function() {    
      
      log.log(body);
      
      res.writeHead(200);
      res.end("");
    });
  } else {
    res.writeHead(404);
    res.end();
  }
});


server.listen(5050, (e)=>{
  console.log(`listening`);
});

