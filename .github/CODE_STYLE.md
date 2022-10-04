### Project Specific
- Please always use `Logger.Log()` over `GD.Print()`
- `void Preinit(GameManager gameManager)` should go just above `override void _Ready()`

### Case Format
- Always prefer use of auto properties over readonly keyword and fields
- Class, struct, methods, protected and public variables and properties are `PascalCase` format
- Private fields are `camelCase` format

### Namespaces / usings
- Please make sure all classes have file scoped namespaces
- If a `using` is used across several scripts, consider making it `global` if there are no conflicts

### Readability
- Try to add comments to all the new things you add, not everyone will understand what you did!
- Favor readability over everything else, space out the code, consider using `=>`, and do not use `new()` if it makes the code look cryptic
- Use `var` wherever you can, if something looks too cryptic just add a comment
- Try not to squash every piece of logic into one class, if code is related to a cat put it in a cat class, don't just stuff it all in animals class
- All abstract classes should start with a capital `A`

### VSCode
- Please set `Tab Size` to `4` and `End of Line Sequence` to `CRLF` *(if you do not do this then you may accidentially change every single line in the project making it extremely difficult to find what you changed and solve merge conflicts)*

### The Order of Things
- All `static` members should go to the very top of the class  
- `_Ready()` `_Process()` functions should be at the top, all user-defined functions go below
- All private methods should go below public methods
- All Godot signal methods should go to the very bottom of the class

### GitHub
- Try not to commit all your work in one commit, create commits with meaningful messages describing small things you add here and there. Or at least create a commit for each separate file.

### Comments
- Do not use the words "you", "we", "I". For e.g. instead of "We prioritize input up for vertical dashing", say "Prioritize input up for vertical dashing"

### Code Snippets
Please make use of the following snippets

- `nodepath` -> `"[Export] protected readonly NodePath NodePath"`
- `packedscene` -> `"public readonly static PackedScene "`
