> [!NOTE]
> This has been archived because it is out of sync with [CSharpGodotTools/Template](https://github.com/CSharpGodotTools/Template) and the code is arguably messy and should be redone from scratch.

<p align="center">
  <img alt="Discord" src="https://img.shields.io/discord/1005979449340211240?color=black&label=Sankari&logo=Discord&logoColor=white">
</p>

<h4 align="center">
  <a href="https://youtu.be/QddaW1QEVYc">Video of Gameplay</a>
</h4>

https://user-images.githubusercontent.com/6277739/199924817-a5d3992f-edd0-444c-be70-0dbe7eb977ca.mp4

https://user-images.githubusercontent.com/6277739/199924992-c549865c-5e34-45fe-9674-bdb8bea82434.mp4

https://user-images.githubusercontent.com/6277739/200099886-5068418b-e8fe-4ce9-b1cd-050bf08eef70.mp4

## What is Sankari?
Sankari ("hero" in Finnish) is a non-profit F2P 2D Platformer about a hero whose village gets raided by the evil king. The hero goes on a journey to free back the villagers that were captured and defeat the evil king. 

This project was created to gain experience in the C# Godot environment for the 2D Platform genre and to prove to myself that I can make a complete game. I hope in making this open source that others will also learn some things that I have learned.

The project will be considered complete when there is around 2 to 3 hours of fun and memorable content. Some of the major goals of the project include getting multiplayer to a playable state and making the code as flexible and as readable as possible.

If you have any questions or suggestions about the project or just want to talk to me in general, my Discord username is `va#9904`. You can also find me on the [Sankari Discord](https://discord.gg/5frafxrwwd).

[Changelog](https://github.com/Valks-Games/sankari/blob/main/.github/CHANGELOG.md)  

## Roadmap
- Use Godot scenes instead of one big master node handling all scene nodes. This will allow for easier testing of inidivual scenes in the future.
- Wrap my head around the current codebase. Make the code look nicer. Make it less confusing to the reader.
- Figure out why "Load First Level" option is not working as intended. (First level is loaded but map screen is hiding the level)
- Tinker with the player movement more to get that more snappier platformer like feel.
- Introduce new water tiles.

## Project Setup
1. Install the latest Godot 4 C# release
2. Install [.NET SDK 6.0](https://dotnet.microsoft.com/en-us/download)
3. Install [.NET Framework 4.7.2](https://duckduckgo.com/?q=.net+framework+4.7.2)
4. In `Godot Editor > Editor Settings > Mono > Builds`: Make sure `Build Tool` is set to `dotnet CLI`

## Contributing
### [I am a coder](https://github.com/Valks-Games/sankari/wiki/Scripting)
### [I am a artist](https://github.com/Valks-Games/sankari/wiki/Creating-Art)
### [I am a level designer](https://github.com/Valks-Games/sankari/wiki/Level-Designing)
### [I am a musician](https://github.com/Valks-Games/sankari/wiki/Creating-Audio)

## License
### Project
This project is under the [MIT license](https://github.com/Valks-Games/sankari/blob/main/LICENSE)

### Assets
- Most assets (art / audio) are under the [CC0 license](https://creativecommons.org/publicdomain/zero/1.0/) from [Open Game Art](https://opengameart.org/)
- Some of the audio is from [Mixkit](https://mixkit.co/free-sound-effects/game-over/) under the Mixkit license
- The networking library [ENet-CSharp](https://github.com/SoftwareGuy/ENet-CSharp) is under the [MIT license](https://github.com/SoftwareGuy/ENet-CSharp/blob/master/LICENSE)

## Credit
Thank you to the following wonderful people that helped make this project something even better

[AdamLaine](https://github.com/AdamLaine)  
[Dunkhan](https://github.com/Dunkhan)  
[Muftwin](https://github.com/Muftwin)  
[Policiu](https://github.com/policiu)  
