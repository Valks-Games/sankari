using Godot;

namespace Sankari;

public interface IPlayerAnimations : IEntityAnimationDash, IEntityAnimation
{ }

public interface IPlayerCommands : IEntityDash, IEntityWallJumpable, IEntityMovement
{ }

public partial class Player : CharacterBody2D, IPlayerAnimations, IPlayerCommands
{
	private enum PlayerCommandType
	{
		Animation,
		Dash,
		Movement,
		WallJump
	}

	[Export] protected NodePath NodePathRayCast2DWallChecksLeft  { get; set; }
	[Export] protected NodePath NodePathRayCast2DWallChecksRight { get; set; }
	[Export] protected NodePath NodePathRayCast2DGroundChecks    { get; set; }

	public static Vector2 RespawnPosition      { get; set; }
	public static bool    HasTouchedCheckpoint { get; set; }
	public static Player  Instance             { get; set; }

	// dependency injection
	public  LevelScene LevelScene { get; set; }

	// movement
	public  Vector2 PrevNetPos { get; set; }
	public  Vector2 MoveDir    { get; set; }

	public  bool HaltPlayerLogic { get; set; }

	// timers
	public  GTimer TimerNetSend      { get; set; }

	// raycasts
	public  Node2D          ParentWallChecksLeft     { get; set; }
	public  Node2D          ParentWallChecksRight    { get; set; }
	public  List<RayCast2D> RayCast2DWallChecksLeft  { get; } = new();
	public  List<RayCast2D> RayCast2DWallChecksRight { get; } = new();
	public  List<RayCast2D> RayCast2DGroundChecks    { get; } = new();
	public  Node2D          ParentGroundChecks       { get; set; }

	public MovementInput PlayerInput { get; set; }

	public int JumpCount          { get; set; }
	public int JumpForce          { get; set; } = 600;
	public int Gravity            { get; set; } = 1200;
	public int MaxJumps           { get; set; } = 1;
	public int GroundAcceleration { get; set; } = 50;
	public int HorizontalDeadZone { get; set; } = 25;

	public PlayerAnimationState AnimationState { get; set; }

	public EntityAnimation CurrentAnimation { get; set; }
	public EntityAnimationIdle AnimationIdle { get; set; }
	public EntityAnimationWalking AnimationWalking { get; set; }
	public EntityAnimationRunning AnimationRunning { get; set; }
	public EntityAnimationDash AnimationDash { get; set; }
	public EntityAnimationJumpStart AnimationJumpStart { get; set; }
	public EntityAnimationJumpFall AnimationJumpFall { get; set; }

	public bool CurrentlyDashing  { get; set; }
	public bool GravityEnabled    { get; set; } = true;

	// animation
	public AnimatedSprite2D AnimatedSprite { get; set; }
	public GTween           DieTween       { get; set; }

	// wall
	public  bool InWallJumpArea { get; set; }
	public  int  WallDir        { get; set; }

	// msc
	public  Window     Tree          { get; set; }
	public  bool       TouchedGround { get; set; }

	public GTimers Timers { get; set; }

	public void PreInit(LevelScene levelScene)
	{
		LevelScene = levelScene;
	}

	private Dictionary<PlayerCommandType, EntityCommand> PlayerCommands { get; set; }
	public GTimer DontCheckPlatformAfterDashDuration { get; set; }

	public override void _Ready()
	{
		Instance = this;

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

		// The up direction must be defined in order for the FloorSnapLength
		// to work properly. A direction of up means gravity goes down and
		// the player jumps up
		UpDirection = Vector2.Up;

		// Prevents the player from bouncing when going down a slope
		FloorSnapLength = 10;

		// Sets the speed to be constant no matter the angle of terrain the
		// player is on. This means the player will walk the same speed on a
		// flat surface and a slope
		FloorConstantSpeed = true;

		// If true, the body will not slide on slopes when calling move_and_slide
		// when the body is standing still.
        // If false, the body will slide on floor's slopes when velocity applies
		// a downward force.
		// Does not seem to have any effect if this is either true or false
		FloorStopOnSlope = false;

		// If true, the body will be able to move on the floor only. This
		// option avoids to be able to walk on walls, it will however allow
		// to slide down along them.
		// Does not seem to have any effect if this is either true or false
		FloorBlockOnWall = true;

		// If true, during a jump against the ceiling, the body will slide,
		// if false it will be stopped and will fall vertically.
		// Does not seem to have any effect if this is either true or false
		SlideOnCeiling = true;

		PlayerCommands = new Dictionary<PlayerCommandType, EntityCommand>
		{
			{ PlayerCommandType.Movement , new EntityCommandMovement(this)  },
			{ PlayerCommandType.Dash     , new EntityCommandDash(this)      },
			{ PlayerCommandType.WallJump , new EntityCommandWallJump(this)  }
		};

		PlayerCommands.Values.ForEach(cmd => cmd.Initialize());
	}

	public override void _PhysicsProcess(double d)
	{
		if (HaltPlayerLogic)
			return;

		var delta = (float)d;

		PlayerInput = MovementUtils.GetPlayerMovementInput();

		UpdateMoveDirection(PlayerInput);

		PlayerCommands.Values.ForEach(cmd => cmd.Update(delta));

		if (!CurrentlyDashing && !DontCheckPlatformAfterDashDuration.IsActive())
			UpdateUnderPlatform(PlayerInput);

		// jump is handled before all movement restrictions
		if (PlayerInput.IsJump)
		{
			if (WallDir != 0)
			{
				//PlayerCommands.Values.ForEach(cmd => cmd.Jump());
				GameManager.EventsPlayer.Notify(EventPlayer.OnJump);
				PlayerCommands[PlayerCommandType.WallJump].Start();
			}
			else if (JumpCount < MaxJumps)
			{
				//PlayerCommands.Values.ForEach(cmd => cmd.Jump());
				GameManager.EventsPlayer.Notify(EventPlayer.OnJump);
				JumpCount++;
				//Velocity = new Vector2(Velocity.x, 0); // reset velocity before jump (is this really needed?)
				Velocity = Velocity - new Vector2(0, JumpForce);
			}
		}

		if (PlayerInput.IsDash)
			PlayerCommands[PlayerCommandType.Dash].Start();

		// gravity
		if (GravityEnabled)
			Velocity = Velocity + new Vector2(0, Gravity * delta);

		if (IsOnGround()) // ground
		{
			Velocity = Velocity + new Vector2(MoveDir.x * GroundAcceleration, 0);

			if (PlayerInput.IsSprint)
				PlayerCommands.Values.ForEach(cmd => cmd.UpdateGroundSprinting(delta));
			else
				PlayerCommands.Values.ForEach(cmd => cmd.UpdateGroundWalking(delta));

			// do not reset jump count when the player is leaving the ground for the first time
			if (Velocity.y > 0)
				JumpCount = 0;
		}
		else // air
		{
			if (PlayerInput.IsFastFall)
				Velocity = Velocity + new Vector2(0, 10);

			PlayerCommands.Values.ForEach(cmd => cmd.UpdateAir(delta));
		}

		Velocity = new Vector2(MoveDeadZone(Velocity.x, HorizontalDeadZone), Velocity.y); // must be after ClampAndDampen(...)

		MoveAndSlide();
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

	public bool IsOnGround()
	{
		foreach (var raycast in RayCast2DGroundChecks)
			if (raycast.IsColliding())
				return true;

		return false;
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

	//Checks from which side the collision occured. -1 if is on the left, 1 on the right, 0 if neither
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
		if (!GameManager.LevelUI.RemoveHealth(damage))
			Died();
		else
		{
			Vector2 velocity;
			//PlayerCommandsOld.CommandDash.Stop();

			velocity.y = -JumpForce * 0.5f; // make y and x jumps less aggressive
			velocity.x = side * JumpForce * 0.5f;
			Velocity = velocity;
		}
	}

	public void Died()
	{
		GameManager.EventsPlayer.Notify(EventPlayer.OnDied);
		//PlayerCommands.Values.ForEach(cmd => cmd.Died());
	}

	private void _on_Player_Area_area_entered(Area2D area)
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
			//PlayerCommands.Values.ForEach(cmd => cmd.FinishedLevel());
			return;
		}

		if (area.IsInGroup("Enemy"))
		{
			//PlayerCommands.Values.ForEach(cmd => cmd.TouchedEnemy());
			TakenDamage(GetCollisionSide(area), 1);
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
