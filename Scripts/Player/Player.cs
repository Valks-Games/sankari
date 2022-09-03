namespace Sankari;

public class Player : KinematicBody2D
{
    [Export] protected readonly NodePath NodePathRayCast2DWallChecksLeft;
    [Export] protected readonly NodePath NodePathRayCast2DWallChecksRight;
    [Export] protected readonly NodePath NodePathRayCast2DGroundChecks;

    public static Vector2 RespawnPosition { get; set; }
    public static bool HasTouchedCheckpoint { get; set; }

    private const int UNIVERSAL_FORCE_MODIFIER = 4;
    private const int SPEED_GROUND_WALK = 15 * UNIVERSAL_FORCE_MODIFIER;
    private const int SPEED_AIR = 4 * UNIVERSAL_FORCE_MODIFIER;
    private const int SPEED_MAX_GROUND = 75 * UNIVERSAL_FORCE_MODIFIER;
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
    private GameManager gameManager;

    // movement
    private Vector2 moveDir;
    private Vector2 velocity;
    private bool haltPlayerLogic;

    // timers
    private GTimer timerDashCooldown;
    private GTimer timerDashDuration;

    // raycasts
    private Node2D parentWallChecksLeft;
    private Node2D parentWallChecksRight;
    private List<RayCast2D> rayCast2DWallChecksLeft = new();
    private List<RayCast2D> rayCast2DWallChecksRight = new();
    private List<RayCast2D> rayCast2DGroundChecks = new();
    private Node2D parentGroundChecks;

    // animation
    private AnimatedSprite animatedSprite;
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
    private Viewport tree;

    public void PreInit(LevelScene levelScene)
    {
        this.levelScene = levelScene;
        gameManager = levelScene.GameManager;
    }

    public override void _Ready()
    {
        if (HasTouchedCheckpoint)
            Position = RespawnPosition;
        timerDashCooldown = new GTimer(this, nameof(OnDashReady), DASH_COOLDOWN, false, false);
        timerDashDuration = new GTimer(this, nameof(OnDashDurationDone), DASH_DURATION, false, false);
        parentGroundChecks = GetNode<Node2D>(NodePathRayCast2DGroundChecks);
        parentWallChecksLeft = GetNode<Node2D>(NodePathRayCast2DWallChecksLeft);
        parentWallChecksRight = GetNode<Node2D>(NodePathRayCast2DWallChecksRight);
        animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        dieTween = new GTween(this);
        tree = GetTree().Root;

        PrepareRaycasts(parentWallChecksLeft, rayCast2DWallChecksLeft);
        PrepareRaycasts(parentWallChecksRight, rayCast2DWallChecksRight);
        PrepareRaycasts(parentGroundChecks, rayCast2DGroundChecks);

        animatedSprite.Play("idle");
    }

    public override void _PhysicsProcess(float delta)
    {
        if (haltPlayerLogic)
            return;

        UpdateMoveDirection();
        HandleMovement(delta);
    }

    private void HandleMovement(float delta)
    {
        var inputJump = Input.IsActionJustPressed("player_jump");
        var inputUp = Input.IsActionPressed("player_move_up");
        var inputDown = Input.IsActionPressed("player_move_down");
        var inputFastFall = Input.IsActionPressed("player_fast_fall");
        var inputDash = Input.IsActionJustPressed("player_dash");

        var gravity = GRAVITY_AIR;
        wallDir = UpdateWallDirection();

        // on a wall and falling
        if (wallDir != 0 && inWallJumpArea)
        {
            animatedSprite.FlipH = wallDir == 1 ? true : false;

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
                gameManager.Audio.PlaySFX("dash");
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

        animatedSprite.FlipH = moveDir.x < 0 ? true : false;

        if (IsOnGround())
        {
            dashCount = 0;

            if (moveDir.x != 0)
                animatedSprite.Play("walk");
            else
                animatedSprite.Play("idle");

            velocity.x += moveDir.x * SPEED_GROUND_WALK;

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
            velocity.x = Mathf.Clamp(velocity.x, -SPEED_MAX_GROUND, SPEED_MAX_GROUND);
            velocity.y = Mathf.Clamp(velocity.y, -SPEED_MAX_AIR, SPEED_MAX_AIR);
        }

        velocity = MoveAndSlide(velocity, Vector2.Up);
    }

    private void Jump()
    {
        animatedSprite.Play("jump_start");
        gameManager.Audio.PlaySFX("player_jump", 80);
    }

    private void CheckIfCanGoUnderPlatform(bool inputDown)
    {
        var collision = rayCast2DGroundChecks[0].GetCollider(); // seems like were getting this twice, this could be optimized to only be got once in _Ready and made into a private variable

        if (collision != null)
        {
            var node = (Node)collision;

            if (inputDown && node.IsInGroup("Platform"))
                (node as APlatform).TemporarilyDisablePlatform();
        }
    }

    private void DoDashStuff()
    {
        var sprite = Prefabs.PlayerDashTrace.Instance<Sprite>();
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

    private bool IsFalling() => velocity.y > 0;

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
        levelScene.Camera.StopFollowingPlayer();

        var dieStartPos = Position.y;

        // animate y position
        dieTween.InterpolateProperty
        (
            "position:y",
            dieStartPos,
            dieStartPos - 80,
            0.75f,
            0,
            Tween.TransitionType.Quint,
            Tween.EaseType.Out
        );

        dieTween.InterpolateProperty
        (
            "position:y",
            dieStartPos - 80,
            dieStartPos + 600,
            1f,
            0.75f,
            Tween.TransitionType.Circ,
            Tween.EaseType.In
        );

        // animate rotation
        dieTween.InterpolateProperty
        (
            "rotation_degrees",
            0,
            160,
            2f,
            0.25f
        );

        dieTween.Start();
        haltPlayerLogic = true;
        gameManager.Audio.StopMusic();
        gameManager.Audio.PlaySFX("game_over_1");
        dieTween.OnAllCompleted(nameof(OnDieTweenCompleted));
    }

    private async void OnDieTweenCompleted()
    {
        await gameManager.TransitionManager.AlphaToBlack();
        await Task.Delay(1000);
        gameManager.LevelUIManager.ShowLives();
        await Task.Delay(1750);
        gameManager.LevelUIManager.RemoveLife();
        await Task.Delay(1000);
        await gameManager.LevelUIManager.HideLivesTransition();
        await Task.Delay(250);
        gameManager.TransitionManager.BlackToAlpha();
        haltPlayerLogic = false;
        gameManager.LevelManager.LoadLevel();
        levelScene.Camera.StartFollowingPlayer();
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
            await gameManager.LevelManager.CompleteLevel(gameManager.LevelManager.CurrentLevel);
            haltPlayerLogic = false;
            return;
        }

        if (area.IsInGroup("Enemy"))
        {
            Died();
            return;
        }

        if (area.IsInGroup("Coin"))
        {
            gameManager.LevelUIManager.AddCoins();
            gameManager.Audio.PlaySFX("coin_pickup_1", 30);
            area.GetParent().QueueFree();
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
