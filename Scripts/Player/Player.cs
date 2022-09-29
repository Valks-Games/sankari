namespace Sankari;

public partial class Player : CharacterBody2D
{
	[Export] protected NodePath NodePathRayCast2DWallChecksLeft { get; set; }
	[Export] protected NodePath NodePathRayCast2DWallChecksRight { get; set; }
	[Export] protected NodePath NodePathRayCast2DGroundChecks { get; set; }

	public static Vector2 RespawnPosition { get; set; }
	public static bool HasTouchedCheckpoint { get; set; }
	public static Player Instance { get; set; }

	private int UniversalForceModifier { get; set; }
	private int SpeedGround { get; set; }
	private int SpeedAir { get; set; }
	private int SpeedMaxGround { get; set; }
	private int SpeedMaxGroundSprint { get; set; }
	private int SpeedMaxAir { get; set; }
	private int SpeedDashVertical { get; set; }
	private int SpeedDashHorizontal { get; set; }
	private int GravityAir { get; set; }
	private int GravityWall { get; set; }
	private int JumpForce { get; set; }
	private int JumpForceWallVert { get; set; }
	private int JumpForceWallHorz { get; set; }
	private int DashCooldown { get; set; }
	private int DashDuration { get; set; }

	// dependecy injcetion
	private LevelScene LevelScene { get; set; }

	// movement
	private Vector2 PrevNetPos { get; set; }
	private Vector2 MoveDir { get; set; }

	private bool HaltPlayerLogic { get; set; }

	// timers
	private GTimer TimerDashCooldown { get; set; }
	private GTimer TimerDashDuration { get; set; }
	private GTimer TimerNetSend { get; set; }

	// raycasts
	private Node2D ParentWallChecksLeft { get; set; }
	private Node2D ParentWallChecksRight { get; set; }
	private List<RayCast2D> RayCast2DWallChecksLeft { get; } = new();
	private List<RayCast2D> RayCast2DWallChecksRight { get; } = new();
	private List<RayCast2D> RayCast2DGroundChecks { get; } = new();
	private Node2D ParentGroundChecks { get; set; }

	// animation
	private AnimatedSprite2D AnimatedSprite { get; set; }
	private GTween DieTween { get; set; }

	// wall
	private bool InWallJumpArea { get; set; }
	private int WallDir { get; set; }

	// dash
	private Vector2 DashDir { get; set; }
	private int MaxDashes { get; set; } = 1;
	private int DashCount { get; set; }
	private bool HorizontalDash { get; set; }
	private bool DashReady { get; set; } = true;
	private bool CurrentlyDashing { get; set; }

	// msc
	private Window Tree { get; set; }

	// fields
	private Vector2 velocityPlayer;

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
		TimerNetSend          = new GTimer(this, nameof(NetUpdate), NetIntervals.HEARTBEAT, true, GameManager.Net.IsMultiplayer());
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
			GameManager.Net.Client.Send(ClientPacketOpcode.PlayerPosition, new CPacketPlayerPosition
			{
				Position = Position
			});

		PrevNetPos = Position;
	}

	private void HandleMovement(float delta)
	{
		var inputJump     = Input.IsActionJustPressed("player_jump");
		var inputUp       = Input.IsActionPressed("player_move_up");
		var inputDown     = Input.IsActionPressed("player_move_down");
		var inputFastFall = Input.IsActionPressed("player_fast_fall");
		var inputDash     = Input.IsActionJustPressed("player_dash");
		var inputSprint   = Input.IsActionPressed("player_sprint");

		WallDir = UpdateWallDirection();

		// on a wall and falling
		if (WallDir != 0 && InWallJumpArea)
		{
			AnimatedSprite.FlipH = WallDir == 1;

			if (IsFalling())
			{
				velocityPlayer.y = 0;

				// fast fall
				if (inputDown)
					velocityPlayer.y += 50;

				// wall jump
				if (inputJump)
				{
					Jump();
					velocityPlayer.x += -JumpForceWallHorz * WallDir;
					velocityPlayer.y -= JumpForceWallVert;
				}
			}
		}
		else
		{
			AnimatedSprite.FlipH = false;
		}

		CheckIfCanGoUnderPlatform(inputDown);

		// dash
		if (inputDash && DashReady && !CurrentlyDashing && DashCount != MaxDashes && !IsOnGround())
		{
			DashDir = GetDashDirection(inputUp, inputDown);

			if (DashDir != Vector2.Zero)
			{
				DashCount++;
				Audio.PlaySFX("dash");
				DashReady = false;
				CurrentlyDashing = true;
				TimerDashDuration.Start();
				TimerDashCooldown.Start();
			}
		}

		if (CurrentlyDashing)
		{
			DoDashStuff();
		}

		AnimatedSprite.FlipH = MoveDir.x < 0;

		if (IsOnGround())
		{
			DashCount = 0;

			if (MoveDir.x != 0)
				AnimatedSprite.Play("walk");
			else
				AnimatedSprite.Play("idle");

			velocityPlayer.x += MoveDir.x * SpeedGround;

			HorzDampening(20);

			if (inputJump)
			{
				Jump();
				velocityPlayer.y = 0; // reset vertical velocity before jumping
				velocityPlayer.y -= JumpForce;
			}
		}
		else
		{
			velocityPlayer.x += MoveDir.x * SpeedAir;

			if (inputFastFall)
				velocityPlayer.y += 10;
		}

		if (IsFalling())
			AnimatedSprite.Play("jump_fall");

		// apply gravity
		velocityPlayer.y += GravityAir * delta;

		if (!CurrentlyDashing)
		{
			if (IsOnGround() && inputSprint)
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

	private void Jump()
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

	private void DoDashStuff()
	{
		var sprite = Prefabs.PlayerDashTrace.Instantiate<Sprite2D>();
		sprite.Texture = AnimatedSprite.Frames.GetFrame(AnimatedSprite.Animation, AnimatedSprite.Frame);
		sprite.GlobalPosition = GlobalPosition;
		sprite.Scale = new Vector2(2f, 2f);
		sprite.FlipH = AnimatedSprite.FlipH;
		//sprite.FlipH = wallDir == 1 ? true : false;
		Tree.AddChild(sprite);

		var dashSpeed = SpeedDashVertical;

		if (HorizontalDash)
			dashSpeed = SpeedDashHorizontal;

		velocityPlayer = DashDir * dashSpeed;
	}

	private Vector2 GetDashDirection(bool inputUp, bool inputDown)
	{
		if (inputDown && MoveDir.x < 0)
		{
			return new Vector2(-1, 1);
		}
		else if (inputDown && MoveDir.x == 0)
		{
			HorizontalDash = false;
			return new Vector2(0, 1);
		}
		else if (inputDown && MoveDir.x > 0)
		{
			return new Vector2(1, 1);
		}
		else if (inputUp && MoveDir.x < 0)
		{
			HorizontalDash = false;
			return new Vector2(-1, -1);
		}
		else if (inputUp && MoveDir.x > 0)
		{
			HorizontalDash = false;
			return new Vector2(1, -1);
		}
		else if (inputUp)
		{
			HorizontalDash = false;
			return new Vector2(0, -1);
		}
		else if (MoveDir.x < 0)
		{
			HorizontalDash = true;
			return new Vector2(-1, 0);
		}
		else if (MoveDir.x > 0)
		{
			HorizontalDash = true;
			return new Vector2(1, 0);
		}

		return Vector2.Zero;
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

	private bool IsOnGround()
	{
		foreach (var raycast in RayCast2DGroundChecks)
			if (raycast.IsColliding())
				return true;

		return false;
	}

	private bool IsFalling() => base.Velocity.y > 0;

	private void UpdateMoveDirection()
	{
		var x = -Convert.ToInt32(Input.IsActionPressed("player_move_left")) + Convert.ToInt32(Input.IsActionPressed("player_move_right"));
		var y = Input.IsActionPressed("player_jump") ? 1 : 0;

		MoveDir = new Vector2(x, y);
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
		AnimatedSprite.Stop();
		LevelScene.Camera.StopFollowingPlayer();

		var dieStartPos = Position.y;

		// animate y position
		DieTween.InterpolateProperty
		(
			"position:y",
			dieStartPos - 80,
			0.75f
		);

		DieTween.InterpolateProperty
		(
			"position:y",
			dieStartPos + 600,
			1f
		);

		// animate rotation
		DieTween.InterpolateProperty
		(
			"rotation",
			0,
			160
		);

		DieTween.Start();
		HaltPlayerLogic = true;
		Audio.StopMusic();
		Audio.PlaySFX("game_over_1");

		DieTween.Callback(() => OnDieTweenCompleted());
	}

	private async void OnDieTweenCompleted()
	{
		Logger.Log("WE ARE HERE");
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
		GameManager.Level.LoadLevelFast();
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
			await GameManager.Level.CompleteLevel(GameManager.Level.CurrentLevel);
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
