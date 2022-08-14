namespace MarioLikeGame;

public class MovingPlatform : KinematicBody2D
{
    public override void _PhysicsProcess(float delta)
    {
        var pos = Position;
        pos.x += 0.1f;
        Position = pos;
    }
}
