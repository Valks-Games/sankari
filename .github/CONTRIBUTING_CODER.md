This project is using the latest stable version of [Godot Mono (C#)](https://godotengine.org/download)  

> ⚠️ Please talk to valk#9904 through Discord whenever you want to work on something, this way I can tell you if others are working on it or if I do not want you to work on that specific thing right now.

[![GitHub issues by-label](https://img.shields.io/github/issues/Valks-Games/sankari/coding?color=black)](https://github.com/Valks-Games/sankari/issues?q=is%3Aissue+is%3Aopen+label%3Acoding)

### Debugging
Attaching vscode debugger to Godot can be done when debugging singleplayer code, however when debugging multiplayer code this can prove to be difficult because the server / client threads constantly need to be running and if their interrupted by breakpoints for more than a set duration then the server and client will timeout.

**How to attach vscode debugger to Godot**  
> ⚠️ I recently tried to create a `launch.json` file using the steps described below but it only generated a json with version and empty configurations array. Something seems wrong here.

In vscode if not done so already, click `create a launch.json file` button and click for `C# Godot` in the debugger tab. If you do not see the `C# Godot` option then you do not have all the Godot extensions installed for VScode. Please refer to the setup guide for VSCode below if this is the case. Next, within Godot go to `Project > Project Settings > Mono > Debugger Agent` and enable `Wait For Debugger` option. Then attach VSCode debugger to Godot, set one or more breakpoints and launch Godot game to start debugging.

**How to debug multiplayer code**  
You can still use vscode debugger to debug your code, just note that you will only be able to debug one breakpoint then you will need to restart the entire game as the server and client will timeout.

Please always use `Logger.Log()` over `GD.Print()` as the logger uses a thread safe approach removing the possibility of game crashes due to console text conflicts / overflow.

### Threads
This game makes use of 3 threads (Godot, Server, Client). Do not directly access public variables or methods from these threads to other threads. If you want to communicate between threads please make use of the appropriate `ConcurrentQueue<T>` channels. Violating thread safety can lead to frequent random game crashes with usually no errors in console making these types of issues extremely hard to track down when they start acting up.

### [Setup VSCode to work with Godot C#](https://github.com/Valks-Games/sankari/blob/main/.github/SETUP_VSCODE.md)

### [Useful Git Workflow Tips](https://github.com/Valks-Games/sankari/blob/main/.github/SETUP_GITHUB_FORK.md)

### [Projects Code Style](https://github.com/GodotModules/GodotModulesCSharp/blob/main/.github/FORMATTING_GUIDELINES.md)

### Exporting the Game
> ⚠️ Do not forget to put `enet.dll` beside the games exported executable or all multiplayer functions will not work

> ⚠️ To suppress the rcedit warning download [rcedit](https://github.com/electron/rcedit/releases) and link the executable to `Godot > Editor > Editor Settings > Export > Windows > Rcedit`
