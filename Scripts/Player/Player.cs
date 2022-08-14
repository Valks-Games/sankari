namespace MarioLikeGame;

public class Player : KinematicBody2D
{
    [Export] protected readonly NodePath NodePathWallChecksLeft;
    [Export] protected readonly NodePath NodePathWallChecksRight;

    private const int SPEED_GROUND = 10;
    private const int SPEED_AIR = 1;
    private const int SPEED_MAX_GROUND = 75;
    private const int SPEED_MAX_AIR = 225;
    private const int GRAVITY_AIR = 200;
    private const int GRAVITY_WALL = 750;
    private const int JUMP_FORCE = 100;
    private const int JUMP_FORCE_WALL_VERT = 100;
    private const int JUMP_FORCE_WALL_HORZ = 50;

    private GameManager _gameManager;
    private LevelManager _levelManager;
    private Vector2 _velocity;
    private bool _inputJump;
    private bool _inputDash;
    private bool _inputDown;
    private bool _dashReady = true;
    private bool _canHorzMove = true;
    private Vector2 _levelStartPos;
    private bool _haltPlayerLogic;
    private GTimer _dashTimer;
    private GTimer _preventHorzMovementAfterJump;
    private Node2D _parentWallChecksLeft;
    private Node2D _parentWallChecksRight;
    private List<RayCast2D> _wallChecksLeft = new();
    private List<RayCast2D> _wallChecksRight = new();
    private int _horzMoveDir;
    private int _wallDir;
    private float _gravity = GRAVITY_AIR;

    public void PreInit(GameManager gameManager)
    {
        _gameManager = gameManager;
        _levelManager = gameManager.LevelManager;
    }

    public override void _Ready()
    {
        _levelStartPos = Position;
        _dashTimer = new GTimer(this, nameof(OnDashReady), 1000, false, false);
        _preventHorzMovementAfterJump = new GTimer(this, nameof(OnPreventHorzDone), 200, false, false);
        _parentWallChecksLeft = GetNode<Node2D>(NodePathWallChecksLeft);
        _parentWallChecksRight = GetNode<Node2D>(NodePathWallChecksRight);

        foreach (RayCast2D raycast in _parentWallChecksLeft.GetChildren()) 
        {
            raycast.AddException(this);
            _wallChecksLeft.Add(raycast);
        }

        foreach (RayCast2D raycast in _parentWallChecksRight.GetChildren()) 
        {
            raycast.AddException(this);
            _wallChecksRight.Add(raycast);
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        if (_haltPlayerLogic)
            return;

        UpdateMoveDirection();
        UpdateWallDirection();
        HandleMovement(delta);
    }

    private void HandleMovement(float delta) 
    {
        _inputJump = Input.IsActionJustPressed("player_jump");
        _inputDash = Input.IsActionJustPressed("player_dash");
        _inputDown = Input.IsActionPressed("player_move_down");

        var snap = Vector2.Down * 16;

        if (_wallDir != 0 && IsFalling())
        {
            _velocity.y = 0;
            _gravity = GRAVITY_WALL;

            if (_inputDown)
                _velocity.y += 50;

            if (_inputJump)
            {
                _canHorzMove = false;
                _preventHorzMovementAfterJump.Start();
                _velocity.x += -JUMP_FORCE_WALL_HORZ * _wallDir;
                _velocity.y -= JUMP_FORCE_WALL_VERT;
            }
        }
        
        if (_wallDir == 0)
        {
            _gravity = GRAVITY_AIR;
            
            if (_inputJump && IsOnFloor())
            {
                snap = Vector2.Zero;
                _gameManager.Audio.PlaySFX("player_jump");
                _velocity.y -= JUMP_FORCE;
            }
        }

        if (_wallDir != 0 && IsOnFloor())
        {
            if (_inputJump && IsOnFloor())
            {
                snap = Vector2.Zero;
                _gameManager.Audio.PlaySFX("player_jump");
                _velocity.y -= JUMP_FORCE;
            }
        }

        _velocity.y += _gravity * delta;
        
        if (_canHorzMove)
            if (IsOnFloor())
                _velocity.x += _horzMoveDir * SPEED_GROUND;
            else
                _velocity.x += _horzMoveDir * SPEED_AIR;
            
        _velocity.x = Mathf.Clamp(_velocity.x, -SPEED_MAX_GROUND, SPEED_MAX_GROUND);
        _velocity.y = Mathf.Clamp(_velocity.y, -SPEED_MAX_AIR, SPEED_MAX_AIR);

        if (IsOnFloor())
        {
            if (_velocity.x >= -2 && _velocity.x <= 2)
                _velocity.x = 0;
            else if (_velocity.x > 2)
                _velocity.x -= 5;
            else if (_velocity.x < 2)
                _velocity.x += 5;
        }

        if (_inputDash && _dashReady)
        {
            _dashReady = false;
            _dashTimer.Start();
            _velocity += _velocity * 10;
        }

        _velocity = MoveAndSlideWithSnap(_velocity, snap, new Vector2(0, -1));
    }

    private bool IsFalling() => _velocity.y > 0;

    private void UpdateMoveDirection() =>
        _horzMoveDir = -Convert.ToInt32(Input.IsActionPressed("player_move_left")) + Convert.ToInt32(Input.IsActionPressed("player_move_right"));

    private void UpdateWallDirection() =>
        _wallDir = -Convert.ToInt32(IsTouchingWallLeft()) + Convert.ToInt32(IsTouchingWallRight());

    private bool IsTouchingWallLeft()
    {
        foreach (var raycast in _wallChecksLeft)
            if (raycast.IsColliding()) 
                return true;

        return false;
    }

    private bool IsTouchingWallRight()
    {
        foreach (var raycast in _wallChecksRight)
            if (raycast.IsColliding())
                return true;

        return false;
    }

    public async Task Died()
    {
        _haltPlayerLogic = true;
        await _gameManager.TransitionManager.AlphaToBlackAndBack();
        _haltPlayerLogic = false;
        //Position = _levelStartPos;
        _gameManager.LevelManager.LoadLevel();
    }

    private void OnDashReady() => _dashReady = true;
    private void OnPreventHorzDone() => _canHorzMove = true;

    private async void _on_Player_Area_area_entered(Area2D area)
    {
        if (_haltPlayerLogic)
            return;

        if (area.IsInGroup("Killzone")) 
        {
            await Died();
            return;
        }

        if (area.IsInGroup("Level Finish"))
        {
            _haltPlayerLogic = true;
            await _levelManager.CompleteLevel(_levelManager.CurrentLevel);
            _haltPlayerLogic = false;
            return;
        }

        if (area.IsInGroup("Enemy")) 
        {
            await Died();
            return;
        }

        if (area.IsInGroup("Coin"))
        {
            _gameManager.Audio.PlaySFX("coin_pickup");
            area.GetParent().QueueFree();
        }
    }
}
