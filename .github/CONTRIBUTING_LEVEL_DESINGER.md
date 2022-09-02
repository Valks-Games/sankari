[![GitHub issues by-label](https://img.shields.io/github/issues/Valks-Games/sankari/level%20designer?color=black&label=Level%20Designer)](https://github.com/Valks-Games/sankari/issues?q=is%3Aissue+is%3Aopen+label%3A%22level+designer%22)

### Assets
All levels are placed in `res://Scenes/Levels` with the naming convention `Level <letter><number>` where letter could be `A` and number could be `1`.

All level specific assets such as coins, enemies, and the player are found in `res://Scenes/Prefabs`.

### Player
Where ever the player is placed is where the level will start. This can be useful to test specific parts of a level by temporarily placing the player in those areas.

### Level Hierarchy

This is an example of what a level hierarchy could look like, this does not have to be followed but it would be nice if everyone agreed upon a universal format.

![3](https://user-images.githubusercontent.com/6277739/188210442-c5ae2c69-c4b6-46ce-94dc-78b17b15acca.png) ![1](https://user-images.githubusercontent.com/6277739/188210002-7e0eb644-e057-4bc8-8af8-f82e8bb253fc.png) ![2](https://user-images.githubusercontent.com/6277739/188210261-ec1fa467-d868-4aae-962f-4e2addb862d4.png)

### Camera
The camera is unique for each level because the "limits" need to be set for each level individually. The yellow border you see below is the camera limits. Note that there is code in the `LevelScene.cs` script that dynamically places colliders at the start of the game so the player can not go outside the level. This is done for the left, right and ceiling of the level. While still on the topic it should be noted that the `LevelScene.cs` script must be attached to the first node in the levels scene. Feel free to change the zoom level of the camera from `0.8` to something else.

![image](https://user-images.githubusercontent.com/6277739/188217154-7b1631a1-68d3-4f5e-bf20-0f8ab1cf7b7f.png)
![image](https://user-images.githubusercontent.com/6277739/188217468-cc1ee705-46f7-41d4-978e-c828e96aaab3.png)

### Tilesets
Make sure the bitmask and colliders are setup in the `TileMap` node.

Multiple `TileMap` nodes can be used to create tiles on different layers as seen below. The tilemap in the background has no colliders setup. Instead a one-way collider was manually placed at the grass top part. You can find this collider in `res://Scenes/Prefabs/Platform/Tileset Platform.tscn`, once placed make sure to right click the shape property in the `CollisionShape2D` node and click `Make Unique`, this way changing the size of this collider will not effect any other collider using this prefab.

![image](https://user-images.githubusercontent.com/6277739/188211331-bfacc803-454a-46da-a2ee-549948d5be67.png)

### Platforms
**Moves Between 2 Points**  

Where ever the platform is placed is where the platforms start position will be and the position on the right is where the platform will move to. Right click the node and click `Editable Children` and modify the position on the right to where you want the platform to move to.

![image](https://user-images.githubusercontent.com/6277739/188214283-9278982b-c61f-4872-8866-7ef09038cd2c.png)

![image](https://user-images.githubusercontent.com/6277739/188214660-55f23f64-b7bd-4ffe-86f7-c412d38fa2eb.png)

**Platform Disappear**

This platform is straight forward to use, the property values are in `ms`.

![image](https://user-images.githubusercontent.com/6277739/188214777-4488a0f5-c8f5-436c-9b01-d18f90b0634b.png)

**Platform Circle**

A platform that moves in a ciruclar motion. Right click the node and click `Editable Children` and modify the radius of the `Radius` node to change how far out the platform moves. (you will see a orange dot on the right side of the circle you can drag when clicking the `Radius` node)

![image](https://user-images.githubusercontent.com/6277739/188215183-9d17a5aa-6359-4ebc-a0c8-82291bba6e2c.png)

![image](https://user-images.githubusercontent.com/6277739/188215303-87ee11d6-7f35-4333-949e-c5dca28b1dfb.png)

### Groups
`WallJumpArea`  
If the player is in a area with this group, they can wall jump on any tileset edge.

`Tileset`  
If a tileset does not have this group then enemies will not know if their colliding with a wall.

`Platform`  
If a tileset platform does not have this group then the player will not be able to drop down from a platfrom with the down key.

`Level Finish`  
If a player touches a area with this group then they will clear the level.

`Bottom`  
If a player or a enemy touches a area with this group they will die. This is to prevent entities from falling endlessely off the bottom of the level. The bottom area should be put down enough so for example large enemies are not instantly deleted when they touch this area as the player will see this.

An example of what the bottom area could look like can be see below marked by the large blue rect underneath the level. See how the area covers the entire width of the level.

![image](https://user-images.githubusercontent.com/6277739/188217709-4a979579-f177-4a3e-b820-213f085d923a.png)

### Enemies
**Basic Enemy**  

These orange balls are *super intelligent*, if told, they can check if their colliding with a wall or if their about go off a cliff. If you beat the example level, you'll know the 3 enemies below have varing speeds.

![image](https://user-images.githubusercontent.com/6277739/188215652-9311b33b-ae05-46aa-8031-882f14797c2a.png)

![image](https://user-images.githubusercontent.com/6277739/188215613-e2bcf8ad-a405-4553-a7c2-46d530e22ccb.png)

To modify the size of an enemy simply modify the scale property of that enemy.

![image](https://user-images.githubusercontent.com/6277739/188217977-63004e7a-59b9-460d-93a9-ded3c6949f2f.png)

### Triggers
Triggers can be setup to perform a specific [action] on one or more [entities] when a specific [entity type] enters a area.

Below you can see a trigger marked by the big blue rect on the left. The 3 enemies of varying sizes at the bottom right are activated when the player enters this trigger area.

![image](https://user-images.githubusercontent.com/6277739/188212774-c6e94076-891b-4ee5-ae13-8beed247ea0c.png)

Here you can see the settings of this trigger.

![image](https://user-images.githubusercontent.com/6277739/188213062-ca109320-ca13-438e-a5fe-f78e8558043b.png)

### Parallax Background  

The parallax background is the art you see in the distance when playing the game. Each layer can have its own horizontal speed. If changing the position of the background, the position of each individual `Sprite` node must be changed otherwise if other values are changed for example in the `ParallaxBackground` node then the position of the background in the editor will not match up with the position in the game. To change the scroll speed of each layer, modify the x value of the scale property in each `ParallaxLayer` node. Values should range from `0` to `1.0`, anything faster than `1.0` would be too fast. Note that parallax backgrounds can also be put in front of the games art (tilesets / player), to achieve this just edit the Z-Index of the background to be `2` or higher.

![image](https://user-images.githubusercontent.com/6277739/188216170-6afc81ab-e40e-41bc-8df1-be71dcaa9ae1.png)
![image](https://user-images.githubusercontent.com/6277739/188216240-092f3ab8-8a0a-463a-aa02-5b2aea0cd7c0.png)


