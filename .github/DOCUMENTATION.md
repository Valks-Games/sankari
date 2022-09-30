### Logger

`Log(object message, ConsoleColor color = ConsoleColor.Gray)` log a message

`LogWarning(object message, ConsoleColor color = ConsoleColor.Yellow)` log a warning

`LogTodo(object message, ConsoleColor color = ConsoleColor.White)` log a todo

`LogErr(Exception e, string hint = default, ConsoleColor color = ConsoleColor.Red, [CallerFilePath] string filePath = default, [CallerLineNumber] int lineNumber = 0)` log a error

`LogDebug(Exception e, string hint = default, ConsoleColor color = ConsoleColor.Red, [CallerFilePath] string filePath = default, [CallerLineNumber] int lineNumber = 0)` log a debug

`LogMs(Action code)` log the time it takes to do a section of code

### Audio

`LoadSFX(string name, string path)` load a sound effect, for e.g. `Audio.LoadSFX("player_jump", "Movement/Jump/sfx_movement_jump1.wav")`

`LoadMusic(string name, string path)` load a music track, for e.g. `Audio.LoadMusic("ice_1", "Level/Ice/Icy_Expanse.mp3")`

`PlayMusic(string name, float pitch = 1)` play a music track (pitch values between 0.1 and 1 are slower, values higher than 1.0 are faster)

`PlaySFX(string name, int volume = 100)` play a sound (volume ranges from 0 to 100)  

`SetSFXVolume(int v)` set the SFX volume (values range from 0 to 100)

`StopMusic()` stop the music track currently being played

### Net

`PingSent` returns the last ping sent from a pong packet

`DisconnectOpcode` returns the disconnect opcode recieved from the server to the client

`Client` returns the client

`Server` returns the server

`EnetInitialized` check to see if enet is initialized

`StartClient(string ip, ushort port, CancellationTokenSource cts)` start the client

`StartServer(ushort port, int maxPlayers, CancellationTokenSource cts)` start the server

`IsHost()` check to see if this client is the host

`IsMultiplayer()` check to see if the client or the server are running

### Notifications

`AddListener(Node sender, Event eventType, Action<object[]> action)` add a listener to a target node of a specified event with optional arguments

`RemoveListener(Node sender, Event eventType)` remove a listener from a target node

`RemoveAllListeners()` remove all listeners from all nodes

`RemoveInvalidListeners()` remove invalid listeners (for e.g. a scene is deinitialized but a listener wasn't removed from a node within that scene)

`Notify(Event eventType, params object[] args)` notify all listeners for a specified event with optional args
