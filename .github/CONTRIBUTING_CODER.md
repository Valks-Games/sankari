> ⚠️ Please talk to valk#9904 through Discord whenever you want to work on something, this way I can tell you if others are working on it or if I do not want you to work on that specific thing right now.

[![GitHub issues by-label](https://img.shields.io/github/issues/Valks-Games/sankari/coding?color=black)](https://github.com/Valks-Games/sankari/issues?q=is%3Aissue+is%3Aopen+label%3Acoding)

[Good First Coding Issues](https://github.com/Valks-Games/sankari/issues?q=is%3Aissue+is%3Aopen+label%3A%22good+first+issue%22+label%3Acoding+)  

## Setup VScode to Work with Godot C#
The built in Godot scripting editor is awful for C# scripting. VSCode is free software, I recommend you use that or JetBrains C# which is not free.

1. Install [VSCode](https://code.visualstudio.com) (or [VSCodium](https://github.com/VSCodium/vscodium) which is VSCode without the built-in telemetry)
2. Required extensions to work with Godot
    - [C#](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp)
    - [C# Tools for Godot](https://marketplace.visualstudio.com/items?itemName=neikeq.godot-csharp-vscode)
    - [godot-tools](https://marketplace.visualstudio.com/items?itemName=geequlim.godot-tools)
    - [Mono Debug](https://marketplace.visualstudio.com/items?itemName=ms-vscode.mono-debug)
    - [Editor Config](https://marketplace.visualstudio.com/items?itemName=EditorConfig.EditorConfig) *(not required but enforces projects indentation settings among other things)*
3. Optional extensions that are useful to have
    - [C# XML Documentation Comments](https://marketplace.visualstudio.com/items?itemName=k--kato.docomment)
    - [Roslynator](https://marketplace.visualstudio.com/items?itemName=josefpihrt-vscode.roslynator)
    - [Tokyo Night Theme](https://marketplace.visualstudio.com/items?itemName=enkia.tokyo-night) *(the theme I always use)*
    - [MoonSharp Debug](https://marketplace.visualstudio.com/items?itemName=xanathar.moonsharp-debug) *(only if debugging lua)*
4. Launch Godot through VSCode by hitting `F1` to open up VSCode command and run `godot tools: open workspace with godot editor` or simply click the `Open Godot Editor` button bottom right popup
5. Set `Godot > Editor > Editor Settings > Mono > Editor > External Editor` to your external editor (for e.g. `Visual Studio Code`)

Note that you can [add these files to exclude from your file view in VSCode](https://gist.github.com/valkyrienyanko/2f9deb179e775650e2d48c7a0e798dec) (it makes the file structure on the left prettier) (I don't know where to paste this in VSCode as I manually edited this directly in the settings, if anyone figures out where please tell me!)

## Debugging
### VSCode Configurations
1. Under `.vscode/` [add launch.json and tasks.json files](https://gist.github.com/valkyrienyanko/45723ed058e175eef2428f7c3230dccb) and replace all instances of `<GODOT_PATH>` with the path to the Godot executable
2. Enable `Wait For Debugger` option in `Godot > Project > Project Settings > Mono > Debugger Agent`
3. Use the `attach` VSCode configuration, set one or more breakpoints and launch Godot game to start debugging
4. You can also use the `Launch 2 sessions` configuration when debugging multiplayer code (this is not the best solution and you may opt for manually launching the game through 2 cmd windows)

### In-Game Console
There is a in-game console you can bring up with `F12`. Type `help` to view a list of all the commands. You can program any of these commands within their respective scripts located under `res://Scripts/UI/Console/Commands`. Feel free to add new commands as well.

### Note About Logging
Please always use `Logger.Log()` over `GD.Print()` as the logger uses a thread safe approach removing the possibility of random game crashes

## Working with Git
### Setup GitHub Fork
1. Fork this repo
2. Install [Git scm](https://git-scm.com/downloads)
3. Clone your fork with `git clone https://github.com/<USERNAME>/Sankari` (replace `<USERNAME>` with your GitHub username)
4. Push and pull changes from your fork with `git pull` `git push`
5. Create a pull request through the GitHub website to merge your work with this repo

### How to Delete Commits From Your Fork
```bash
# Delete all commits except for <last_working_commit_id>
git reset --hard <last_working_commit_id>

# Push the changes (be sure that this is what you really want to do or you may lose a lot of progress)
git push --force
```

### How to Fetch the Latest Updates From This Repo to Your Fork
```bash
# Add upstream as a remote (check remotes with git remote -v)
git remote add upstream https://github.com/Valks-Games/sankari.git

# Fetch data from upstream
git fetch upstream

# Merge upstream with your fork (if you don't care about your history, then replace merge with rebase)
git merge upstream/main
```

## Threads
This game makes use of 3 threads (Godot, Server, Client). Do not directly access public variables or methods from these threads to other threads. If you want to communicate between threads please make use of the appropriate `ConcurrentQueue<T>` channels. Violating thread safety can lead to frequent random game crashes with usually no errors in console making these types of issues extremely hard to track down when they start acting up.

## [Projects Code Style](https://github.com/GodotModules/GodotModulesCSharp/blob/main/.github/FORMATTING_GUIDELINES.md)

## Exporting the Game
> ⚠️ Do not forget to put `enet.dll` beside the games exported executable or all multiplayer functions will not work

> ⚠️ To suppress the rcedit warning download [rcedit](https://github.com/electron/rcedit/releases) and link the executable to `Godot > Editor > Editor Settings > Export > Windows > Rcedit`
