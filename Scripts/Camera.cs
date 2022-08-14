namespace MarioLikeGame;

public class Camera : Camera2D
{
    private Player _player;

    public override void _Ready()
    {
        _player = GetNode<Player>("../Player");
    }

    public override void _Process(float delta)
    {
        Position = _player.Position;
    }
}
