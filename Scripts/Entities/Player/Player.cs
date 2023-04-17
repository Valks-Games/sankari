using Godot;

namespace Sankari;

public partial class Player : MovingEntity<Player>
{
    public override int MaxSpeedWalk { get; set; } = 300;
    public bool CurrentlyDashing { get; set; }
    public int JumpForce { get; set; } = 100; // Force applies when jumping
    private float JumpForceLoss { get; set; } = 7.5f;
    private float JumpForceLossCounter { get; set; }
    private bool HoldingJumpKey { get; set; }
    public int MaxJumps { get; set; } = 1; // Max number of Jumps
    public bool AllowAirJumps { get; set; } = false; // Allow mid air jumping

    private int JumpCount { get; set; }
    private int StompForce { get; set; } = 600;

    public override int HalfHearts 
    { 
        get => base.HalfHearts; 
        set 
        {
            base.HalfHearts = value;

            HealthBar.QueueFreeChildren(); // clear all heart sprites

            var fullHearts = value / 2;
            var halfHearts = value % 2;

            for (int i = 0; i < halfHearts; i++)
                AddHeartSprite(Heart.Half);

            for (int i = 0; i < fullHearts; i++)
                AddHeartSprite(Heart.Full);
        }
    }

    public GTimer        TimerNetSend                       { get; set; }
    public LevelScene    LevelScene                         { get; set; }
    public Vector2       PrevNetPos                         { get; set; }
    public MovementInput PlayerInput                        { get; set; }
    public int           HorizontalDeadZone                 { get; set; } = 25;
    public GTween        DieTween                           { get; set; }
    public GTimer        DontCheckPlatformAfterDashDuration { get; set; }
    public GTimer        PreventMovementTimer               { get; set; }

    private bool PreventMovement { get; set; } = false;

    // health
    private Node HealthBar { get; set; }

    public void PreInit(LevelScene levelScene) => LevelScene = levelScene;

    public override void Init()
    {
        HealthBar = GameManager.LevelUI.HealthBar;
        HalfHearts = 6;

        Commands[PlayerCommandType.Dash]          = new PlayerCommandDash(this);
        Commands[PlayerCommandType.WallJump]      = new PlayerCommandWallJump(this);

        Animations[EntityAnimationType.Idle]      = new PlayerAnimationIdle(this);
        Animations[EntityAnimationType.Walking]   = new PlayerAnimationWalking(this);
        Animations[EntityAnimationType.Running]   = new PlayerAnimationRunning(this);
        Animations[EntityAnimationType.JumpStart] = new PlayerAnimationJumpStart(this);
        Animations[EntityAnimationType.JumpFall]  = new PlayerAnimationJumpFall(this);
        Animations[EntityAnimationType.Dash]      = new PlayerAnimationDash(this);

        CurrentAnimation = EntityAnimationType.Idle;

        if (GameManager.PlayerManager.ActiveCheckpoint)
            Position = GameManager.PlayerManager.RespawnPosition;

        DieTween = new GTween(this);

        // dont go under platform at the end of a dash for X ms
        GetCommandClass<PlayerCommandWallJump>(PlayerCommandType.WallJump).WallJump += OnWallJump;

        DontCheckPlatformAfterDashDuration = new GTimer(this, 500);
        PreventMovementTimer               = new GTimer(this, PreventMovementFinished, 50);
    }

    public override void UpdatePhysics()
    {
        PlayerInput = MovementUtils.GetPlayerMovementInput(); // PlayerInput = ... needs to go before base._PhysicsProcess(delta)

        UpdateMoveDirection(PlayerInput);

        if (!CurrentlyDashing && !DontCheckPlatformAfterDashDuration.IsActive())
            UpdateUnderPlatform(PlayerInput);

        // Check if Entity in on ground and not current moving away from it
        if (IsNearGround() && Velocity.Y >=0)
            JumpCount = 0;

        // jump is handled before all movement restrictions
        if (PlayerInput.IsJumpJustPressed)
        {
            if (!IsNearGround()) // Wall jump
            {
                Commands[PlayerCommandType.WallJump].Start();
            }
            else
            {
                // Ground Jump
                if (JumpCount < MaxJumps && (IsNearGround() || AllowAirJumps))
                {
                    HoldingJumpKey = true;
                    JumpForceLossCounter = 0;

                    Events.Player.Notify(EventPlayer.OnJump);

                    JumpCount++;
                    //Velocity = new Vector2(Velocity.X, 0); // reset velocity before jump (is this really needed?)
                    //Velocity = Velocity - new Vector2(0, JumpForce);
                }
            }
        }

        if (PlayerInput.IsJumpPressed && HoldingJumpKey)
        {
            JumpForceLossCounter += JumpForceLoss;
            Velocity -= new Vector2(0, Mathf.Max(0, JumpForce - JumpForceLossCounter));
        }

        if (PlayerInput.IsJumpJustReleased)
        {
            HoldingJumpKey = false;
        }

        if (PlayerInput.IsDash)
            Commands[PlayerCommandType.Dash].Start();

        Velocity = new Vector2(MoveDeadZone(Velocity.X, HorizontalDeadZone), Velocity.Y); // must be after ClampAndDampen(...)
    }

    public override void UpdatePhysicsGround()
    {
        if (PlayerInput.IsSprint)
        {
            MaxSpeed = MaxSpeedSprint;
            Commands.Values.ForEach(cmd => cmd.UpdateGroundSprinting(Delta));
        }
        else
        {
            MaxSpeed = MaxSpeedWalk;
            Commands.Values.ForEach(cmd => cmd.UpdateGroundWalking(Delta));
        }
    }

    public override void UpdatePhysicsAir()
    {
        if (PlayerInput.IsFastFall)
            Velocity = Velocity + new Vector2(0, 10);
        if (PlayerInput.IsStomp)
            Velocity = new Vector2(0, StompForce);
    }

    /// <summary>
    /// Called when a Dash Command finishes as Dash
    /// </summary>
    public void OnDashDone(object _, EventArgs _2) => DontCheckPlatformAfterDashDuration.StartMs();

    public void OnWallJump(object _, EventArgs _2)
    {
        Events.Player.Notify(EventPlayer.OnJump);

        // Lock movement
        PreventMovement = true;
        PreventMovementTimer.StartMs(); 
    }

    public void PreventMovementFinished()
    {
        PreventMovement = false;
    }

    private float MoveDeadZone(float horzVelocity, int deadzone)
    {
        if (MoveDir.X == 0 && horzVelocity >= -deadzone && horzVelocity <= deadzone)
            return horzVelocity * 0.5f;

        return horzVelocity;
    }

    private async void UpdateUnderPlatform(MovementInput input)
    {
        var collision = RaycastsGround[0].GetCollider();

        if (collision is TileMap tilemap)
        {
            if (input.IsDown && tilemap.IsInGroup("Platform"))
            {
                // Player is in layer 1
                // Enemies are in layer 2

                tilemap.EnableLayers(2); // this disables layer 1 (the layer the player is in)
                await Task.Delay(1000);
                tilemap.EnableLayers(1, 2); // re-enable layers 1 and 2

                // This works but isn't the best. For example for multiplayer, if all "OtherPlayer"s are
                // in layer 1, one player disabling the layer will disable the platform for all players.
                // Also what if we want to move this implementation to enemies? If a enemy disables the
                // layer 2, all other enemies on that platform will fall as well!
            }
        }
    }

    private void UpdateMoveDirection(MovementInput input)
    {
        if (!PreventMovement)
        {
            var x = -Convert.ToInt32(input.IsLeft) + Convert.ToInt32(input.IsRight);
            var y = -Convert.ToInt32(input.IsUp) + Convert.ToInt32(input.IsDown);
            MoveDir = new Vector2(x, y);
        }
        else
        {
            MoveDir = MovementUtils.GetDirection(Velocity);
        }
    }

    public override void Kill() => new PlayerCommandDeath(this).Start();

    public async Task FinishedLevel() // this feels like it should be moved somewhere else
    {
        HaltLogic = true;
        await LevelManager.CompleteLevel(LevelManager.CurrentLevel);
        HaltLogic = false;
    }

    public async void OnDieTweenCompleted()
    {
        if (GameManager.PlayerManager.RemoveLife())
        {
            await GameManager.Transition.AlphaToBlack();
            await Task.Delay(1000);
            GameManager.LevelUI.ShowLives();
            await Task.Delay(1750);
            GameManager.LevelUI.SetLabelLives(GameManager.PlayerManager.Lives);
            await Task.Delay(1000);
            await GameManager.LevelUI.HideLivesTransition();
            await Task.Delay(250);
            GameManager.LevelUI.SetLabelCoins(GameManager.PlayerManager.Coins);
            GameManager.Transition.BlackToAlpha();
            HaltLogic = false;
            LevelManager.LoadLevelFast();
            LevelScene.Camera.StartFollowingPlayer();
        }
        else
        {
            GameManager.PlayerManager.ResetCoins();
            await GameManager.Transition.AlphaToBlack();
            await Task.Delay(1000);
            GameManager.LevelUI.ShowGameOver();
            await Task.Delay(1750);
            GameManager.LoadMap();
            GameManager.Transition.BlackToAlpha();
            GameManager.LevelUI.HideGameOver();
            GameManager.LevelUI.SetLabelCoins(GameManager.PlayerManager.Coins);
        }
    }

    private enum Heart
    {
        Half,
        Full
    }

    private void AddHeartSprite(Heart heart)
    {
        var heartTexture = heart == Heart.Half ? Textures.HalfHeart : Textures.FullHeart;

        var textureRect = new TextureRect()
        {
            Texture = heartTexture,
            ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
            CustomMinimumSize = new Vector2(50, 50),
            StretchMode = TextureRect.StretchModeEnum.KeepAspect
        };

        HealthBar.AddChild(textureRect);
    }
}
