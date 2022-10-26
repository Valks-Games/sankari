namespace Sankari;

public interface IPlayerAnimations : IEntityAnimationDash, IEntityAnimation
{ }

public interface IPlayerCommands : IEntityDash, IEntityWallJumpable, IEntityGroundJumpable, IEntityMovement
{ }

public partial class Player : Entity, IPlayerAnimations, IPlayerCommands
{
	[Export] protected NodePath NodePathRayCast2DWallChecksLeft { get; set; }
	[Export] protected NodePath NodePathRayCast2DWallChecksRight { get; set; }
	[Export] protected NodePath NodePathRayCast2DGroundChecks { get; set; }

	// Static
	public static Vector2 RespawnPosition { get; set; }

	public static bool HasTouchedCheckpoint { get; set; }

	// IEntityWallJumpable
	public List<RayCast2D> RayCast2DWallChecksLeft { get; } = new();

	public List<RayCast2D> RayCast2DWallChecksRight { get; } = new();

	// IEntityJumpable
	public bool InWallJumpArea { get; set; }

	// IEntityMovement
	public int GroundAcceleration { get; set; } = 50;

	// IEntityMoveable
	public Window Tree { get; set; }

	// IEntityDash
	public bool CurrentlyDashing { get; set; }

	// IEntityAnimation
	public AnimatedSprite2D AnimatedSprite { get; set; }

	// Not in a interface
	public GTimer        TimerNetSend                       { get; set; }
	public LevelScene    LevelScene                         { get; set; }
	public Vector2       PrevNetPos                         { get; set; }
	public MovementInput PlayerInput                        { get; set; }
	public int           HorizontalDeadZone                 { get; set; } = 25;
	public GTween        DieTween                           { get; set; }
	public GTimer        DontCheckPlatformAfterDashDuration { get; set; }

	public void PreInit(LevelScene levelScene) => LevelScene = levelScene;

	public override void _Ready()
	{
		Commands[EntityCommandType.Movement]      = new EntityCommandMovement(this);
		Commands[EntityCommandType.Dash]          = new EntityCommandDash(this);
		Commands[EntityCommandType.WallJump]      = new EntityCommandWallJump(this);
		Commands[EntityCommandType.GroundJump]    = new EntityCommandGroundJump(this);

		Animations[EntityAnimationType.Idle]      = new EntityAnimationIdle(this);
		Animations[EntityAnimationType.Walking]   = new EntityAnimationWalking(this);
		Animations[EntityAnimationType.Running]   = new EntityAnimationRunning(this);
		Animations[EntityAnimationType.JumpStart] = new EntityAnimationJumpStart(this);
		Animations[EntityAnimationType.JumpFall]  = new EntityAnimationJumpFall(this);
		Animations[EntityAnimationType.Dash]      = new EntityAnimationDash(this);

		CurrentAnimation = EntityAnimationType.Idle;

		if (HasTouchedCheckpoint)
			Position = RespawnPosition;

		TimerNetSend          = new GTimer(this, nameof(NetUpdate), NetIntervals.HEARTBEAT, true, Net.IsMultiplayer());
		ParentGroundChecks    = GetNode<Node2D>(NodePathRayCast2DGroundChecks);
		ParentWallChecksLeft  = GetNode<Node2D>(NodePathRayCast2DWallChecksLeft);
		ParentWallChecksRight = GetNode<Node2D>(NodePathRayCast2DWallChecksRight);
		AnimatedSprite        = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		DieTween              = new GTween(this);
		Timers                = new GTimers(this);
		Tree                  = GetTree().Root;

		// dont go under platform at the end of a dash for X ms
		GetCommandClass<EntityCommandDash>(EntityCommandType.Dash).DashDurationDone += OnDashDone;
		DontCheckPlatformAfterDashDuration = new GTimer(this, 500, false, false);

		PrepareRaycasts(ParentWallChecksLeft , RayCast2DWallChecksLeft);
		PrepareRaycasts(ParentWallChecksRight, RayCast2DWallChecksRight);
		PrepareRaycasts(ParentGroundChecks   , RayCast2DGroundChecks);

		base._Ready(); // there are some things in base._Ready() that require to go after everything above
	}

	public override void _PhysicsProcess(double delta)
	{
		if (HaltLogic)
			return;

		PlayerInput = MovementUtils.GetPlayerMovementInput(); // PlayerInput = ... needs to go before base._PhysicsProcess(delta)

		base._PhysicsProcess(delta);

		UpdateMoveDirection(PlayerInput);

		if (!CurrentlyDashing && !DontCheckPlatformAfterDashDuration.IsActive())
			UpdateUnderPlatform(PlayerInput);

		// jump is handled before all movement restrictions
		if (PlayerInput.IsJump)
		{
			if (!IsOnGround()) // Wall jump
			{
				Commands[EntityCommandType.WallJump].Start();
			}
			else
			{
				Commands[EntityCommandType.GroundJump].Start();
			}
		}

		if (PlayerInput.IsDash)
			Commands[EntityCommandType.Dash].Start();

		Velocity = new Vector2(MoveDeadZone(Velocity.x, HorizontalDeadZone), Velocity.y); // must be after ClampAndDampen(...)
	}

	public override void UpdateGround()
	{
		Velocity = Velocity + new Vector2(MoveDir.x * GroundAcceleration, 0);

		if (PlayerInput.IsSprint)
			Commands.Values.ForEach(cmd => cmd.UpdateGroundSprinting(Delta));
		else
			Commands.Values.ForEach(cmd => cmd.UpdateGroundWalking(Delta));
	}

	public override void UpdateAir()
	{
		if (PlayerInput.IsFastFall)
			Velocity = Velocity + new Vector2(0, 10);
	}

	/// <summary>
	/// Called when a Dash Command finishes as Dash
	/// </summary>
	public void OnDashDone(object _, EventArgs _2) => DontCheckPlatformAfterDashDuration.Start();

	private float MoveDeadZone(float horzVelocity, int deadzone)
	{
		if (MoveDir.x == 0 && horzVelocity >= -deadzone && horzVelocity <= deadzone)
			return horzVelocity * 0.5f;

		return horzVelocity;
	}

	private void NetUpdate()
	{
		if (Position != PrevNetPos)
			Net.Client.Send(ClientPacketOpcode.PlayerPosition, new CPacketPlayerPosition
			{
				Position = Position
			});

		PrevNetPos = Position;
	}

	private async void UpdateUnderPlatform(MovementInput input)
	{
		var collision = RayCast2DGroundChecks[0].GetCollider();

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
		var x = -Convert.ToInt32(input.IsLeft) + Convert.ToInt32(input.IsRight);
		var y = -Convert.ToInt32(input.IsUp) + Convert.ToInt32(input.IsDown);

		MoveDir = new Vector2(x, y);
	}

	public override void Kill() => new PlayerCommandDeath(this).Start();

	public async Task FinishedLevel()
	{
		HaltLogic = true;
		await LevelManager.CompleteLevel(LevelManager.CurrentLevel);
		HaltLogic = false;
	}

	private void _on_Player_Area_area_entered(Area2D area)
	{
		if (HaltLogic)
			return;

		if (area.IsInGroup("WallJumpArea"))
			InWallJumpArea = true;
	}

	private void _on_Area_area_exited(Area2D area)
	{
		if (area.IsInGroup("WallJumpArea"))
			InWallJumpArea = false;
	}
}
