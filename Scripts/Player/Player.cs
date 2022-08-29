namespace Sankari;

public class Player : KinematicBody2D
{
    [Export] protected readonly NodePath NodePathRayCast2DWallChecksLeft;
    [Export] protected readonly NodePath NodePathRayCast2DWallChecksRight;
    [Export] protected readonly NodePath NodePathRayCast2DGroundChecks;
    [Export] protected readonly NodePath NodePathSprite;

    private const int SPEED_GROUND_WALK = 15;
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

    // dependecy injcetion
    private GameManager _gameManager;
    private LevelManager _levelManager;

    // movement
    private Vector2 _moveDir;
    private Vector2 _velocity;
    private bool _haltPlayerLogic;

    // timers
    private GTimer _timerDashCooldown;
    private GTimer _timerDashDuration;

    // raycasts
    private Node2D _parentWallChecksLeft;
    private Node2D _parentWallChecksRight;
    private List<RayCast2D> _rayCast2DWallChecksLeft = new();
    private List<RayCast2D> _rayCast2DWallChecksRight = new();
    private List<RayCast2D> _rayCast2DGroundChecks = new();
    private Node2D _parentGroundChecks;
    
    // animation
    private Sprite _sprite;
    private GTween _dieTween;

    // dash
    private Vector2 _dashDir;
    private bool _horizontalDash;
    private bool _hasTouchedGroundAfterDash = true;
    private bool _dashReady = true;
    private bool _currentlyDashing;

    public void PreInit(GameManager gameManager)
    {
        _gameManager = gameManager;
        _levelManager = gameManager.LevelManager;
    }

    public override void _Ready()
    {
        _timerDashCooldown = new GTimer(this, nameof(OnDashReady), DASH_COOLDOWN, false, false);
        _timerDashDuration = new GTimer(this, nameof(OnDashDurationDone), DASH_DURATION, false, false);
        _parentGroundChecks = GetNode<Node2D>(NodePathRayCast2DGroundChecks);
        _parentWallChecksLeft = GetNode<Node2D>(NodePathRayCast2DWallChecksLeft);
        _parentWallChecksRight = GetNode<Node2D>(NodePathRayCast2DWallChecksRight);
        _sprite = GetNode<Sprite>(NodePathSprite);
        _dieTween = new GTween(this);

        PrepareRaycasts(_parentWallChecksLeft, _rayCast2DWallChecksLeft);
        PrepareRaycasts(_parentWallChecksRight, _rayCast2DWallChecksRight);
        PrepareRaycasts(_parentGroundChecks, _rayCast2DGroundChecks);
    }

    public override void _PhysicsProcess(float delta)
    {
        if (_haltPlayerLogic)
            return;

        UpdateMoveDirection();
        HandleMovement(delta);
    }

    private void HandleMovement(float delta)
    {
        var inputJump = Input.IsActionJustPressed("player_jump");
        var inputDown = Input.IsActionPressed("player_move_down");
        var inputDash = Input.IsActionJustPressed("player_dash");

        var gravity = GRAVITY_AIR;
        var wallDir = UpdateWallDirection();

        // on a wall and falling
        if (wallDir != 0)
        {
            if (IsFalling())
            {
                _velocity.y = 0;
                gravity = GRAVITY_WALL;

                if (inputDown)
                    _velocity.y += 50;

                // wall jump
                if (inputJump)
                {
                    _velocity.x += -JUMP_FORCE_WALL_HORZ * wallDir;
                    _velocity.y -= JUMP_FORCE_WALL_VERT;
                }
            }
        }

        CheckIfCanGoUnderPlatform(inputDown);

        // dash
        if (inputDash && _dashReady && _hasTouchedGroundAfterDash && !_currentlyDashing)
        {
            _gameManager.Audio.PlaySFX("dash");
            _dashReady = false;
            _currentlyDashing = true;
            _timerDashDuration.Start();
            _timerDashCooldown.Start();
            _dashDir = GetDashDirection();
        }

        if (_currentlyDashing)
        {
            gravity = 0;
            DoDashStuff();
        }
        else
            gravity = GRAVITY_AIR;

        if (IsOnGround())
        {
            _hasTouchedGroundAfterDash = true;

            _velocity.x += _moveDir.x * SPEED_GROUND_WALK;

            HorzDampening(5, 2);

            if (inputJump)
            {
                _gameManager.Audio.PlaySFX("player_jump", 80);
                _velocity.y -= JUMP_FORCE;
            }
        }
        else
        {
            _velocity.x += _moveDir.x * SPEED_AIR;
        }

        // apply gravity
        _velocity.y += gravity * delta;

        if (!_currentlyDashing)
        {
            _velocity.x = Mathf.Clamp(_velocity.x, -SPEED_MAX_GROUND, SPEED_MAX_GROUND);
            _velocity.y = Mathf.Clamp(_velocity.y, -SPEED_MAX_AIR, SPEED_MAX_AIR);
        }

        _velocity = MoveAndSlide(_velocity, Vector2.Up);
    }

    private void CheckIfCanGoUnderPlatform(bool inputDown)
    {
        var collision = _rayCast2DGroundChecks[0].GetCollider(); // seems like were getting this twice, this could be optimized to only be got once in _Ready and made into a private variable

        if (collision != null)
        {
            var node = (Node)collision;

            if (node.IsInGroup("Platform") && inputDown)
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
        _hasTouchedGroundAfterDash = false;
    }

    private Vector2 GetDashDirection()
    {
        // determine dash direction
        var input_up = Input.IsActionPressed("player_move_up");

        if (input_up && _moveDir.x < 0)
        {
            _horizontalDash = false;
            return new Vector2(-1, -1);
        }
        else if (input_up && _moveDir.x > 0)
        {
            _horizontalDash = false;
            return new Vector2(1, -1);
        }
        else if (input_up)
        {
            _horizontalDash = false;
            return new Vector2(0, -1);
        }
        else if (_moveDir.x < 0)
        {
            _horizontalDash = true;
            return new Vector2(-1, 0);
        }
        else if (_moveDir.x > 0)
        {
            _horizontalDash = true;
            return new Vector2(1, 0);
        }

        return Vector2.Zero;
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

    private bool IsOnGround()
    {
        foreach (var raycast in _rayCast2DGroundChecks)
            if (raycast.IsColliding())
                return true;

        return false;
    }

    private bool IsFalling() => _velocity.y > 0;

    private void UpdateMoveDirection()
    {
        _moveDir.x = -Convert.ToInt32(Input.IsActionPressed("player_move_left")) + Convert.ToInt32(Input.IsActionPressed("player_move_right"));
        _moveDir.y = Input.IsActionPressed("player_jump") ? 1 : 0;
    }

    private int UpdateWallDirection()
    {
        var left = IsTouchingWallLeft();
        var right = IsTouchingWallRight();

        _sprite.FlipH = right;

        return -Convert.ToInt32(left) + Convert.ToInt32(right);
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

    private void PrepareRaycasts(Node parent, List<RayCast2D> list)
    {
        foreach (RayCast2D raycast in parent.GetChildren())
        {
            raycast.AddException(this);
            list.Add(raycast);
        }
    }

    public void Died()
    {
        var dieStartPos = Position.y;

        // animate y position
        _dieTween.InterpolateProperty
        (
            "position:y",
            dieStartPos,
            dieStartPos - 30,
            0.75f,
            0,
            Tween.TransitionType.Quint,
            Tween.EaseType.Out
        );

        _dieTween.InterpolateProperty
        (
            "position:y",
            dieStartPos - 30,
            dieStartPos + 100,
            1f,
            0.75f,
            Tween.TransitionType.Circ,
            Tween.EaseType.In
        );

        // animate rotation
        _dieTween.InterpolateProperty
        (
            "rotation_degrees",
            0,
            160,
            2f,
            0.25f
        );

        _dieTween.Start();
        _haltPlayerLogic = true;
        _gameManager.Audio.StopMusic();
        _gameManager.Audio.PlaySFX("game_over_1");
        _dieTween.OnAllCompleted(nameof(OnDieTweenCompleted));

    }

    private async void OnDieTweenCompleted()
    {
        await _gameManager.TransitionManager.AlphaToBlackAndBack();
        _haltPlayerLogic = false;
        _gameManager.LevelManager.LoadLevel();
    }

    private void OnDashReady() => _dashReady = true;
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
