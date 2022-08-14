namespace MarioLikeGame;

public class Slime : KinematicBody2D, IEnemy
{
    private const float _gravity = 250f;
    private Vector2 _velocity;
    private bool _jumping;
    private GTimer _jumpTimer;
    private bool _movingForward;
    private int _wallHugTime;

    public void PreInit(Player player)
    {
        
    }

    public override void _Ready()
    {
        _jumpTimer = new GTimer(this, nameof(OnJumpTimer), 2000);
    }

    public override void _PhysicsProcess(float delta)
    {
        _velocity.y += delta * _gravity;

        if (IsOnFloor() && !_jumping)
        {
            _velocity.x = 0;
            _velocity.y = 0;
        }

        if (_jumping)
            _jumping = false;

        if (IsOnWall())
        {
            _wallHugTime++;

            if (_wallHugTime >= 50)
                _movingForward = !_movingForward;
        }

        MoveAndSlide(_velocity, new Vector2(0, -1));
    }

    private void OnJumpTimer()
    {
        _jumping = true;
        _wallHugTime = 0;
        _velocity.x += _movingForward ? 20 : -20;
        _velocity.y -= 150;
    }
}
