## Server Setup

This version is using .NET 3 and MongoDB as database. Both installers are added to the repo.

### MongoDB setup

Install it using the installer. Create a new database with user.    
It is installed by default in C:\Program Files\MongoDB\Server\5.0\bin. Open this folder from command prompt and type: mongodb    
mongodb command line should open, create a user:   

db.createUser({   
  user: "uber",    
  pwd: "admin",     
  roles: [    
    { role: "userAdmin", db: "admin" },   
    { role: "dbAdmin",   db: "admin" },     
    { role: "readWrite", db: "admin" }    
  ]    
});    
    
Database name must be admin.    
Open mongod.cfg and add:    
security:    
  authorization: "enabled"    

Backup Database:     

mongodump --host localhost:27017 --username uber --password admin --authenticationDatabase admin     

Restore Database:    
mongorestore --drop     

Restart MongoDB:     
net stop MongoDB     
net start MongoDB     

Access Database:    
Open MongoDBCompass     

filter user by CMID: { UserId : { $in: [1, 2, 232] }}    

### Discord Bot Setup
Go to discord developer portal.   
Create new application -> create new Bot         
Go to Oauth2:    
set scopes: bot, application.commands    
set bot permissions: Send Messages, Create Public Threads, create Private      Threads,     Manage Messages, Manage Threads     

Generate token, invite Discord bot to the server.        
In assets/configs discord.config, set token and channel ids from your discord server      
To get a channel id: right click on a channel -> copy link, you will get something like this:     
https://discord.com/channels/977629304894656606/1074770011199119521      
Only take the last part: 1074770011199119521      
Put this into the discord.config, for example:     
lobbychannel:1074770011199119521     

Discord token is a secret and is not to be published.

### Firewall Settings

Go to Firewall Advanced Settings

Inbound -> TCP -> Port 5000 -> allow connections

Inbound -> UDP -> Port 5055 and 5056 -> allow connections

Outbound -> TCP -> Port 5000 -> allow connections

Outbound -> UDP -> Port 5055 and 5056 -> allow connections

Same settings need to be applied on both the Windows as well as in webhost settings.

### Photon License

Get your free license (100 CCU) from Photon's Website: https://www.photonengine.com/server

### Get server files
copy /photon to server.
Install MongoDB and dotnet 3 on server.

### Webservice configuration
IP address is set to 127.0.0.1 per default for both Webservice and Realtime.
Webservice Server can contain Realtime, while Realtime could be one or more separate servers.

Take your servers' public IP address(es) and put it into following config files.

in Webservice Server:
Edit Webservices/assets/configs/game/servers.json   
CommServer is the Webservice server.   
GameServer is the Realtime server.   

in Realtime Server:    
Edit IP address in uberstrok.realtime.server.json, default line content is: "webservices": "http://127.0.0.1:5000/2.0/",   
Locations:    
photon\deploy\UberStrok.Realtime.Server.Comm and    
photon\deploy\UberStrok.Realtime.Server.Game   

### Run Server
Install dotnet 3.0 on server. Dotnet Installer is in repo.
Start the Webservice by executing the .exe from Webservices.    

Steps to start Realtime:   
Paste Photon License to: bin_Win64 or bin_Win32  
It might show .NET Framework 3.5 missing, which can be ignored.   
Start PhotonController.exe.  
The PhotonController will be started at your taskbar tray, rightclick and find UberStrok, click Start as application.   

### Client Setup

Client can be found in UberClient with the same branch name.
