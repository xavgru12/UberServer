## Server Setup

This version is using .NET 6 and LiteDB as database. In Serversetup/Webservices the LiteDB client is added.

### WAF setup

Go to Serversetup/WAF and edit the appsettings,json
add IP address of your server, default is: 13.38.94.69
WAF is used to redirect the webservices for better security. A domain shall be used in a live version. Putting in the IP address is a little hack for easy setup.
If the domain shall be used, it needs to be set in:
WAF -> appsettings.json like the example for live.uberforever.eu
and in Realtime Server, look in Webservice configuration for more info.

### Firewall Settings

Go to Firewall Advanced Settings

Inbound -> TCP -> Port 80 -> allow connections

Inbound -> UDP -> Port 5055 and 5056 -> allow connections

Same settings need to be applied on both the Windows as well as in webhost settings

### Photon License

Get your free license (100 CCU) from Photon's Website: https://www.photonengine.com/server

### Get server files
Compile all Releases using Visual Studio.  
Get files from bin/Release.  
Get Webservice and copy to ServerSetup/Webservice   
Get Realtime server:   
Get from src\UberStrok.Realtime.Server.Comm and copy to ServerSetup/PhotonRealtimeServer/UberStrok.Realtime.Server.Comm   
Get from src\UberStrok.Realtime.Server.Game and copy to ServerSetup/PhotonRealtimeServer/UberStrok.Realtime.Server.Game/bin
copy ServerSetup to your server.   

### Webservice configuration
IP address is set to 127.0.0.1 per default for both Webservice and Realtime.
Webservice Server can contain Realtime, while Realtime could be one or more separate servers.

Take your servers' public IP address(es) and put it into following config files.

in Webservice Server:
Edit Webservices/configs/game/servers.json   
CommServer is the Webservice server.   
GameServer is the Realtime server.   

in Realtime Server:    
Edit Webservice.txt, default content is: http://127.0.0.1/   
Locations:    
PhotonRealtimeServer\UberStrok.Realtime.Server.Comm and    
PhotonRealtimeServer\UberStrok.Realtime.Server.Game   

### Run Server
Install dotnet 6.0 on server. Dotnet Installer is in /ServerSetup.
Start the Webservice by executing the .exe from Webservices.    

Steps to start Realtime:   
Paste Photon License to: bin_Win64 or bin_Win32  
It might show .NET Framework 3.5 missing, which can be ignored.   
Start PhotonController.exe.  
The PhotonController will be started at your taskbar tray, rightclick and find UberStrok, click Start as application.   

### Client Setup

Compile UberStrok.Patcher, go to its bin/Release and execute.
