[![GitHub issues by-label](https://img.shields.io/github/issues/Valks-Games/sankari/art?color=black)](https://github.com/Valks-Games/sankari/issues?q=is%3Aissue+is%3Aopen+label%3Aart)  

**Only accepting art uploaded to [Open Game Art](https://opengameart.org/) with the [CC0 license](https://creativecommons.org/publicdomain/zero/1.0/)**

### Tilesets
To get an idea of what the tileset looks like in Godot have a look at Figure 1.1

Rules to follow when making tilesets
- Most of the tilesets should be 64 x 64 (some can be different sizes depending on what it is you are trying to do)
- The example usage of the tileset should be put into a separate png
- Tiles should be packed together without any gaps between them starting from the top left
- Make sure the png size is a divisor of the tiles dimensions
- The png's width and height do not need to be the same, if you have a lot of empty space then crop the image to the content
- Once the tiles are positioned in the tileset and given to the level designer, do not change the position of the tiles otherwise this may cause 1) missing tiles and 2) swapped tiles in Godot making the level designer having to redo the entire level(s) that used that tileset
- Platforms the player can stand on should go into their own individual pngs
- If there is more than one variation of a tile, organize them in a horizontal or vertical strip (see Figure 1.2)

Figure 1.1 

![image](https://user-images.githubusercontent.com/6277739/187558865-557f4f93-24e9-47fa-bb94-6541b1c82a86.png)  
*Cyan = Auto tile*  
*Yellow = Single tile*  

Figure 1.2

![image](https://user-images.githubusercontent.com/6277739/187090161-712dee94-bed2-4ad6-a60e-dab32734dcd3.png)

### Animated Tiles
Animated tiles must have each frame in their own individual pngs. For example imagine a animated water tile, the file names should go like `water_0.png`, `water_1.png`, `water_2.png`, `water_3.png` etc.

### Character Animations
Each character animation must be in its own png in a horizontal strip. For an example have a look at the coins in Figure 2.1. Do not mix animations together, for example if there is a player walking and idle animation, do not put them in the same png on top of each other.

Figure 2.1

![Full Coins](https://user-images.githubusercontent.com/6277739/187054154-977638e4-4844-4df0-851a-f2c0b0b5f960.png)

### Parallax Backgrounds
- All parallax backgrounds should be 3840 x 2160 pixels in size
- To change the position of the parallax layers you must only modify the transform of the `Sprite` node (see Figure 3.1)
- To modify the horizontal scroll speed, modify the x scale component of a `ParallaxLayer` node

Figure 3.1

![image](https://user-images.githubusercontent.com/6277739/187559367-83cf86ec-b51f-4b94-a41f-1b13f2580886.png)
