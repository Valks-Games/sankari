namespace Sankari;

public abstract partial class Entity : CharacterBody2D
{
	// ================================= EXPORTS =================================

	/// <summary>
	/// Should this entity care about wall collisions?
	/// </summary>
	[Export] public bool DontCollideWithWall { get; set; }

	/// <summary>
	/// Should this entity keep moving forward when near a cliff?
	/// </summary>
	[Export] public bool FallOffCliff { get; set; }

	/// <summary>
	/// There are many entities, this will help debug specific entities since
	/// this value can be set per entity through the inspector in the editor
	/// </summary>
	[Export] public bool Debug { get; set; }

	// ================================= PROPERTIES =================================

	// Movement

	/// <summary>
	/// The direction this entity is currently moving
	/// </summary>
	public Vector2 MoveDir { get; protected set; }

	/// <summary>
	/// Should this entity be affected by gravity?
	/// </summary>
	public bool GravityEnabled { get; set; } = true;

	/// <summary>
	/// The gravity of the entity
	/// </summary>
	public virtual int Gravity { get; set; } = 1200;

	/// <summary>
	/// ???
	/// </summary>
	public virtual int ModGravityMaxSpeed { get; set; } = 1200;

	/// <summary>
	/// ??? - While the property name sounds self explanatory, this needs to be looked into further to
	/// ensure it's doing what it says it's doing
	/// </summary>
	public int GroundAcceleration { get; set; } = 50;

	// Msc

	/// <summary>
	/// All commands for an entity call Initialize() for first frame and
	/// Update() and UpdateAir() every frame
	/// </summary>
	public Dictionary<EntityCommandType, EntityCommand> Commands { get; set; } = new();

	/// <summary>
	/// All animations for an entity call UpdateState() and HandleStateTransitions() 
	/// every frame
	/// </summary>
	public Dictionary<EntityAnimationType, EntityAnimation> Animations { get; set; } = new();

	/// <summary>
	/// The current animation that is being used for the entity
	/// </summary>
	public EntityAnimationType CurrentAnimation { get; set; } = EntityAnimationType.None;

	/// <summary>
	/// A label used for mostly debugging information displayed above the entity in-game but
	/// can also be used for other things
	/// </summary>
	public Label Label { get; set; }

	/// <summary>
	/// The delta from _PhysicsProcess(double delta) converted to a float
	/// </summary>
	public float Delta { get; protected set; }

	/// <summary>
	/// A convience property to help with the initialization of timers
	/// </summary>
	public GTimers Timers { get; set; }

	/// <summary>
	/// Used to halt the _PhysicsProcess()
	/// </summary>
	public bool HaltLogic { get; set; }

	/// <summary>
	/// The immunity time in milliseconds after getting hit
	/// </summary>
	public int ImmunityMs { get; set; } = 500;

	/// <summary>
	/// Constantly updates giving information whether the entity is in a damage zone or not
	/// </summary>
	public bool InDamageZone { get; set; }

	public Window Tree { get; set; }

	// ================================= RAYCASTS =================================

	// Raycast Parents
	protected Node ParentRaycastsWallLeft   { get; set; }
	protected Node ParentRaycastsWallRight  { get; set; }
	protected Node ParentRaycastsCliffLeft  { get; set; }
	protected Node ParentRaycastsCliffRight { get; set; }
	protected Node ParentRaycastsGround     { get; set; }

	// Raycasts
	// these are public because some interfaces rely on them
	public List<RayCast2D> RaycastsWallLeft { get; set; } = new();
	public List<RayCast2D> RaycastsWallRight { get; set; } = new();
	public List<RayCast2D> RaycastsCliffLeft { get; set; } = new();
	public List<RayCast2D> RaycastsCliffRight { get; set; } = new();
	public List<RayCast2D> RaycastsGround { get; set; } = new();
	
	// ================================= FIELDS =================================

	// Why are these fields and not properties?
	protected int gravityMaxSpeed = 1200;
	private GTimer immunityTimer; // the timer for immunity
	private int damageTakenForce = 300;

	sealed public override void _Ready()
	{
		Tree = GetTree().Root;

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

		// Assuming all entities will have "Label" path
		Label = GetNodeOrNull<Label>("Label");

		if (Label != null)
		{
			Label.Text = Name;
			Label.Visible = false;
		}

		// Setup raycasts
		// Assuming all entities will have the hardcoded paths
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

		// Do not check for cliffs if FallOffCliff is set to true
		if (FallOffCliff)
		{
			SetRaycastsEnabled(RaycastsCliffLeft, false);
			SetRaycastsEnabled(RaycastsCliffRight, false);
		}

		// Do not collide with walls if DontCollideWithWall is set to true
		if (DontCollideWithWall)
		{
			SetRaycastsEnabled(RaycastsWallLeft, false);
			SetRaycastsEnabled(RaycastsWallRight, false);
		}
		
		// All entities will use Init() instead of _Ready()
		Init();
		
		Commands.Values.ForEach(cmd => cmd.Initialize());
		Animations[EntityAnimationType.None] = new EntityAnimationNone();
	}

	sealed public override void _PhysicsProcess(double delta)
	{
		if (HaltLogic) // perhaps SetPhysicsProcess(false) should be used instead of this
			return;

		ModGravityMaxSpeed = gravityMaxSpeed; // ???
		Delta = (float)delta; // convert Delta to a float as most Godot functions require float inputs

		// All entities will use UpdatePhysics() instead of _PhysicsProcess(double delta)
		UpdatePhysics();

		Animations[CurrentAnimation].UpdateState();
		Animations[CurrentAnimation].HandleStateTransitions();

		Commands.Values.ForEach(cmd => cmd.Update(Delta));

		// gravity
		if (GravityEnabled)
			Velocity = Velocity + new Vector2(0, Gravity * Delta); // appears to do the same thing as Velocity = Velocity.MoveToward(new Vector2(0, ModGravityMaxSpeed), Gravity * Delta);

		if (IsNearGround())
		{
			Velocity = Velocity + new Vector2(MoveDir.x * GroundAcceleration, 0);
			UpdateGround();
		}
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
	/// frame. Delta is defined in Entity. MoveAndSlide() is called right after this.
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

	public bool IsNearGround() => AreRaycastsColliding(RaycastsGround, "Ground");
	protected bool IsNearWallLeft() => AreRaycastsColliding(RaycastsWallLeft, "Wall Left");
	protected bool IsNearWallRight() => AreRaycastsColliding(RaycastsWallRight, "Wall Right");
	protected bool IsNearCliffLeft() => AreRaycastsColliding(RaycastsCliffLeft, "Cliff Left");
	protected bool IsNearCliffRight() => AreRaycastsColliding(RaycastsCliffRight, "Cliff Right");

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
			raycast.ExcludeRaycastParents(raycast.GetParent());
			raycastList.Add(raycast);
		}
	}

	public virtual void UpdateGround() { }

	public virtual void UpdateAir() { }

	/// <summary>
	/// Attempts to get the command parsed as Type. If the parse is not successful, default(TCommand) is returned.
	/// </summary>
	/// <typeparam name="TCommand">EntityCommand to cast</typeparam>
	/// <param name="commandType">Entry into Commands</param>
	/// <returns>Gets the command parsed as Type or default(TCommand)</returns>
	public TCommand GetCommandClass<TCommand>(EntityCommandType commandType) where TCommand : EntityCommand
	{
		if (Commands[commandType] is TCommand command)
			return command;

		return default(TCommand);
	}
}
