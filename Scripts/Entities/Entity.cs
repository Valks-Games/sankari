namespace Sankari;

public abstract partial class Entity : CharacterBody2D
{
	[Export] public bool DontCollideWithWall { get; set; }
	[Export] public bool FallOffCliff { get; set; }
	[Export] public bool Debug { get; set; }

	public Dictionary<EntityCommandType, EntityCommand>     Commands   { get; set; } = new();
	public Dictionary<EntityAnimationType, EntityAnimation> Animations { get; set; } = new();

	public EntityAnimationType CurrentAnimation { get; set; } = EntityAnimationType.None;

	public float           Delta                   { get; protected set; }
	public Vector2         MoveDir                 { get; protected set; }
	public GTimers         Timers                  { get; set; }
	public virtual int     Gravity                 { get; set; } = 1200;
	public bool            GravityEnabled          { get; set; } = true;
	public bool            HaltLogic               { get; set; }
	public virtual int     ModGravityMaxSpeed      { get; set; } = 1200;
	public Node2D          ParentWallChecksLeft    { get; set; }
	public Node2D          ParentWallChecksRight   { get; set; }
	public bool            TouchedGround           { get; set; }
	public Node2D          ParentGroundChecks      { get; set; }
	public int             ImmunityMs              { get; set; } = 500;
	public bool            InDamageZone            { get; set; }
	public int             GroundAcceleration      { get; set; } = 50;

	protected Node ParentRaycastsWallLeft   { get; set; }
	protected Node ParentRaycastsWallRight  { get; set; }
	protected Node ParentRaycastsCliffLeft  { get; set; }
	protected Node ParentRaycastsCliffRight { get; set; }
	protected Node ParentRaycastsGround     { get; set; }

	private List<RayCast2D> RaycastsWallLeft { get; set; } = new();
	private List<RayCast2D> RaycastsWallRight { get; set; } = new();
	private List<RayCast2D> RaycastsCliffLeft { get; set; } = new();
	private List<RayCast2D> RaycastsCliffRight { get; set; } = new();
	protected List<RayCast2D> RaycastsGround { get; set; } = new();

	protected int gravityMaxSpeed = 1200;
	private GTimer immunityTimer;
	private int damageTakenForce = 300;

	sealed public override void _Ready()
	{
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

		immunityTimer = new GTimer(this, nameof(OnImmunityTimerFinished), ImmunityMs, false, false);

		// Setup raycasts
		ParentRaycastsWallLeft = GetNodeOrNull<Node>("Raycasts/Wall/Left");
		ParentRaycastsWallRight = GetNodeOrNull<Node>("Raycasts/Wall/Right");
		ParentRaycastsCliffLeft = GetNodeOrNull<Node>("Raycasts/Cliff/Left");
		ParentRaycastsCliffRight = GetNodeOrNull<Node>("Raycasts/Cliff/Right");
		ParentRaycastsGround = GetNodeOrNull<Node>("Raycasts/Ground");

		PrepareRaycasts(ParentRaycastsWallLeft, RaycastsWallLeft);
		PrepareRaycasts(ParentRaycastsWallRight, RaycastsWallRight);
		PrepareRaycasts(ParentRaycastsCliffLeft, RaycastsCliffLeft);
		PrepareRaycasts(ParentRaycastsCliffRight, RaycastsCliffRight);
		PrepareRaycasts(ParentRaycastsGround, RaycastsGround);

		if (FallOffCliff)
		{
			SetRaycastsEnabled(RaycastsCliffLeft, false);
			SetRaycastsEnabled(RaycastsCliffRight, false);
		}

		if (DontCollideWithWall)
		{
			SetRaycastsEnabled(RaycastsWallLeft, false);
			SetRaycastsEnabled(RaycastsWallRight, false);
		}
		
		Init();
		
		Commands.Values.ForEach(cmd => cmd.Initialize());
		Animations[EntityAnimationType.None] = new EntityAnimationNone();
	}

	sealed public override void _PhysicsProcess(double delta)
	{
		if (HaltLogic)
			return;

		ModGravityMaxSpeed = gravityMaxSpeed;
		Delta = (float)delta;

		UpdatePhysics();

		Animations[CurrentAnimation].UpdateState();
		Animations[CurrentAnimation].HandleStateTransitions();

		Commands.Values.ForEach(cmd => cmd.Update(Delta));

		// gravity
		if (GravityEnabled)
			Velocity = Velocity.MoveToward(new Vector2(0, ModGravityMaxSpeed), Gravity * Delta);

		if (IsNearGround())
			UpdateGround();
		else
		{
			UpdateAir();
			Commands.Values.ForEach(cmd => cmd.UpdateAir(Delta));
		}

		MoveAndSlide();
	}

	/// <summary>
	/// The equivalent to _Ready(), everything here gets called in one frame. All commands
	/// and animations should be setup in here.
	/// </summary>
	public abstract void Init();

	/// <summary>
	/// The equivalent to _UpdatePhysics(float delta), everything here gets called every
	/// frame. Delta is defined in Entity.
	/// </summary>
	public abstract void UpdatePhysics();

	public virtual void Kill() { }

	private void OnImmunityTimerFinished() 
	{
		if (InDamageZone)
			TakenDamage(1, 1); // not sure how to input "side" here
	}

	public void TakenDamage(int side, int damage)
	{
		// enemy has no idea what players health is, don't kill the player when their health is below or equal to zero
		if (GameManager.LevelUI.Health <= 0)
			return;

		if (immunityTimer.IsActive())
			return;
		else
			immunityTimer.Start();

		if (!GameManager.LevelUI.RemoveHealth(damage))
			Kill();
		else
		{
			Vector2 velocity;
			Commands[EntityCommandType.Dash].Stop();

			velocity.y = -damageTakenForce;
			velocity.x = side * damageTakenForce;
			Velocity = velocity;
		}
	}

	public bool IsFalling() => Velocity.y > 0;

	// Checks from which side the collision occurred. -1 if is on the left, 1 on the right, 0 if neither
	public int GetCollisionSide(Area2D area)
	{
		if (this.GlobalPosition.x < area.GlobalPosition.x)
			return -1;
		else if (this.GlobalPosition.x > area.GlobalPosition.x)
			return 1;

		return 0;
	}

	protected bool IsNearWallLeft() => AreRaycastsColliding(RaycastsWallLeft, "Wall Left");
	protected bool IsNearWallRight() => AreRaycastsColliding(RaycastsWallRight, "Wall Right");
	protected bool IsNearCliffLeft() => AreRaycastsColliding(RaycastsCliffLeft, "Cliff Left");
	protected bool IsNearCliffRight() => AreRaycastsColliding(RaycastsCliffRight, "Cliff Right");

	public bool IsNearGround()
	{
		foreach (var raycast in RaycastsGround)
			if (raycast.IsColliding())
				return true;

		return false;
	}

	private bool AreRaycastsColliding(List<RayCast2D> raycasts, string raycastGroup)
	{
		if (raycasts.Count == 0)
		{
			Logger.LogWarning($"Tried to check raycasts for {raycastGroup} but no raycasts were specified for this group");
			return false;
		}

		foreach (var raycast in raycasts)
		{
			var collider = raycast.GetCollider() as Node;

			if (collider != null && collider.IsInGroup("Tileset"))
				return true;
		}

		return false;
	}

	private void SetRaycastsEnabled(List<RayCast2D> raycasts, bool enabled) 
	{
		if (raycasts.Count == 0)
		{
			Logger.LogWarning($"Tried to set raycasts enabled to {enabled} but failed because no raycasts exist");
			return;
		}

		raycasts.ForEach(raycast => raycast.Enabled = enabled);	
	}

	private void PrepareRaycasts(Node raycastsParent, List<RayCast2D> raycastList) 
	{
		if (raycastsParent == null)
			return;

		foreach (RayCast2D raycast in raycastsParent.GetChildren())
		{
			ExcludeRaycastParents(raycast, raycast.GetParent());
			raycastList.Add(raycast);
		}
	}

	/// <summary>
	/// A convience function to tell the raycast to exlude all parents that
	/// are of type CollisionObject2D (for example a ground raycast should
	/// only check for the ground, not the player itself)
	/// </summary>
	private void ExcludeRaycastParents(RayCast2D raycast, Node parent) 
	{
		if (parent != null)
		{
			if (parent is CollisionObject2D collision)
				raycast.AddException(collision);

			ExcludeRaycastParents(raycast, parent.GetParentOrNull<Node>());
		}
	}

	public virtual void UpdateGround() 
	{
		Velocity = Velocity + new Vector2(MoveDir.x * GroundAcceleration, 0);	
	}

	public virtual void UpdateAir() { }

	/// <summary>
	/// Attempts to get the command parsed as Type. If the parse is not successful, default(TCommand) is returned.
	/// </summary>
	/// <typeparam name="TCommand">EntityCommand to cast</typeparam>
	/// <param name="commandType">Entry into Commands</param>
	/// <returns>Gets the command parsed as Type or default(TCommand)</returns>
	protected TCommand GetCommandClass<TCommand>(EntityCommandType commandType) where TCommand : EntityCommand
	{
		if (Commands[commandType] is TCommand command)
		{
			return command;
		}
		return default(TCommand);
	}
}
