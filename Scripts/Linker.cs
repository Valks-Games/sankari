namespace Sankari;

public class Linker : Node
{
    [Export] public readonly NodePath NodePathTransition;
    
    public override void _Ready()
    {
        new GameManager(this);
    }

    public override void _Process(float delta)
    {
        Logger.Update();
    }
}
