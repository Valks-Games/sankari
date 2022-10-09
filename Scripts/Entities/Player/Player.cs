using Godot;

namespace Sankari;

public interface IPlayerSkills : IEntityDashable, IEntityWallJumpable { }

public partial class Player : CharacterBody2D
{
	private class PlayerSkills
	{
		public EntityCommandDash CommandDash { get; set; }
		public EntityCommandWallJumps CommandWallJump { get; set; }

		public EntityCommand[] GetCommands()
		{
			return new EntityCommand[2] { CommandDash, CommandWallJump };
		}
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
	public int JumpForce          { get; set; } = 700;
	public int Gravity            { get; set; } = 1000;
	public int MaxJumps           { get; set; } = 1;
	public int GroundAcceleration { get; set; } = 50;
	public int HorizontalDeadZone { get; set; } = 25;


	public Vector2 PlayerVelocity; // this was made as a field intentionally

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

	private PlayerCommand[] PlayerCommands { get; set; }

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

		PrepareRaycasts(ParentWallChecksLeft, RayCast2DWallChecksLeft);
		PrepareRaycasts(ParentWallChecksRight, RayCast2DWallChecksRight);
		PrepareRaycasts(ParentGroundChecks, RayCast2DGroundChecks);

		AnimatedSprite.Play("idle");

		UpDirection = Vector2.Up;

		FloorConstantSpeed = false; // this messes up downward slope velocity if set to true
		FloorStopOnSlope = false;   // players should slide on slopes

		PlayerCommands = new PlayerCommand[3] 
		{
			new PlayerCommandAnimation(this),
			new PlayerCommandMovement(this),
			new PlayerCommandAudio(this)
		};

		PlayerCommands.ForEach(cmd => cmd.Init());
	}

	public override void _PhysicsProcess(double d)
	{
		if (HaltPlayerLogic)
			return;

		var delta = (float)d;

		PlayerInput = MovementUtils.GetPlayerMovementInput();

		// Velocity is a property struct, so it needs to be turned into a field to be modifiable
		PlayerVelocity = Velocity;

		PlayerCommands.ForEach(cmd => cmd.Update(delta));

		// jump is handled before all movement restrictions
		if (PlayerInput.IsJump && JumpCount < MaxJumps)
			PlayerCommands.ForEach(cmd => cmd.Jump());

		// gravity
		PlayerVelocity.y += Gravity * delta;
		
		if (IsOnGround()) // ground
		{
			PlayerVelocity.x += MoveDir.x * GroundAcceleration;

			if (PlayerInput.IsSprint)
				PlayerCommands.ForEach(cmd => cmd.UpdateGroundSprinting());
			else
				PlayerCommands.ForEach(cmd => cmd.UpdateGroundWalking());
			
			// do not reset jump count when the player is leaving the ground for the first time
			if (PlayerVelocity.y > 0)
				JumpCount = 0;
		}
		else // air
		{
			PlayerCommands.ForEach(cmd => cmd.UpdateAir());
		}
		
		PlayerVelocity.x = MoveDeadZone(PlayerVelocity.x, HorizontalDeadZone); // must be after ClampAndDampen(...)

		Velocity = PlayerVelocity;

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

	private int UpdateWallDirection()
	{
		var left = CollectionExtensions.IsAnyRayCastColliding(RayCast2DWallChecksLeft);
		var right = CollectionExtensions.IsAnyRayCastColliding(RayCast2DWallChecksRight);

		return -Convert.ToInt32(left) + Convert.ToInt32(right);
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

	public void TakenDamage(int side, float damage)
	{
		if (!GameManager.LevelUI.RemoveHealth(damage))
			Died();
		else
		{
			Vector2 velocity;
			//PlayerCommandsOld.CommandDash.Stop();

			var JumpForce = 500;

			velocity.y = -JumpForce * 0.5f; // make y and x jumps less aggressive
			velocity.x = side * JumpForce * 0.5f;
			Velocity = velocity;
		}
	}

	public void Died() => PlayerCommands.ForEach(cmd => cmd.Died());

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
			PlayerCommands.ForEach(cmd => cmd.FinishedLevel());
			return;
		}

		if (area.IsInGroup("Enemy"))
		{
			PlayerCommands.ForEach(cmd => cmd.TouchedEnemy());
			TakenDamage(GetCollisionSide(area), 0.5f);
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
