namespace Sankari;

public class Player : KinematicBody2D
{
    [Export] protected readonly NodePath NodePathRayCast2DWallChecksLeft;
    [Export] protected readonly NodePath NodePathRayCast2DWallChecksRight;
    [Export] protected readonly NodePath NodePathRayCast2DFloorCheck;
    [Export] protected readonly NodePath NodePathRayCast2DSlopeCheck;
    [Export] protected readonly NodePath NodePathSprite;

    private const int SPEED_GROUND = 10;
    private const int SPEED_AIR = 1;
    private const int SPEED_MAX_GROUND = 75;
    private const int SPEED_MAX_AIR = 225;
    private const int SPEED_DASH_VERTICAL = 100;
    private const int SPEED_DASH_HORIZONTAL = 150;
    private const int GRAVITY_AIR = 350;
    private const int GRAVITY_WALL = 750;
    private const int JUMP_FORCE = 150;
    private const int JUMP_FORCE_WALL_VERT = 150;
    private const int JUMP_FORCE_WALL_HORZ = 75;
    private const int DASH_COOLDOWN = 500;
    private const int DASH_DURATION = 200;
    private const int PREVENT_HORZ_MOVEMENT_AFTER_WALL_JUMP_DURATION = 200;

    private GameManager _gameManager;
    private LevelManager _levelManager;
    private Vector2 _velocity;
    private bool _inputJump;
    private bool _inputDash;
    private bool _inputDown;
    private bool _dashReady = true;
    private bool _currentlyDashing;
    private bool _canHorzMove = true;
    private Vector2 _levelStartPos;
    private bool _haltPlayerLogic;
    private GTimer _timerDashCooldown;
    private GTimer _timerDashDuration;
    private GTimer _preventHorzMovementAfterJump;
    private Node2D _parentWallChecksLeft;
    private Node2D _parentWallChecksRight;
    private List<RayCast2D> _rayCast2DWallChecksLeft = new();
    private List<RayCast2D> _rayCast2DWallChecksRight = new();
    private RayCast2D _rayCast2DFloorCheck;
    private RayCast2D _rayCast2DSlopeCheck;
    private Vector2 _moveDir;
    private int _wallDir;
    private float _gravity = GRAVITY_AIR;
    private Sprite _sprite;
    private GTween _dieTween;

    public void PreInit(GameManager gameManager)
    {
        _gameManager = gameManager;
        _levelManager = gameManager.LevelManager;
    }

    public override void _Ready()
    {
        _levelStartPos = Position;
        _timerDashCooldown = new GTimer(this, nameof(OnDashReady), DASH_COOLDOWN, false, false);
        _timerDashDuration = new GTimer(this, nameof(OnDashDurationDone), DASH_DURATION, false, false);
        _preventHorzMovementAfterJump = new GTimer(this, nameof(OnPreventHorzDone), PREVENT_HORZ_MOVEMENT_AFTER_WALL_JUMP_DURATION, false, false);
        _rayCast2DFloorCheck = GetNode<RayCast2D>(NodePathRayCast2DFloorCheck);
        _rayCast2DFloorCheck.AddException(this);
        _rayCast2DSlopeCheck = GetNode<RayCast2D>(NodePathRayCast2DSlopeCheck);
        _rayCast2DSlopeCheck.AddException(this);
        _parentWallChecksLeft = GetNode<Node2D>(NodePathRayCast2DWallChecksLeft);
        _parentWallChecksRight = GetNode<Node2D>(NodePathRayCast2DWallChecksRight);
        _sprite = GetNode<Sprite>(NodePathSprite);
        _dieTween = new GTween(this);

        foreach (RayCast2D raycast in _parentWallChecksLeft.GetChildren())
        {
            raycast.AddException(this);
            _rayCast2DWallChecksLeft.Add(raycast);
        }

        foreach (RayCast2D raycast in _parentWallChecksRight.GetChildren())
        {
            raycast.AddException(this);
            _rayCast2DWallChecksRight.Add(raycast);
        }
    }

    private float _dieStartPos;

    public override void _PhysicsProcess(float delta)
    {
        if (_haltPlayerLogic) 
        {
            return;
        }

        UpdateMoveDirection();
        UpdateWallDirection();
        HandleMovement(delta);
    }

    private Vector2 _dashDir;
    private bool _horizontalDash;
    private bool _hasTouchedGroundAfterDash = true;

    private void HandleMovement(float delta)
    {
        _inputJump = Input.IsActionJustPressed("player_jump");
        _inputDash = Input.IsActionJustPressed("player_dash");
        _inputDown = Input.IsActionPressed("player_move_down");

        // on a wall and falling
        if (_wallDir != 0)
        {
            if (IsFalling())
            {
                _velocity.y = 0;
                _gravity = GRAVITY_WALL;

                if (_inputDown)
                    _velocity.y += 50;

                // wall jump
                if (_inputJump)
                {
                    _canHorzMove = false;
                    _preventHorzMovementAfterJump.Start();
                    _velocity.x += -JUMP_FORCE_WALL_HORZ * _wallDir;
                    _velocity.y -= JUMP_FORCE_WALL_VERT;
                }
            }
        }

        if (_inputJump && IsOnFloor())
            Jump();

        // Platform Check
        PlatformDownCheck();

        // dash
        if (_inputDash && _dashReady && _hasTouchedGroundAfterDash && !_currentlyDashing)
        {
            HandleDash();
        }

        if (_currentlyDashing)
        {
            DoDashStuff();
        }
        else
            _gravity = GRAVITY_AIR;

        // apply gravity
        _velocity.y += _gravity * delta;

        if (_canHorzMove)
            if (IsOnGround())
            {
                _hasTouchedGroundAfterDash = true;
                _velocity.x += _moveDir.x * SPEED_GROUND;
                HorzDampening(5, 2);
            }
            else
            {
                _velocity.x += _moveDir.x * SPEED_AIR;
            }

        if (!_currentlyDashing)
        {
            _velocity.x = Mathf.Clamp(_velocity.x, -SPEED_MAX_GROUND, SPEED_MAX_GROUND);
            _velocity.y = Mathf.Clamp(_velocity.y, -SPEED_MAX_AIR, SPEED_MAX_AIR);
        }

        _velocity = MoveAndSlide(_velocity, Vector2.Up);
    }

    private void PlatformDownCheck()
    {
        var collision = _rayCast2DFloorCheck.GetCollider(); // seems like were getting this twice, this could be optimized to only be got once in _Ready and made into a private variable

        if (collision != null) 
        {
            var node = (Node)collision;

            if (node.IsInGroup("Platform") && _inputDown)
            {
                var platform = (APlatform)node;
                platform.TemporarilyDisablePlatform();
            }
        }
    }

    private void DoDashStuff()
    {
        var sprite = Prefabs.PlayerDashTrace.Instance<Sprite>();
        sprite.GlobalPosition = GlobalPosition;
        GetTree().Root.AddChild(sprite);

        var dashSpeed = SPEED_DASH_VERTICAL;

        if (_horizontalDash)
            dashSpeed = SPEED_DASH_HORIZONTAL;

        _velocity = _dashDir * dashSpeed;
        _gravity = 0;
        _hasTouchedGroundAfterDash = false;
    }

    private void HandleDash()
    {
        _dashReady = false;
        _currentlyDashing = true;
        _timerDashDuration.Start();
        _timerDashCooldown.Start();

        // determine dash direction
        var input_up = Input.IsActionPressed("player_move_up");
        if (input_up && _moveDir.x < 0)
        {
            _horizontalDash = false;
            _dashDir = new Vector2(-1, -1);
        }
        else if (input_up && _moveDir.x > 0)
        {
            _horizontalDash = false;
            _dashDir = new Vector2(1, -1);
        }
        else if (input_up)
        {
            _horizontalDash = false;
            _dashDir = new Vector2(0, -1);
        }
        else if (_moveDir.x < 0)
        {
            _horizontalDash = true;
            _dashDir = new Vector2(-1, 0);
        }
        else if (_moveDir.x > 0)
        {
            _horizontalDash = true;
            _dashDir = new Vector2(1, 0);
        }
    }

    private void HorzDampening(int dampening, int deadzone)
    {
        if (_velocity.x >= -deadzone && _velocity.x <= deadzone)
            _velocity.x = 0;
        else if (_velocity.x > deadzone)
            _velocity.x -= dampening;
        else if (_velocity.x < deadzone)
            _velocity.x += dampening;
    }

    private void Jump()
    {
        _gameManager.Audio.PlaySFX("player_jump", 80);
        _velocity.y -= JUMP_FORCE;
    }

    private bool IsOnGround() => _rayCast2DFloorCheck.IsColliding();
    private bool IsOnSlope()
    {
        if (_rayCast2DSlopeCheck.IsColliding())
            return false;

        return _rayCast2DSlopeCheck.GetCollisionNormal().Dot(Vector2.Up) < 1;
    }

    private bool IsFalling() => _velocity.y > 0;

    private void UpdateMoveDirection()
    {
        _moveDir.x = -Convert.ToInt32(Input.IsActionPressed("player_move_left")) + Convert.ToInt32(Input.IsActionPressed("player_move_right"));
        _moveDir.y = Input.IsActionPressed("player_jump") ? 1 : 0;
    }


    private void UpdateWallDirection()
    {
        var left = IsTouchingWallLeft();
        var right = IsTouchingWallRight();

        _sprite.FlipH = right;

        _wallDir = -Convert.ToInt32(left) + Convert.ToInt32(right);
    }

    private bool IsTouchingWallLeft()
    {
        foreach (var raycast in _rayCast2DWallChecksLeft)
            if (raycast.IsColliding())
                return true;

        return false;
    }

    private bool IsTouchingWallRight()
    {
        foreach (var raycast in _rayCast2DWallChecksRight)
            if (raycast.IsColliding())
                return true;

        return false;
    }

    public void Died()
    {
        _dieStartPos = Position.y;

        _dieTween.InterpolateProperty
        (
            "position:y", 
            _dieStartPos, 
            _dieStartPos - 30, 
            0.75f, 
            0, 
            Tween.TransitionType.Quint, 
            Tween.EaseType.Out
        );
        
        _dieTween.InterpolateProperty
        (
            "position:y", 
            _dieStartPos - 30, 
            _dieStartPos + 100, 
            1.25f, 
            0.75f, 
            Tween.TransitionType.Circ, 
            Tween.EaseType.In
        );

        _dieTween.Start();
        _haltPlayerLogic = true;
        _gameManager.Audio.StopMusic();
        _gameManager.Audio.PlaySFX("game_over");
        _dieTween.OnAllCompleted(nameof(OnDieTweenCompleted));
        
    }

    private async void OnDieTweenCompleted()
    {
        await _gameManager.TransitionManager.AlphaToBlackAndBack();
        _haltPlayerLogic = false;
        _gameManager.LevelManager.LoadLevel();
    }

    private void OnDashReady() => _dashReady = true;
    private void OnPreventHorzDone() => _canHorzMove = true;
    private void OnDashDurationDone() => _currentlyDashing = false;

    private async void _on_Player_Area_area_entered(Area2D area)
    {
        if (_haltPlayerLogic)
            return;

        if (area.IsInGroup("Killzone"))
        {
            Died();
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
            Died();
            return;
        }

        if (area.IsInGroup("Coin"))
        {
            _gameManager.Audio.PlaySFX("coin_pickup", 70);
            area.GetParent().QueueFree();
        }
    }
}
