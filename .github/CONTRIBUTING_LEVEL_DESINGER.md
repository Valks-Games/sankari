[![GitHub issues by-label](https://img.shields.io/github/issues/Valks-Games/sankari/level%20designer?color=black&label=Level%20Designer)](https://github.com/Valks-Games/sankari/issues?q=is%3Aissue+is%3Aopen+label%3A%22level+designer%22)

Not much to see here right now, come back later!

### Level Hierarchy

This is an example of what a level hierarchy could look like, this does not have to be followed but it would be nice if everyone agreed upon a universal format.

![3](https://user-images.githubusercontent.com/6277739/188210442-c5ae2c69-c4b6-46ce-94dc-78b17b15acca.png) ![1](https://user-images.githubusercontent.com/6277739/188210002-7e0eb644-e057-4bc8-8af8-f82e8bb253fc.png) ![2](https://user-images.githubusercontent.com/6277739/188210261-ec1fa467-d868-4aae-962f-4e2addb862d4.png)

### Tilesets
Make sure the bitmask and colliders are setup in the `TileMap` node.

Multiple `TileMap` nodes can be used to create tiles on different layers as seen below. The tilemap in the background has no colliders setup. Instead a one-way collider was manually placed at the grass top part. You can find this collider in `res://Scenes/Prefabs/Platform/Tileset Platform.tscn`, once placed make sure to right click the shape property in the `CollisionShape2D` node and click `Make Unique`, this way changing the size of this collider will not effect any other collider using this prefab.

![image](https://user-images.githubusercontent.com/6277739/188211331-bfacc803-454a-46da-a2ee-549948d5be67.png)

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


