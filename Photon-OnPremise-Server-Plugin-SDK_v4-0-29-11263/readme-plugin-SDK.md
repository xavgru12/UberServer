# For more infos see
http://doc.photonengine.com/en/onpremise/current/applications/loadbalancing/plugins
http://doc.photonengine.com/en/onpremise/current/applications/loadbalancing/plugins-faq

# Quickstart of the Photon Server Plugin SDK

1) extract photon-sdk-dotnet-demo-turnbased-console-X.X.X.XX-XXXX.zip

2) IMPORTANT: downloaded files (selfextracting exes, zips or 7z files) should allways be **unblocked** before you unpack them!
   see here for details: https://blogs.msdn.microsoft.com/delay/p/unblockingdownloadedfile/
 
3) Unpack Photon-OnPremise-Server-Plugin-SDK_vx-x-xx-xxxx.zip

4) Unpack photon-sdk-dotnet-demo-turnbased-console-X.X.X.XX-XXXX.zip

5) cd Photon-OnPremise-Server-Plugin-SDK_vx-x-xx-xxxx\src-server\Plugins

6) Open WebHooks.sln 
   IMPORTANT: per mouse click (or keyboard)
   NOTE: if you open the solution per file dialog in Visual Studio then F5 (start debug) will fail to start photon

7) Open WebhooksPlugin.cs. Go to the first line in OnCreateGame(...) and set a breakpoint.

8) Press F5 to start Photon in debug mode

9) cd demo-turnbased-console\demo-turnbased-console\bin\Debug

10) Start TurnbasedConsole.exe - you should see the following output (once you press enter)

    Enter player #: 0 (= default, press enter)
    connecting to:localhost:5055 as:MyPlayer0
    State: ConnectingToMasterserver - localhost:5055
    h   | Help
    sa  | Set Actor : actornr as int
    sr  | Set Room : roomname
    c   | Create Room : roomname
    j   | Join Room : roomname
    jc  | Join Or Create Room : roomname
    jr  | Re-Join Room : roomname
    sro | Set Room isOpen : true|false
    srv | Set Room isVisible : true|false
    llr | List Lobby Rooms
    lsg | List Saved Games
    gsg | Get Saved Games
    d   | Disconnect
    l   | Leave (abandon)
    q   | Quit
    ce  | Cached Event (not forwarded) : event as string
    fe  | Forward Event  (non-cached) : event as string
    sii | Slice Increase Index
    ssi | Slice Set Index : index as int
    spi | Slice Purge Index : index as int
    spu | Slice Purge Up To Index : index as int
    spt | Set Property Turn : actornr as int
    
    For help (list of all available commands see above) press: h<ENTER>
    Note: commands and parameteres are entered in one line separated trough ':',
    for example c:myroom<ENTER> will create a room with name 'myroom'.

    ------------------------ continuing to connect ...
    StatusCode: Connect
    StatusCode: EncryptionEstablished
    State: ConnectedToMaster

11) Next create a room by typing 

    c<Enter>

This should trigger the breakpoint. Press F5 to continue - you should see the following lines:

    >>> Create Room:
    State: ConnectingToGameserver - 127.0.0.1:5056
    StatusCode: Disconnect
    StatusCode: Connect
    StatusCode: EncryptionEstablished
    State: ConnectingToGameserver - 127.0.0.1:5056
    a[1]: joined

12) To get all available commands type

    h<Enter>

