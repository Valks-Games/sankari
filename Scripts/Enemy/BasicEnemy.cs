namespace Sankari;

public class BasicEnemy : KinematicBody2D, IEnemy
{
    private const float _gravity = 6000f;
    private bool _movingForward;

    public void PreInit(Player player)
    {

    }

    public override void _PhysicsProcess(float delta)
    {
        var velocity = new Vector2(0, 0);
        velocity.y += delta * _gravity;

        if (IsOnWall())
            _movingForward = !_movingForward;

        if (_movingForward)
            velocity.x += 10;
        else
            velocity.x -= 10;

        MoveAndSlide(velocity, new Vector2(0, -1));
    }
}
