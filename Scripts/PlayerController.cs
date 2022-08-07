namespace MarioLikeGame;

public class PlayerController : KinematicBody2D
{
    private const float _wallGravity = 200;
    private const float _gravity = 200;
    private const int _walkSpeed = 100;
    private const int _jumpForce = -100;
    private bool _jumping;
    private Vector2 _velocity;

    private GameManager _gameManager;
    private LevelManager _levelManager;
    private Vector2 _levelStartPos;

    public void PreInit(GameManager gameManager)
    {
        _gameManager = gameManager;
        _levelManager = gameManager.LevelManager;
    }

    public override void _Ready()
    {
        _levelStartPos = Position;
    }

    public override void _PhysicsProcess(float delta)
    {
        _velocity.x = 0;

        _velocity.y += _gravity * delta;

        // input
        var left = Input.IsActionPressed("player_move_left");
        var right = Input.IsActionPressed("player_move_right");
        var jump = Input.IsActionJustPressed("player_jump");

        if (jump && IsOnFloor())
        {
            _jumping = true;
            _velocity.y = _jumpForce;
        }

        if (right)
            _velocity.x += _walkSpeed;
        if (left)
            _velocity.x -= _walkSpeed;

        if (_jumping && IsOnFloor())
            _jumping = false;

        _velocity = MoveAndSlide(_velocity, new Vector2(0, -1));
    }

    private void _on_Player_Area_area_entered(Area2D area)
    {
        if (area.Name == "Bottom")
        {
            Position = _levelStartPos;
        }

        if (area.Name == "Level Finish")
        {
            _levelManager.CompleteLevel(_levelManager.CurrentLevel);
        }

        if (area.Name == "Enemy")
        {
            Position = _levelStartPos;
        }
    }
}
