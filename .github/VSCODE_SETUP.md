## VSCode
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
5. Set `Godot > Editor > Editor Settings > Mono > Editor > External Editor` to `Visual Studio Code`

Note that you can [add these files to exclude from your file view in VSCode](https://gist.github.com/valkyrienyanko/2f9deb179e775650e2d48c7a0e798dec) (it makes the file structure on the left prettier) (I don't know where to paste this in VSCode as I manually edited this directly in the settings, if anyone figures out where please tell me!)

### VSCode Configurations
1. Under `.vscode/` [add launch.json and tasks.json files](https://gist.github.com/valkyrienyanko/45723ed058e175eef2428f7c3230dccb) and replace all instances of `<GODOT_PATH>` with the path to the Godot executable
2. Enable `Wait For Debugger` option in `Godot > Project > Project Settings > Mono > Debugger Agent`
3. Use the `attach` VSCode configuration, set one or more breakpoints and launch Godot game to start debugging
