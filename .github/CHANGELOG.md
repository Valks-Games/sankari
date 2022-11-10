## v0.1.28
- Updated the [scripting](https://github.com/Valks-Games/sankari/wiki/Scripting) and [multiplayer scripting](https://github.com/Valks-Games/sankari/wiki/Multiplayer-Scripting) sections of the wiki
- Improved readability of scripts

## v0.1.27
- Updated to Godot 4 Beta 4
- Health functions are no longer specific to the player and are now defined in MovingEntity
- Raycasts are no longer specific to the player and are now defined in MovingEntity
- Added the start the frog / slime enemy
- Fix enemies not moving under certain conditions
- Fix player teleport issue
- Several other small changes

## v0.1.26
- Added player health mechanics (https://github.com/Valks-Games/sankari/issues/63)
- ExtensionsInput.FilterRange() does not work right when there is a minimum specified (https://github.com/Valks-Games/sankari/issues/77)
- New method for Notifications.cs -> RemoveAllListeners(Node sender) (https://github.com/Valks-Games/sankari/issues/83)
- Fix not being able to click on a popup menu
- Attempted to improve player movement (still really bad)
- Cleanup up player script a bit
- Fix Godot tweens to use the new Godot 4 tween features fixing numerous bugs
- Fix player crashing game when touching an area with the group name "Bottom"
- Converted several classes to be static (probably not the best idea and should be using a event system instead, see Notifications.cs)
- Fix player death lives left text not being shown (https://github.com/Valks-Games/sankari/issues/145)
- Add convience options for the developer (https://github.com/Valks-Games/sankari/issues/135)
- Fix enemy raycasts preventing enemies from moving

## v0.1.25
- Fixed pressing quit button doing nothing (https://github.com/Valks-Games/sankari/issues/132)
- Figured out how to set the game window title (https://github.com/Valks-Games/sankari/issues/131)
- Fixed Newtonsoft not being detected (https://github.com/Valks-Games/sankari/issues/130)
- Figured out how to get name from individual tiles in a tileset (https://github.com/Valks-Games/sankari/issues/121)
- Converted GTween class for Godot 4 (https://github.com/Valks-Games/sankari/issues/120)
- Converted all scripts to Godot 4
- Figured out how to give names to individual tiles in a tileset (https://github.com/Valks-Games/sankari/issues/119)
- Renamed project to "Sankari" (https://github.com/Valks-Games/sankari/issues/114)
- Added event listeners for player join leave events (https://github.com/Valks-Games/sankari/issues/113)
- Fixed multiple violations of thread safety (https://github.com/Valks-Games/sankari/issues/112 https://github.com/Valks-Games/sankari/issues/115)
- Added more `Send()` methods for the server (https://github.com/Valks-Games/sankari/issues/111)
- Enhance `PrintFull()` method virtually reducing all crashes (https://github.com/Valks-Games/sankari/issues/110) 
- Added console command history (https://github.com/Valks-Games/sankari/issues/109)

## v0.1.24
- loading levels are synced with clients
- changing map position is synced with clients

## v0.1.23
- player list is updated when a player joins / leaves (tested and works for all cases)

## v0.1.22
- there is a new multiplayer menu that can only be accessed from the map scene
- you can now host / join other games (no game netcode has been achieved so far, just connections and netcode UI logic)

## v0.1.21
- fix enemies falling off platform when player goes through bottom of platform with down key
- redid the entire grass tileset making each tile 3 pixels wider in all directions
- redid the tileset bitmask and collisions
- tileset was downgraded from 64x to 16x pixels but uses 4x scale property in game engine
- redid the entire level because of the new tileset
- fix enemies going crazy when player goes through platform
- give unique names to map tileset to be referred in script
- player can now sprint if holding down the dash key while grounded
- fix player not being able to jump on slope sometimes
- speed up walking animation when sprinting
- fix an offset with the area on the coin
- experiment with particles on coins
- moved trigger scripts to their child nodes
- experiment with spawners and custom trigger script
- started working on merging GodotModules CSharp (mostly netcode right now)

## v0.1.20
- player can no longer do Vector2.Zero dashes

## v0.1.19
- implemented checkpoints

## v0.1.18
- decrease player dash cooldown by 250 ms
- implemented chain dashing (player max dash count still set to 1)
- player can no longer dash while grounded
- increase player air speed from 1 to 4
- fix player ghost slide effect
- level 1 is now much easier 

## v0.1.17
- animate player
- increase player friction drastically so it's not like you're on ice
- stop player animation on death
- reduce coin pickup sound volume 

## v0.1.16
- fix bitmask in tilemap
- fix null error in trigger script
- add only execute once option to trigger script
- wait for godot to disable areas
- each orange enemy can have its own unique speed
- added option for orange enemy not to check if it's colliding with a wall
- enemy speed effects frame speed 

## v0.1.15
- show remaining lives animation on death
- camera no longer follows player on death animation
- orange ball enemies no longer fall off cliffs if setting is enabled
- added "triggers", a trigger can be created to effect [any number of specific entities] with a specific [action] when a [entity type] enters a Area2D
- circle platforms have visual radius in editor for level designers
- fix player dash trace not being flipped on wall
- player dash trace is scaled properly now

## v0.1.14
- fixed all player slope physics
- player no longer wall jumps on non-wall jumpable tiles and no longer goes into wall jumping mode when passing through a platform
- enemies are deleted if they fall out of the level
- dash is replenished when touching a climbable wall
- increment coin UI value when touching a coin
- fix basic enemy getting stuck near start of slopes
- tilemap (and all other art) was upscaled to fit 64 x 64 style
- added parallax background to level 1
- redesigned level 1 completely
- scripts no longer use the underscore convention on private variables
- lots of scripts were enhanced / cleaned up
- level bound collisions are dynamically created at runtime based on the camera limits
- lots of file structures were reorganized
- new platform art
- added 4 new tiles (for slopes)
- added "tileset platforms"
- added background tilemaps
- enemies start at random frame just like coins now

## v0.1.13
- figure out how to give enemies different sizes
- redesign tileset and level 1 again
- add spikes

## v0.1.12
- added music to menu 

## v0.1.11
- all sound volumes were adjusted
- added player death animation
- redesigned level 1 with the new tileset
- animated basic enemy (a rolling orange)
- added SFX for player dash / game over

## v0.1.10
- platform that moves in a circular motion
- platform that warns the player with a slow pulse for x seconds then pulses faster for y seconds then disappears then reappears after z seconds

## v0.1.9
- player dashing mechanic was vastly improved
- player can only perform next dash after making contact with the ground

## v0.1.8
- changed namespace to Sankari
- moving platforms that smoothly interpolate between 2 points

## v0.1.7
- adjust volume of music and sfx sounds
- add collisions to new tilemap so player does not continuously fall out of level 1
- player now has z-index of 1 so the player does not render behind trees
- adjust camera left and right bounds to hide the ugly walls containing the map

## v0.1.6
- player can no longer cancel gravity whilst doing a dash whilst going up a wall forever
- player can jump on slopes
- you can no longer spam load a level from the map
- fixed crash if loading a level with no music specified
- dashing shows a faded silhouette of the player for 200ms

## v0.1.5
- player sprite is flipped when touching a wall on the left
- player slope physics improved
- player no longer snaps to the ground after reaching the end of a slope

## v0.1.4
- wall jumping
- moving platform / players velocity is synced with platform 

## v0.1.3
- figured out one way platforms
- dying not only resets player position but loads the entire level again
- levels can have their own unique music tracks
- map has its own music

## v0.1.2
- slime
- turret 

## v0.1.1
- fade transitions
- parallax backgrounds
- player jump sound
- sfx pitch randomization (no 2 sounds with same pitch are played in a row)
- coins
- coin pickup sound
- tweaked viewport / aspect settings
- each level needs a separate configured camera now
- instead of checking the name of the area node, the group its in is checked
- player and enemies are no longer rendered in front of water 

## v0.1
- lots of stuff that is not logged here
- player state machine
