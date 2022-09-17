## Differences

|                 |            Unity                                            |   Godot       |
| ----------------|------------------------------------------------------------ | ------------- |
| License         | Proprietary, closed, free license with revenue caps and usage restrictions  | MIT license, free and fully open source without any restriction  |
| Scripting        | <ul><li>`[SerializeField]`</li><li>`GetComponent<T>`</li><li>`Start()` `Update()` `FixedUpdate()`</li></ul> | <ul><li>`[Export]`</li><li>`GetNode<T>`</li><li>`_Ready()` `_Process()` `_PhysicsProcess()`</li></ul> |
| Prefabs          | Convert to a prefab. | Everything in Godot is a Node. Players, prefabs, scripts and scenes are nodes. |
| C#               | Primary Support for C# | Secondary Support for C# (can not use engine script profiling tools) |
| Notable Features | Shader graph looks and feels really nice | <ul><li>Extremely lightweight</li><li>Creating UI is very flexible</li></ul> |
| Missing Features | Networking stack documentation is awful and working with it is very unfriendly to the coder | <ul><li>Inverse Kinematics are broken</li><li>PathFollow2D has no support for `sync_physics`</li><li>No support for compute shaders in Godot 3.x</li></ul> |
