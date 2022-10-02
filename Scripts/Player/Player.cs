namespace Sankari;

interface IPlayerSkills : IEntityDashable, IEntityWallJumpable { }

public partial class Player : CharacterBody2D, IPlayerSkills
{
	[Export] protected NodePath NodePathRayCast2DWallChecksLeft  { get; set; }
	[Export] protected NodePath NodePathRayCast2DWallChecksRight { get; set; }
	[Export] protected NodePath NodePathRayCast2DGroundChecks    { get; set; }

	public static Vector2 RespawnPosition      { get; set; }
	public static bool    HasTouchedCheckpoint { get; set; }
	public static Player  Instance             { get; set; }

	public int UniversalForceModifier { get; set; } = 4;
	public int SpeedGround            { get; set; } = 60;
	public int SpeedAir               { get; set; }	= 16;
	public int SpeedMaxGround         { get; set; }	= 300;
	public int SpeedMaxGroundSprint   { get; set; }	= 400;
	public int SpeedMaxAir            { get; set; }	= 900;
	public int SpeedDashVertical      { get; set; }	= 400;
	public int SpeedDashHorizontal    { get; set; }	= 600;
	public int GravityAir             { get; set; }	= 1400;
	public int GravityWall            { get; set; }	= 3000;
	public int JumpForce              { get; set; }	= 600;
	public int JumpForceWallVert      { get; set; }	= 600;
	public int JumpForceWallHorz      { get; set; }	= 300;

	// dependecy injcetion
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

	private PlayerCommand[] PlayerCommands { get; set; }

	public void PreInit(LevelScene levelScene)
	{
		LevelScene = levelScene;
	}

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

		PlayerCommands = new PlayerCommand[2]
		{
			new PlayerCommandDash(this),
			new PlayerCommandWallJumps(this)
		};

		foreach (var command in PlayerCommands)
			command.Initialize();
	}

	public override void _PhysicsProcess(double d)
	{
		var delta = (float)d;

		if (HaltPlayerLogic)
			return;

		UpdateMoveDirection();
		HandleMovement(delta);
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

	private void HandleMovement(float delta)
	{
		MovementInput input = MovementUtils.GetMovementInput();
		WallDir = UpdateWallDirection();

		UpdateUnderPlatform(input);

		foreach (var command in PlayerCommands)
			command.Update(input);

		HandleGravityState(delta, input);

		foreach (var command in PlayerCommands)
			command.LateUpdate(input);

		// Finish up animiations
		if (IsFalling())
			AnimatedSprite.Play("jump_fall");

		// Flip Sprint if we are moving negatively
		AnimatedSprite.FlipH = MoveDir.x < 0;
		MoveAndSlide();
	}

	private void HandleGravityState(float delta, MovementInput input)
	{
		Vector2 velocity = Velocity;

		if (IsOnGround())
		{
			if (!TouchedGround)
			{
				TouchedGround = true;
				velocity.y = 0;
			}

			if (MoveDir.x != 0)
				AnimatedSprite.Play("walk");
			else
				AnimatedSprite.Play("idle");

			velocity.x += MoveDir.x * SpeedGround;

			velocity.x = HorzDampening(velocity.x, 20);

			if (input.IsJump)
			{
				Jump();
				velocity.y = 0; // reset vertical velocity before jumping
				velocity.y -= JumpForce;
			}
		}
		else
		{
			// apply gravity
			TouchedGround = false;
			velocity.y += GravityAir * delta;

			velocity.x += MoveDir.x * SpeedAir;

			if (input.IsFastFall)
				velocity.y += 10;
		}

		if (IsFalling())
			AnimatedSprite.Play("jump_fall");

		Velocity = velocity;

		MoveAndSlide();
	}

	public void Jump()
	{
		AnimatedSprite.Play("jump_start");
		Audio.PlaySFX("player_jump", 80);
	}

	private int UpdateWallDirection()
	{
		var left = IsTouchingWallLeft();
		var right = IsTouchingWallRight();

		return -Convert.ToInt32(left) + Convert.ToInt32(right);
	}

	private bool IsTouchingWallLeft()
	{
		foreach (var raycast in RayCast2DWallChecksLeft)
			if (raycast.IsColliding())
				return true;

		return false;
	}

	private bool IsTouchingWallRight()
	{
		foreach (var raycast in RayCast2DWallChecksRight)
			if (raycast.IsColliding())
				return true;

		return false;
	}

	private async void UpdateUnderPlatform(MovementInput input)
	{
		var collision = RayCast2DGroundChecks[0].GetCollider(); // seems like were getting this twice, this could be optimized to only be got once in _Ready and made into a private variable

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

	private float HorzDampening(float number, uint dampening)
	{
		// deadzone has to be bigger than dampening value or the player ghost slide effect will occur
		int deadzone = (int)(dampening * 1.5f);

		if (Mathf.Abs(number) < deadzone)
		{
			return 0;
		}
		else if (number > deadzone)
		{
			return number - deadzone;
		}
		else if (number < deadzone)
		{
			return number + dampening;
		}

		return 0;
	}

	public bool IsOnGround()
	{
		foreach (var raycast in RayCast2DGroundChecks)
			if (raycast.IsColliding())
				return true;

		return false;
	}

	public bool IsFalling() => base.Velocity.y > 0;

	private void UpdateMoveDirection()
	{
		var x = -Convert.ToInt32(Input.IsActionPressed("player_move_left")) + Convert.ToInt32(Input.IsActionPressed("player_move_right"));
		var y = Input.IsActionPressed("player_jump") ? 1 : 0;

		MoveDir = new Vector2(x, y);
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
		HaltPlayerLogic = true;
		AnimatedSprite.Stop();
		LevelScene.Camera.StopFollowingPlayer();

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
		Audio.StopMusic();
		Audio.PlaySFX("game_over_1");

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
			HaltPlayerLogic = true;
			await LevelManager.CompleteLevel(LevelManager.CurrentLevel);
			HaltPlayerLogic = false;
			return;
		}

		if (area.IsInGroup("Enemy"))
		{
			Died();
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
