namespace Sankari;

public partial class Player : CharacterBody2D
{
	[Export] protected NodePath NodePathRayCast2DWallChecksLeft  { get; set; }
	[Export] protected NodePath NodePathRayCast2DWallChecksRight { get; set; }
	[Export] protected NodePath NodePathRayCast2DGroundChecks    { get; set; }

	public static Vector2 RespawnPosition      { get; set; }
	public static bool    HasTouchedCheckpoint { get; set; }
	public static Player  Instance             { get; set; }

	public int UniversalForceModifier { get; set; }
	public int SpeedGround            { get; set; }
	public int SpeedAir               { get; set; }
	public int SpeedMaxGround         { get; set; }
	public int SpeedMaxGroundSprint   { get; set; }
	public int SpeedMaxAir            { get; set; }
	public int SpeedDashVertical      { get; set; }
	public int SpeedDashHorizontal    { get; set; }
	public int GravityAir             { get; set; }
	public int GravityWall            { get; set; }
	public int JumpForce              { get; set; }
	public int JumpForceWallVert      { get; set; }
	public int JumpForceWallHorz      { get; set; }
	public int DashCooldown           { get; set; }
	public int DashDuration           { get; set; }

	// dependecy injcetion
	public  LevelScene LevelScene { get; set; }

	// movement
	public  Vector2 PrevNetPos { get; set; }
	public  Vector2 MoveDir    { get; set; }

	public  bool HaltPlayerLogic { get; set; }

	// timers
	public  GTimer TimerDashCooldown { get; set; }
	public  GTimer TimerDashDuration { get; set; }
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
	public bool    DashReady        { get; set; } = true;
	public  Window     Tree          { get; set; }
	public  bool       TouchedGround { get; set; }
	public bool    CurrentlyDashing { get; set; }
	private PlayerCommandDash PlayerDash    { get; set; } = new();
	private PlayerCommandWallJumps PlayerWallJumps { get; set; } = new();
	private PlayerCommand[] PlayerCommands { get; set; } = new PlayerCommand[2] 
	{
		new PlayerCommandDash(),
		new PlayerCommandWallJumps()
	};

	// fields
	public Vector2 velocityPlayer;

	public bool InputJump     { get; private set; }
	public bool InputUp       { get; private set; }
	public bool InputDown     { get; private set; }
	public bool InputFastFall { get; private set; }
	public bool InputDash     { get; private set; }
	public bool InputSprint   { get; private set; }

	public void PreInit(LevelScene levelScene)
	{
		this.LevelScene = levelScene;
	}

	public Player()
	{
		UniversalForceModifier = 4;
		SpeedGround            = 15 * UniversalForceModifier;
		SpeedAir               = 4 * UniversalForceModifier;
		SpeedMaxGround         = 75 * UniversalForceModifier;
		SpeedMaxGroundSprint   = 100 * UniversalForceModifier;
		SpeedMaxAir            = 225 * UniversalForceModifier;
		SpeedDashVertical      = 100 * UniversalForceModifier;
		SpeedDashHorizontal    = 150 * UniversalForceModifier;
		GravityAir             = 350 * UniversalForceModifier;
		GravityWall            = 750 * UniversalForceModifier;
		JumpForce              = 150 * UniversalForceModifier;
		JumpForceWallVert      = 150 * UniversalForceModifier;
		JumpForceWallHorz      = 75 * UniversalForceModifier;
		DashCooldown           = 350;
		DashDuration           = 200;
	}

	public override void _Ready()
	{
		Instance = this;

		if (HasTouchedCheckpoint)
			Position = RespawnPosition;

		TimerDashCooldown     = new GTimer(this, nameof(OnDashReady), DashCooldown, false, false);
		TimerDashDuration     = new GTimer(this, nameof(OnDashDurationDone), DashDuration, false, false);
		TimerNetSend          = new GTimer(this, nameof(NetUpdate), NetIntervals.HEARTBEAT, true, Net.IsMultiplayer());
		ParentGroundChecks    = GetNode<Node2D>(NodePathRayCast2DGroundChecks);
		ParentWallChecksLeft  = GetNode<Node2D>(NodePathRayCast2DWallChecksLeft);
		ParentWallChecksRight = GetNode<Node2D>(NodePathRayCast2DWallChecksRight);
		AnimatedSprite        = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		DieTween              = new GTween(this);
		Tree                  = GetTree().Root;

		PrepareRaycasts(ParentWallChecksLeft, RayCast2DWallChecksLeft);
		PrepareRaycasts(ParentWallChecksRight, RayCast2DWallChecksRight);
		PrepareRaycasts(ParentGroundChecks, RayCast2DGroundChecks);

		AnimatedSprite.Play("idle");

		UpDirection = Vector2.Up;

		FloorConstantSpeed = false; // this messes up downward slope velocity if set to true
		FloorStopOnSlope = false;   // players should slide on slopes
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
		InputJump     = Input.IsActionJustPressed("player_jump");
		InputUp       = Input.IsActionPressed("player_move_up");
		InputDown     = Input.IsActionPressed("player_move_down");
		InputFastFall = Input.IsActionPressed("player_fast_fall");
		InputDash     = Input.IsActionJustPressed("player_dash");
		InputSprint   = Input.IsActionPressed("player_sprint");

		CheckIfCanGoUnderPlatform(InputDown);

		foreach (var command in PlayerCommands)
			command.Update(this);

		AnimatedSprite.FlipH = MoveDir.x < 0;

		if (IsOnGround())
		{
			if (!TouchedGround)
			{
				TouchedGround = true;	
				velocityPlayer.y = 0;
			}

			if (MoveDir.x != 0)
				AnimatedSprite.Play("walk");
			else
				AnimatedSprite.Play("idle");

			velocityPlayer.x += MoveDir.x * SpeedGround;

			HorzDampening(20);

			if (InputJump)
			{
				Jump();
				velocityPlayer.y = 0; // reset vertical velocity before jumping
				velocityPlayer.y -= JumpForce;
			}
		}
		else
		{
			// apply gravity
			TouchedGround = false;
			velocityPlayer.y += GravityAir * delta;

			velocityPlayer.x += MoveDir.x * SpeedAir;

			if (InputFastFall)
				velocityPlayer.y += 10;
		}

		if (IsFalling())
			AnimatedSprite.Play("jump_fall");

		if (!CurrentlyDashing)
		{
			if (IsOnGround() && InputSprint)
			{
				AnimatedSprite.SpeedScale = 1.5f;
				velocityPlayer.x = Mathf.Clamp(velocityPlayer.x, -SpeedMaxGroundSprint, SpeedMaxGroundSprint);
			}
			else
			{
				AnimatedSprite.SpeedScale = 1;
				velocityPlayer.x = Mathf.Clamp(velocityPlayer.x, -SpeedMaxGround, SpeedMaxGround);
			}

			velocityPlayer.y = Mathf.Clamp(velocityPlayer.y, -SpeedMaxAir, SpeedMaxAir);
		}

		Velocity = velocityPlayer;

		MoveAndSlide();
	}

	public void Jump()
	{
		AnimatedSprite.Play("jump_start");
		Audio.PlaySFX("player_jump", 80);
	}

	private async void CheckIfCanGoUnderPlatform(bool inputDown)
	{
		var collision = RayCast2DGroundChecks[0].GetCollider(); // seems like were getting this twice, this could be optimized to only be got once in _Ready and made into a private variable

		if (collision != null && collision is TileMap tilemap)
		{
			if (inputDown && tilemap.IsInGroup("Platform"))
			{
				tilemap.EnableLayers(2);
				await Task.Delay(1000);
				tilemap.EnableLayers(1, 2);
			}
		}
	}

	private void HorzDampening(int dampening)
	{
		// deadzone has to be bigger than dampening value or the player ghost slide effect will occur
		int deadzone = (int)(dampening * 1.5f);

		if (velocityPlayer.x >= -deadzone && velocityPlayer.x <= deadzone)
		{
			velocityPlayer.x = 0;
		}
		else if (velocityPlayer.x > deadzone)
		{
			velocityPlayer.x -= dampening;
		}
		else if (velocityPlayer.x < deadzone)
		{
			velocityPlayer.x += dampening;
		}
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

	private void OnDashReady() => DashReady = true;
	private void OnDashDurationDone() => CurrentlyDashing = false;

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
