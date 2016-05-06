# What's this?
I needed to implement a networking application with a server in a local network, with no Internet access.
Then I found that unity5 networking api providing exactly what i want.
So I studied manual, scripting API and made simple & light implementation providing basic features.
It will be useful for non-game, which like manage client's realtime status.

In my case, UNET is actually quite strong.
I am using this unity multiplayer feature as a kind of test tool for children. About 2~30 mobile devices connect to server(also mobile device), and then test result are displayed on server in realtime. 


# Features
- Auto discovery( make clients discover server's address at startup )
- Manage client connection status with scroll view(turn to red if disconnected)
- Send custom network message.(simple chat)
- Auto reconnecting when the client has lost connection.
- Make your own UI for the NetworkManager instead of NetworkManager HUD Component

# The fast track
All you have to do to start using it in your project:

1. [Download this zip](https://github.com/ifndefdeadmau5/unity5-networking-HLAPI-getting-started/archive/master.zip), Open with editor.
2. Execute at least two unity app and start each server and client mode.
3. **You’re done!**

# Screenshots
![alt tag](https://raw.githubusercontent.com/ifndefdeadmau5/unity5-networking-HLAPI-getting-started/master/Assets/Screenshot/menu.png)
![alt tag](https://raw.githubusercontent.com/ifndefdeadmau5/unity5-networking-HLAPI-getting-started/master/Assets/Screenshot/server.png)
