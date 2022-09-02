[![GitHub issues by-label](https://img.shields.io/github/issues/Valks-Games/sankari/level%20designer?color=black&label=Level%20Designer)](https://github.com/Valks-Games/sankari/issues?q=is%3Aissue+is%3Aopen+label%3A%22level+designer%22)

### Assets
All levels are placed in `res://Scenes/Levels` with the naming convention `Level <letter><number>` where letter could be `A` and number could be `1`.

All level specific assets such as coins, enemies, and the player are found in `res://Scenes/Prefabs`.

### Player
Where ever the player is placed is where the level will start. This can be useful to test specific parts of a level by temporarily placing the player in those areas.

### Level Hierarchy

This is an example of what a level hierarchy could look like, this does not have to be followed but it would be nice if everyone agreed upon a universal format.

![3](https://user-images.githubusercontent.com/6277739/188210442-c5ae2c69-c4b6-46ce-94dc-78b17b15acca.png) ![1](https://user-images.githubusercontent.com/6277739/188210002-7e0eb644-e057-4bc8-8af8-f82e8bb253fc.png) ![2](https://user-images.githubusercontent.com/6277739/188210261-ec1fa467-d868-4aae-962f-4e2addb862d4.png)

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

### Triggers
Triggers can be setup to perform a specific [action] on one or more [entities] when a specific [entity type] enters a area.

Below you can see a trigger marked by the big blue rect on the left. The 3 enemies of varying sizes at the bottom right are activated when the player enters this trigger area.

![image](https://user-images.githubusercontent.com/6277739/188212774-c6e94076-891b-4ee5-ae13-8beed247ea0c.png)

Here you can see the settings of this trigger.

![image](https://user-images.githubusercontent.com/6277739/188213062-ca109320-ca13-438e-a5fe-f78e8558043b.png)


