**Logger**

`Logger.Log(object message, ConsoleColor color = ConsoleColor.Gray)` log a message

`Logger.LogWarning(object message, ConsoleColor color = ConsoleColor.Yellow)` log a warning

`Logger.LogTodo(object message, ConsoleColor color = ConsoleColor.White)` log a todo

`Logger.LogErr(Exception e, string hint = default, ConsoleColor color = ConsoleColor.Red, [CallerFilePath] string filePath = default, [CallerLineNumber] int lineNumber = 0)` log a error

**Audio**  

`Audio.LoadSFX(string name, string path)` load a sound effect, for e.g. `LoadSFX("player_jump", "Movement/Jump/sfx_movement_jump1.wav")`

`Audio.LoadMusic(string name, string path)` load a music track, for e.g. `LoadMusic("ice_1", "Level/Ice/Icy_Expanse.mp3")`

`Audio.PlayMusic(string name, float pitch = 1)` play a music track (pitch values between 0.1 and 1 are slower, values higher than 1.0 are faster)

`Audio.PlaySFX(string name, int volume = 100)` play a sound (volume ranges from 0 to 100)  

`Audio.SetSFXVolume(int v)` set the SFX volume (values range from 0 to 100)

`Audio.StopMusic()` stop the music track currently being played
