namespace Sankari;

public class Camera : Camera2D
{
    private Player player;

    public override void _Ready()
    {
        player = GetNode<Player>("../Player");
    }

    public override void _Process(float delta)
    {
        Position = player.Position;
    }

    public void StopFollowingPlayer() => SetProcess(false);
    public void StartFollowingPlayer() => SetProcess(true);
}
