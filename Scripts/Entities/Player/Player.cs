namespace Sankari;

public interface IPlayerAnimations : IEntityAnimationDash, IEntityAnimation
{ }

public interface IPlayerCommands : IEntityDash, IEntityWallJumpable, IEntityGroundJumpable, IEntityMovement
{ }

public partial class Player : Entity, IPlayerAnimations, IPlayerCommands
{
	[Export] protected NodePath NodePathRayCast2DWallChecksLeft  { get; set; }
	[Export] protected NodePath NodePathRayCast2DWallChecksRight { get; set; }
	[Export] protected NodePath NodePathRayCast2DGroundChecks    { get; set; }

	// static
	public static Vector2 RespawnPosition      { get; set; }
	public static bool    HasTouchedCheckpoint { get; set; }

	// IEntityBase
	public  Vector2 MoveDir    { get; set; }
	public GTimers Timers { get; set; }

	// IEntityWallJumpable
	public  List<RayCast2D> RayCast2DWallChecksLeft  { get; } = new();
	public  List<RayCast2D> RayCast2DWallChecksRight { get; } = new();
	public int JumpForceWallHorz  { get; set; } = 800;
	public int JumpForceWallVert  { get; set; } = 500;

	// IEntityJumpable
	public int JumpCount          { get; set; }
	public  bool InWallJumpArea { get; set; }
	public  int  WallDir        { get; set; }

	// IEntityGroundJumpable
	public int JumpForce          { get; set; } = 600;

	// IEntityMovement
	public int GroundAcceleration { get; set; } = 50;

	// IEntityMoveable
	public  Window     Tree          { get; set; }

	// IEntityDash
	public bool CurrentlyDashing  { get; set; }

	// IEntityAnimation
	public AnimatedSprite2D AnimatedSprite { get; set; }

	// Not in a interface
	public  bool HaltPlayerLogic { get; set; }
	public  GTimer TimerNetSend      { get; set; }
	public  Node2D ParentWallChecksLeft     { get; set; }
	public  Node2D ParentWallChecksRight    { get; set; }
	public  Node2D ParentGroundChecks       { get; set; }
	public  LevelScene LevelScene { get; set; }
	public  Vector2 PrevNetPos { get; set; }
	public MovementInput PlayerInput { get; set; }
	public int MaxJumps           { get; set; } = 1;
	public int HorizontalDeadZone { get; set; } = 25;
	public GTween DieTween       { get; set; }
	public  bool TouchedGround { get; set; }

	public void PreInit(LevelScene levelScene)
	{
		LevelScene = levelScene;
	}

	public GTimer DontCheckPlatformAfterDashDuration { get; set; }

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
		DontCheckPlatformAfterDashDuration = new GTimer(this, 500, false, false);

		PrepareRaycasts(ParentWallChecksLeft, RayCast2DWallChecksLeft);
		PrepareRaycasts(ParentWallChecksRight, RayCast2DWallChecksRight);
		PrepareRaycasts(ParentGroundChecks, RayCast2DGroundChecks);

		base._Ready(); // there are some things in base._Ready() that require to go after everything above
	}

	public override void _PhysicsProcess(double delta)
	{
		PlayerInput = MovementUtils.GetPlayerMovementInput(); // PlayerInput = ... needs to go before base._PhysicsProcess(delta)
		
		base._PhysicsProcess(delta);

		if (HaltPlayerLogic)
			return;

		UpdateMoveDirection(PlayerInput);

		if (!CurrentlyDashing && !DontCheckPlatformAfterDashDuration.IsActive())
			UpdateUnderPlatform(PlayerInput);

		// jump is handled before all movement restrictions
		if (PlayerInput.IsJump)
		{
			if (WallDir != 0 && !IsOnGround()) // Wall jump
			{
				Commands[EntityCommandType.WallJump].Start();
			}
			else if (JumpCount < MaxJumps) 
			{
				if (IsOnGround()) // Ground jump
				{
					Commands[EntityCommandType.GroundJump].Start();
				}
				else // Mid air jump
				{

				}
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

		// reset jump count whenever the player is falling
		if (IsFalling())
			JumpCount = 0;
	}

	public override void UpdateAir()
	{
		if (PlayerInput.IsFastFall)
			Velocity = Velocity + new Vector2(0, 10);
	}

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
				tilemap.EnableLayers(2);
				await Task.Delay(1000);
				tilemap.EnableLayers(1, 2);
			}
		}
	}

	private void UpdateMoveDirection(MovementInput input)
	{
		var x = -Convert.ToInt32(input.IsLeft) + Convert.ToInt32(input.IsRight);
		var y = -Convert.ToInt32(input.IsUp) + Convert.ToInt32(input.IsDown);

		MoveDir = new Vector2(x, y);
	}

	public bool IsFalling() => Velocity.y > 0;

	private void PrepareRaycasts(Node parent, List<RayCast2D> list)
	{
		foreach (RayCast2D raycast in parent.GetChildren())
		{
			raycast.AddException(this);
			list.Add(raycast);
		}
	}

	// Checks from which side the collision occured. -1 if is on the left, 1 on the right, 0 if neither
	public int GetCollisionSide(Area2D area)
	{
		if (this.GlobalPosition.x < area.GlobalPosition.x)
			return -1;
		else if (this.GlobalPosition.x > area.GlobalPosition.x)
			return 1;

		return 0;
	}

	public void TakenDamage(int side, int damage)
	{
		// enemy has no idea what players health is, don't kill the player when their health is below or equal to zero
		if (GameManager.LevelUI.Health <= 0)
			return;

		if (!GameManager.LevelUI.RemoveHealth(damage))
			Died();
		else
		{
			Vector2 velocity;
			Commands[EntityCommandType.Dash].Stop();

			velocity.y = -JumpForce * 0.5f; // make y and x jumps less aggressive
			velocity.x = side * JumpForce * 0.5f;
			Velocity = velocity;
		}
	}

	public void Died()
	{
		GameManager.EventsPlayer.Notify(EventPlayer.OnDied);
		
		HaltPlayerLogic = true;
		LevelScene.Camera.StopFollowingPlayer();
		AnimatedSprite.Stop();

		var dieStartPos = Position.y;
		var goUpDuration = 1.25f;

		// animate y position
		DieTween.InterpolateProperty
		(
			"position:y",
			dieStartPos - 80,
			goUpDuration,
			0 // delay
		);

		DieTween.InterpolateProperty
		(
			"position:y",
			dieStartPos + 400,
			1.5f,
			goUpDuration, // delay
			true
		)
		.From(dieStartPos - 80);

		// animate rotation
		DieTween.InterpolateProperty
		(
			"rotation",
			Mathf.Pi,
			1.5f,
			goUpDuration, // delay
			true
		);

		DieTween.Start();
		DieTween.Callback(() => OnDieTweenCompleted());
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
		HaltPlayerLogic = false;
		LevelManager.LoadLevelFast();
		LevelScene.Camera.StartFollowingPlayer();
	}

	public async Task FinishedLevel()
	{
		HaltPlayerLogic = true;
		await LevelManager.CompleteLevel(LevelManager.CurrentLevel);
		HaltPlayerLogic = false;
	}

	private async void _on_Player_Area_area_entered(Area2D area)
	{
		if (HaltPlayerLogic)
			return;

		if (area.IsInGroup("Killzone"))
		{
			Died();
			return;
		}

		if (area.IsInGroup("Level Finish"))
		{
			await FinishedLevel();
			return;
		}

		if (area.IsInGroup("WallJumpArea"))
			InWallJumpArea = true;
	}

	private void _on_Area_area_exited(Area2D area)
	{
		if (area.IsInGroup("WallJumpArea"))
			InWallJumpArea = false;
	}
}
