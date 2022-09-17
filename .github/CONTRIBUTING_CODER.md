> ⚠️ Please talk to valk#9904 through Discord whenever you want to work on something, this way I can tell you if others are working on it or if I do not want you to work on that specific thing right now.

> ℹ️ If you comment on an issue in GitHub I can assign you to that issue. 

[![GitHub issues by-label](https://img.shields.io/github/issues/Valks-Games/sankari/coding?color=black)](https://github.com/Valks-Games/sankari/issues?q=is%3Aissue+is%3Aopen+label%3Acoding)

## Setup VS2022 to work with Godot C#
1. Make sure you have the latest installation of VS2022
2. Set `Godot > Editor > Editor Settings > Mono > Editor > External Editor` to `Visual Studio`
3. To get debugging working with Godot, create a new folder next to `project.godot` called `Properties` and within that folder create a new JSON file with the following contents

```json
{
  "profiles": {
    "Profile 1": {
      "commandName": "Executable",
      "executablePath": "<REPLACE_WITH_PATH_TO_YOUR_GODOT_EXECUTABLE>",
      "commandLineArgs": "--path .",
      "workingDirectory": ".",
      "nativeDebugging": true
    }
  }
}
```

There is also a in-game console you can bring up with `F12`. Type `help` to view a list of all the commands. You can program any of these commands within their respective scripts located under `res://Scripts/UI/Console/Commands`.

Please always use `Logger.Log()` over `GD.Print()` as the logger uses a thread safe approach removing the possibility of random game crashes

Try making use of `Print()` and `PrintFull()`. For example `Logger.Log(myArray.Print())` and `Logger.Log(this.PrintFull())`, try it out!

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

> ℹ️ To suppress the rcedit warning download [rcedit](https://github.com/electron/rcedit/releases) and link the executable to `Godot > Editor > Editor Settings > Export > Windows > Rcedit`
