## Preamble
I've decided to make a 2D platformer taking inspiration from Super Mario World (SNES). I just started working on this, I have a artist helping me, plus a lot of art is from opengameart.org. So far I've got a placeholder for a top-down view map and the logic for transitioning between different levels / blocking the players progression when appropriate. I have some basic enemies that go back and fourth, if you touch them you go back to the start of the level, likewise same thing happens if you fall out of the level. I want to make the game more interesting by expanding on the players mechanics (wall jumping / sliding), and adding moving platforms, and more types of enemies. The game does not have a good name, as it's just called "Mario Like Game" right now.

The roadmap can be found in the [Discord for the game here](https://discord.gg/5frafxrwwd)

## Contributing
The project is and will remain open source and free-to-play. The game is using Godot 3.5 mono C# and if multiplayer is implemented, ENet-CSharp will be used. I'm also going to be using snippets of code from my previous project "Godot Modules" and will actively be working on this in parallel.

If you would like to contribute as a artist, musician or coder, give me a DM on Discord (valk#9904) and I'll send you the projects current roadmap and tell you more about the project.

### Code
Please have a look at [formatting guidelines](https://github.com/GodotModules/GodotModulesCSharp/blob/main/.github/FORMATTING_GUIDELINES.md) and read the setup guide for setting up Godot, VSCode and working with GitHub [here](https://github.com/GodotModules/GodotModulesCSharp/blob/main/.github/CONTRIBUTING.md) *(but ignore the part about the dev branch as this is not the case for this project)*

### Art
All level tilemaps are 8x8 and the map is a 16x16 tilemap. 

Please cram all tiles in a nice grid like layout starting at the top left of the image.
![image](https://user-images.githubusercontent.com/6277739/183508968-b2f67d5d-7df6-4295-bec0-61b8e72ee718.png)

For animated tiles, save each frame in a separate file (`water_0.png`, `water_1.png`, `water_2.png`, ...)
![image](https://user-images.githubusercontent.com/6277739/183509095-3d6e3dce-7abe-4e8a-ac4d-cf1f3a32b330.png)

https://user-images.githubusercontent.com/6277739/183507538-a311b3b4-c4c1-48df-9c74-e450780ad9bc.mp4
