### Preamble
I am looking for C# programmers to help peer-review my code and expand on new gameplay mechanics like moving platforms, new enemies and player abilities.

If you need help understanding something, please just ask me on Discord (valk#9904) for help.

**Please talk to me whenever you want to work on something, this way I can tell you if others are working on it or if I do not want you to work on that specific thing right now.**

[![GitHub issues by-label](https://img.shields.io/github/issues/Valks-Games/sankari/coding?color=black)](https://github.com/Valks-Games/sankari/issues?q=is%3Aissue+is%3Aopen+label%3Acoding)

### VSCode
Please use VSCode, using the built in Godot script editor with C# should be a crime.
1. Install [VSCode](https://code.visualstudio.com)
2. Install the following extensions for VSCode
    - [C#](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp)
    - [C# Tools for Godot](https://marketplace.visualstudio.com/items?itemName=neikeq.godot-csharp-vscode)
    - [godot-tools](https://marketplace.visualstudio.com/items?itemName=geequlim.godot-tools)
    - [Mono Debug](https://marketplace.visualstudio.com/items?itemName=ms-vscode.mono-debug)
    - [MoonSharp Debug](https://marketplace.visualstudio.com/items?itemName=xanathar.moonsharp-debug) (only if debugging lua)
3. Launch Godot through VSCode by hitting `F1` to open up VSCode command and run `godot tools: open workspace with godot editor` or simply click the `Open Godot Editor` button bottom right

### Setup GitHub Fork
1. Fork this repo
2. Install [Git scm](https://git-scm.com/downloads)
3. Clone your fork with `git clone https://github.com/<USERNAME>/Sankari` (replace `<USERNAME>` with your GitHub username)
4. Push and pull changes from your fork with `git pull` `git push`
5. Create a pull request through the GitHub website to merge your work with this repo

> ⚠️ Please double check that you are not changing every single line in the project when you commit because you had the wrong line space settings. To see the correct line settings, have a look at the code style document below.

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

# Merge upstream with your fork
git merge upstream/main
```

### Code Style
Please make use of the following [code style](https://github.com/GodotModules/GodotModulesCSharp/blob/main/.github/FORMATTING_GUIDELINES.md).
