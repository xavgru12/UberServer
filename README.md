## Server Setup

This version is using .NET 6 and MongoDB as database. Both installers are added to the release.

### MongoDB setup

Install it using the installer. Create a new database with user. Install mongosh tools as well. Both of it is added in every release.
It is installed by default in C:\Program Files\MongoDB\Server\5.0\bin. Open this folder from command prompt and type:  

#### initialize a database

enter mongo CLI
```bash 
mongo
```
Change to the database admin. This step is crucial and the name has to be admin
```bash 
use admin
```

Create a user
```bash
db.createUser({   
  user: "uber",    
  pwd: "admin",     
  roles: [    
    { role: "userAdmin", db: "admin" },   
    { role: "dbAdmin",   db: "admin" },     
    { role: "readWrite", db: "admin" }    
  ]    
}); 
```

You might need to add root for using Backup and Restore commands
```bash
db.grantRolesToUser('uber', [{ role: 'root', db: 'admin' }])
```

Set security setting by opening mongod.cfg and add:  
```bash   
security:    
  authorization: "enabled"
```

#### backup a database
Make sure you have access rights to the folder where mongo is installed: C:\Program Files\MongoDB\Server\5.0\bin (eg witout needing to type password in for creating folders/files)

Use mongodump which is part of mongosh tools:
```bash 
mongodump --host localhost:27017 --username uber --password admin --authenticationDatabase admin  
```
A folder called /dump is created.

#### restore a database
Use mongorestore which is part of mongosh tools. This command will look for the folder /dump and restore it:
```bash 
mongorestore --drop   
```

#### restart MongoDB   

  
```bash   
net stop MongoDB     
net start MongoDB     
```
#### Access Database:    
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
Compile all Releases using Visual Studio.  
Get files from bin/Release.  
Get Webservice and copy to ServerSetup/Webservice   
Get Realtime server:   
Get from src\UberStrok.Realtime.Server.Comm and copy to ServerSetup/PhotonRealtimeServer/UberStrok.Realtime.Server.Comm   
Get from src\UberStrok.Realtime.Server.Game and copy to ServerSetup/PhotonRealtimeServer/UberStrok.Realtime.Server.Game/bin 
Paste your discord-config to ServerSetup/Webservices/assets/configs/discord.config.      
Finally copy ServerSetup to your server.   

### Webservice configuration
IP address is set to 127.0.0.1 per default for both Webservice and Realtime.
Webservice Server can contain Realtime, while Realtime could be one or more separate servers.

Take your servers' public IP address(es) and put it into following config files.

in Webservice Server:
Edit Webservices/configs/game/servers.json   
CommServer is the Webservice server.   
GameServer is the Realtime server.   

in Realtime Server:    
Edit uberstrok.realtime.server.json, default content is: "webservices": "http://127.0.0.1:5000/2.0/",         
Locations:    
PhotonRealtimeServer\UberStrok.Realtime.Server.Comm\bin and    
PhotonRealtimeServer\UberStrok.Realtime.Server.Game\bin         

### Run Server
Install dotnet 6.0 on server. Dotnet Installer is in the Github release.
Start the Webservice by executing the .exe from Webservices.    

Steps to start Realtime:   
Paste Photon License to: bin_Win64 or bin_Win32  
It might show .NET Framework 3.5 missing, which can be ignored.   
Start PhotonController.exe.  
The PhotonController will be started at your taskbar tray, rightclick and find UberStrok, click Start as application.   

#Steps for Realtime not starting (PhotonLicense.dll Blocked).
Link: https://stackoverflow.com/questions/34400546/could-not-load-file-or-assembly-operation-is-not-supported-exception-from-hres
Check the PhotonCLR.log file to get the error code that cause the service to shutdown.
if you see this error (HRESULT: 0x80131515) it means that the PhotonLicensing.dll is blocked.
Location:
PhotonRealTimeServer/bin_Win64 or 
PhotonRealTimeServer/bin_Win32
Right click the PhotonLicensing.dll and open the file properties.
There will be a security flag noting that the file came from another computer and might be blocked to protect your computer.
Select the unblock option and Apply the change.
Realtime servers should start fine now.
