[![GitHub issues by-label](https://img.shields.io/github/issues/Valks-Games/sankari/art?color=black)](https://github.com/Valks-Games/sankari/issues?q=is%3Aissue+is%3Aopen+label%3Aart)  

### Tilesets
- Most of the tilesets should be 16 x 16 (some can be 8 x 8 depending on what it is you are trying to do)
- The example usage of the tileset should be put into a separate png
- Tiles should be packed together without any gaps between them starting from the top left
- If you have the tiles for it, sort the tiles so they work well with Godots autotile system (see Figure 1.1)
- Make sure the png size is a divisor of the tiles dimensions
- The png's width and height do not need to be the same, if you have a lot of empty space then crop the image to the content
- Once the tiles are positioned in the tileset and given to the level designer, do not change the position of the tiles otherwise this may cause 1) missing tiles and 2) swapped tiles in Godot making the level designer having to redo the entire level(s) that used that tileset
- Platforms the player can stand on should go into their own individual pngs
- If there is more than one variation of a tile, organize them in a horizontal or vertical strip (see Figure 1.2)

Figure 1.1 

![image](https://user-images.githubusercontent.com/6277739/187054011-2bc3672c-70c5-4bc3-afc1-a8e88fde434c.png)

Figure 1.2

![image](https://user-images.githubusercontent.com/6277739/187090161-712dee94-bed2-4ad6-a60e-dab32734dcd3.png)

### Animated Tiles
Animated tiles must have each frame in their own individual pngs. For example imagine a animated water tile, the file names should go like `water_0.png`, `water_1.png`, `water_2.png`, `water_3.png` etc.

### Character Animations
Each character animation must be in its own png in a horizontal strip. For an example have a look at the coins in Figure 2.1. Do not mix animations together, for example if there is a player walking and idle animation, do not put them in the same png on top of each other.

Figure 2.1

![Full Coins](https://user-images.githubusercontent.com/6277739/187054154-977638e4-4844-4df0-851a-f2c0b0b5f960.png)
