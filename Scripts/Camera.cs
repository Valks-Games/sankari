namespace MarioLikeGame;

public class Camera : Camera2D
{
    private PlayerStateManager _player;

    public override void _Ready()
    {
        _player = GetNode<PlayerStateManager>("../Player");
    }

    public override void _Process(float delta)
    {
        Position = _player.Position;
    }
}
