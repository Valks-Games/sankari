namespace Sankari;

public partial class Player : CharacterBody2D
{
    [Export] protected  NodePath NodePathRayCast2DWallChecksLeft;
    [Export] protected  NodePath NodePathRayCast2DWallChecksRight;
    [Export] protected  NodePath NodePathRayCast2DGroundChecks;

    public static Vector2 RespawnPosition { get; set; }
    public static bool HasTouchedCheckpoint { get; set; }
    public static Player Instance;

    private const int UNIVERSAL_FORCE_MODIFIER = 4;
    private const int SPEED_GROUND = 15 * UNIVERSAL_FORCE_MODIFIER;
    private const int SPEED_AIR = 4 * UNIVERSAL_FORCE_MODIFIER;
    private const int SPEED_MAX_GROUND = 75 * UNIVERSAL_FORCE_MODIFIER;
    private const int SPEED_MAX_GROUND_SPRINT = 100 * UNIVERSAL_FORCE_MODIFIER;
    private const int SPEED_MAX_AIR = 225 * UNIVERSAL_FORCE_MODIFIER;
    private const int SPEED_DASH_VERTICAL = 100 * UNIVERSAL_FORCE_MODIFIER;
    private const int SPEED_DASH_HORIZONTAL = 150 * UNIVERSAL_FORCE_MODIFIER;
    private const int GRAVITY_AIR = 350 * UNIVERSAL_FORCE_MODIFIER;
    private const int GRAVITY_WALL = 750 * UNIVERSAL_FORCE_MODIFIER;
    private const int JUMP_FORCE = 150 * UNIVERSAL_FORCE_MODIFIER;
    private const int JUMP_FORCE_WALL_VERT = 150 * UNIVERSAL_FORCE_MODIFIER;
    private const int JUMP_FORCE_WALL_HORZ = 75 * UNIVERSAL_FORCE_MODIFIER;
    private const int DASH_COOLDOWN = 350;
    private const int DASH_DURATION = 200;

    // dependecy injcetion
    private LevelScene levelScene;

    // movement
    private Vector2 prevNetPos;
    private Vector2 moveDir;
    private Vector2 velocity;
    private bool haltPlayerLogic;

    // timers
    private GTimer timerDashCooldown;
    private GTimer timerDashDuration;
    private GTimer timerNetSend;

    // raycasts
    private Node2D parentWallChecksLeft;
    private Node2D parentWallChecksRight;
    private readonly List<RayCast2D> rayCast2DWallChecksLeft = new();
    private readonly List<RayCast2D> rayCast2DWallChecksRight = new();
    private readonly List<RayCast2D> rayCast2DGroundChecks = new();
    private Node2D parentGroundChecks;

    // animation
    private AnimatedSprite2D animatedSprite;
    private GTween dieTween;

    // wall
    private bool inWallJumpArea;
    private int wallDir;

    // dash
    private Vector2 dashDir;
    private const int MAX_DASHES = 1;
    private int dashCount;
    private bool horizontalDash;
    private bool dashReady = true;
    private bool currentlyDashing;

    // msc
    private Window tree;

    public void PreInit(LevelScene levelScene)
    {
        this.levelScene = levelScene;
    }

    public override void _Ready()
    {
        Instance = this;
        if (HasTouchedCheckpoint)
            Position = RespawnPosition;
        timerDashCooldown = new GTimer(this, nameof(OnDashReady), DASH_COOLDOWN, false, false);
        timerDashDuration = new GTimer(this, nameof(OnDashDurationDone), DASH_DURATION, false, false);
        timerNetSend = new GTimer(this, nameof(NetUpdate), NetIntervals.HEARTBEAT, true, GameManager.Net.IsMultiplayer());
        parentGroundChecks = GetNode<Node2D>(NodePathRayCast2DGroundChecks);
        parentWallChecksLeft = GetNode<Node2D>(NodePathRayCast2DWallChecksLeft);
        parentWallChecksRight = GetNode<Node2D>(NodePathRayCast2DWallChecksRight);
        animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        dieTween = new GTween(this);
        tree = GetTree().Root;

        PrepareRaycasts(parentWallChecksLeft, rayCast2DWallChecksLeft);
        PrepareRaycasts(parentWallChecksRight, rayCast2DWallChecksRight);
        PrepareRaycasts(parentGroundChecks, rayCast2DGroundChecks);

        animatedSprite.Play("idle");

		UpDirection = Vector2.Up;
    }

    public override void _PhysicsProcess(double d)
    {
        var delta = (float)d;
        if (haltPlayerLogic)
            return;

        UpdateMoveDirection();
        HandleMovement(delta);
    }

    private void NetUpdate() 
    {
        if (Position != prevNetPos)
            GameManager.Net.Client.Send(ClientPacketOpcode.PlayerPosition, new CPacketPlayerPosition 
            {
                Position = Position
            });

        prevNetPos = Position;
    }

    private void HandleMovement(float delta)
    {
        var inputJump = Input.IsActionJustPressed("player_jump");
        var inputUp = Input.IsActionPressed("player_move_up");
        var inputDown = Input.IsActionPressed("player_move_down");
        var inputFastFall = Input.IsActionPressed("player_fast_fall");
        var inputDash = Input.IsActionJustPressed("player_dash");
        var inputSprint = Input.IsActionPressed("player_sprint");

        var gravity = GRAVITY_AIR;
        wallDir = UpdateWallDirection();

        // on a wall and falling
        if (wallDir != 0 && inWallJumpArea)
        {
            animatedSprite.FlipH = wallDir == 1;

            if (IsFalling())
            {
                velocity.y = 0;
                gravity = GRAVITY_WALL;

                if (inputDown)
                    velocity.y += 50;

                // wall jump
                if (inputJump)
                {
                    Jump();
                    velocity.x += -JUMP_FORCE_WALL_HORZ * wallDir;
                    velocity.y -= JUMP_FORCE_WALL_VERT;
                }
            }
        }
        else
        {
            animatedSprite.FlipH = false;
        }

        CheckIfCanGoUnderPlatform(inputDown);

        // dash
        if (inputDash && dashReady && !currentlyDashing && dashCount != MAX_DASHES && !IsOnGround())
        {
            dashDir = GetDashDirection(inputUp, inputDown);

            if (dashDir != Vector2.Zero)
            {
                dashCount++;
                GameManager.Audio.PlaySFX("dash");
                dashReady = false;
                currentlyDashing = true;
                timerDashDuration.Start();
                timerDashCooldown.Start();
            }
        }

        if (currentlyDashing)
        {
            gravity = 0;
            DoDashStuff();
        }
        else
            gravity = GRAVITY_AIR;

        animatedSprite.FlipH = moveDir.x < 0;

        if (IsOnGround())
        {
            dashCount = 0;

            if (moveDir.x != 0)
                animatedSprite.Play("walk");
            else
                animatedSprite.Play("idle");

            velocity.x += moveDir.x * SPEED_GROUND;

            HorzDampening(20);

            if (inputJump)
            {
                Jump();
                velocity.y = 0;
                velocity.y -= JUMP_FORCE;
            }
        }
        else
        {
            velocity.x += moveDir.x * SPEED_AIR;

            if (inputFastFall)
                velocity.y += 10;
        }

        if (IsFalling())
            animatedSprite.Play("jump_fall");

        // apply gravity
        velocity.y += gravity * delta;

        if (!currentlyDashing)
        {
            if (IsOnGround() && inputSprint)
            {
                animatedSprite.SpeedScale = 1.5f;
                velocity.x = Mathf.Clamp(velocity.x, -SPEED_MAX_GROUND_SPRINT, SPEED_MAX_GROUND_SPRINT);
            }
            else 
            {
                animatedSprite.SpeedScale = 1;
                velocity.x = Mathf.Clamp(velocity.x, -SPEED_MAX_GROUND, SPEED_MAX_GROUND);
            }

            velocity.y = Mathf.Clamp(velocity.y, -SPEED_MAX_AIR, SPEED_MAX_AIR);
        }

		Velocity = velocity;

        MoveAndSlide();
    }

    private void Jump()
    {
        animatedSprite.Play("jump_start");
        GameManager.Audio.PlaySFX("player_jump", 80);
    }

    private async void CheckIfCanGoUnderPlatform(bool inputDown)
    {
        var collision = rayCast2DGroundChecks[0].GetCollider(); // seems like were getting this twice, this could be optimized to only be got once in _Ready and made into a private variable

        if (collision != null && collision is TileMap)
        {
            var tilemap = collision as TileMap;

            if (inputDown && tilemap.IsInGroup("Platform"))
            {
                tilemap.EnableLayers(2);
                await Task.Delay(1000);
                tilemap.EnableLayers(1, 2);
            }
        }
    }

    private void DoDashStuff()
    {
        var sprite = Prefabs.PlayerDashTrace.Instantiate<Sprite2D>();
        sprite.Texture = animatedSprite.Frames.GetFrame(animatedSprite.Animation, animatedSprite.Frame);
        sprite.GlobalPosition = GlobalPosition;
        sprite.Scale = new Vector2(2f, 2f);
        sprite.FlipH = animatedSprite.FlipH;
        //sprite.FlipH = wallDir == 1 ? true : false;
        tree.AddChild(sprite);

        var dashSpeed = SPEED_DASH_VERTICAL;

        if (horizontalDash)
            dashSpeed = SPEED_DASH_HORIZONTAL;

        velocity = dashDir * dashSpeed;
    }

    private Vector2 GetDashDirection(bool inputUp, bool inputDown)
    {
        if (inputDown && moveDir.x < 0)
        {
            return new Vector2(-1, 1);
        }
        else if (inputDown && moveDir.x == 0)
        {
            horizontalDash = false;
            return new Vector2(0, 1);
        }
        else if (inputDown && moveDir.x > 0)
        {
            return new Vector2(1, 1);
        }
        else if (inputUp && moveDir.x < 0)
        {
            horizontalDash = false;
            return new Vector2(-1, -1);
        }
        else if (inputUp && moveDir.x > 0)
        {
            horizontalDash = false;
            return new Vector2(1, -1);
        }
        else if (inputUp)
        {
            horizontalDash = false;
            return new Vector2(0, -1);
        }
        else if (moveDir.x < 0)
        {
            horizontalDash = true;
            return new Vector2(-1, 0);
        }
        else if (moveDir.x > 0)
        {
            horizontalDash = true;
            return new Vector2(1, 0);
        }

        return Vector2.Zero;
    }

    private void HorzDampening(int dampening)
    {
        // deadzone has to be bigger than dampening value or the player ghost slide effect will occur
        int deadzone = (int)(dampening * 1.5f);

        if (velocity.x >= -deadzone && velocity.x <= deadzone)
        {
            velocity.x = 0;
        }
        else if (velocity.x > deadzone)
        {
            velocity.x -= dampening;
        }
        else if (velocity.x < deadzone)
        {
            velocity.x += dampening;
        }
    }

    private bool IsOnGround()
    {
        foreach (var raycast in rayCast2DGroundChecks)
            if (raycast.IsColliding())
                return true;

        return false;
    }

    private bool IsFalling() => Velocity.y > 0;

    private void UpdateMoveDirection()
    {
        moveDir.x = -Convert.ToInt32(Input.IsActionPressed("player_move_left")) + Convert.ToInt32(Input.IsActionPressed("player_move_right"));
        moveDir.y = Input.IsActionPressed("player_jump") ? 1 : 0;
    }

    private int UpdateWallDirection()
    {
        var left = IsTouchingWallLeft();
        var right = IsTouchingWallRight();

        return -Convert.ToInt32(left) + Convert.ToInt32(right);
    }

    private bool IsTouchingWallLeft()
    {
        foreach (var raycast in rayCast2DWallChecksLeft)
            if (raycast.IsColliding())
                return true;

        return false;
    }

    private bool IsTouchingWallRight()
    {
        foreach (var raycast in rayCast2DWallChecksRight)
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
        animatedSprite.Stop();
        levelScene.camera.StopFollowingPlayer();

        var dieStartPos = Position.y;

        // animate y position
        dieTween.InterpolateProperty
        (
            "position:y",
            dieStartPos - 80,
            0.75f
        );

        dieTween.InterpolateProperty
        (
            "position:y",
            dieStartPos + 600,
            1f
        );

        // animate rotation
        dieTween.InterpolateProperty
        (
            "rotation_degrees",
            0,
            160
        );

        dieTween.Start();
        haltPlayerLogic = true;
        GameManager.Audio.StopMusic();
        GameManager.Audio.PlaySFX("game_over_1");
        dieTween.OnAllCompleted(nameof(OnDieTweenCompleted));
    }

    private async void OnDieTweenCompleted()
    {
        await GameManager.Transition.AlphaToBlack();
        await Task.Delay(1000);
        GameManager.LevelUI.ShowLives();
        await Task.Delay(1750);
        GameManager.LevelUI.RemoveLife();
        await Task.Delay(1000);
        await GameManager.LevelUI.HideLivesTransition();
        await Task.Delay(250);
        GameManager.Transition.BlackToAlpha();
        haltPlayerLogic = false;
        GameManager.Level.LoadLevelFast();
        levelScene.camera.StartFollowingPlayer();
    }

    private void OnDashReady() => dashReady = true;
    private void OnDashDurationDone() => currentlyDashing = false;

    private async void _on_Player_Area_area_entered(Area2D area)
    {
        if (haltPlayerLogic)
            return;

        if (area.IsInGroup("Killzone"))
        {
            Died();
            return;
        }

        if (area.IsInGroup("Level Finish"))
        {
            haltPlayerLogic = true;
            await GameManager.Level.CompleteLevel(GameManager.Level.CurrentLevel);
            haltPlayerLogic = false;
            return;
        }

        if (area.IsInGroup("Enemy"))
        {
            Died();
            return;
        }

        if (area.IsInGroup("WallJumpArea"))
            inWallJumpArea = true;
    }

    private void _on_Area_area_exited(Area2D area)
    {
        if (area.IsInGroup("WallJumpArea"))
            inWallJumpArea = false;
    }
}
