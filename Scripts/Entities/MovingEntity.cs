namespace Sankari;

public abstract partial class MovingEntity : CharacterBody2D
{
	[Export] public bool DontCollideWithWall { get; set; } // should this entity care about wall collisions?
	[Export] public bool FallOffCliff        { get; set; } // should this entity keep moving forward when near a cliff?
	[Export] public bool Debug               { get; set; } // there are many entities, this will help debug specific entities since this value can be set per entity through the inspector in the editor

	public Vector2 MoveDir { get; protected set; } // the direction this entity is currently moving

	public virtual bool GravityEnabled     { get; set; } = true; // should this entity be affected by gravity?
	public virtual int  Gravity            { get; set; } = 1200; // the gravity of the entity
	public virtual int  ModGravityMaxSpeed { get; set; } = 1200; // ???
	public virtual int  AccelerationGround { get; set; } = 50;   // the ground acceleration of the entity
	public virtual int  ImmunityMs         { get; set; } = 500;  // the immunity time in milliseconds after getting hit
	public virtual int  MaxSpeedWalk       { get; set; } = 350;
	public virtual int  MaxSpeedSprint     { get; set; } = 500;
	public virtual int  MaxSpeedAir        { get; set; } = 350;
	public virtual int  AirAcceleration    { get; set; } = 30;
	public virtual int  DampeningAir       { get; set; } = 10;
	public virtual int  DampeningGround    { get; set; } = 25;
	public virtual bool ClampDampenGround  { get; set; } = true;
	public virtual bool ClampDampenAir     { get; set; } = true;
	public virtual int  HalfHearts         { get; set; } = 6;
	public virtual int  MaximumHealth      { get; set; } = 6;

	public int MaxSpeed { get; set; }
	public bool InWallJumpArea { get; set; }
	public AnimatedSprite2D AnimatedSprite { get; set; }

	/// <summary>
	/// All commands for an entity call Initialize() for first frame and Update() and UpdateAir() every frame
	/// </summary>
	public Dictionary<EntityCommandType, MovingEntityCommand> Commands { get; set; } = new();

	/// <summary>
	/// All animations for an entity call UpdateState() and HandleStateTransitions() every frame
	/// </summary>
	public Dictionary<EntityAnimationType, EntityAnimation> Animations { get; set; } = new();

	public EntityAnimationType CurrentAnimation { get; set; } = EntityAnimationType.None; // The current animation that is being used for the entity

	public Label Label { get; set; } // a label used for mostly debugging information displayed above the entity in-game but can also be used for other thing
	public float Delta { get; protected set; } // the delta from _PhysicsProcess(double delta) converted to a float
	public GTimers Timers { get; set; } // a convience property to help with the initialization of timers
	public bool HaltLogic { get; set; } // used to see if _PhysicsProcess() was halted or not
	public bool InDamageZone { get; set; } // the entity is in a damage zone or not
	public Window Tree { get; set; }

	// Raycast Parents
	protected Node ParentRaycastsWallLeft   { get; set; }
	protected Node ParentRaycastsWallRight  { get; set; }
	protected Node ParentRaycastsCliffLeft  { get; set; }
	protected Node ParentRaycastsCliffRight { get; set; }
	protected Node ParentRaycastsGround     { get; set; }

	// Raycasts
	public List<RayCast2D> RaycastsWallLeft { get; set; } = new();
	public List<RayCast2D> RaycastsWallRight { get; set; } = new();
	public List<RayCast2D> RaycastsCliffLeft { get; set; } = new();
	public List<RayCast2D> RaycastsCliffRight { get; set; } = new();
	public List<RayCast2D> RaycastsGround { get; set; } = new();

	protected int GravityMaxSpeed { get; set; } = 1200;
	protected GTimer ImmunityTimer { get; set; }
	protected int DamageTakenForce { get; set; } = 300;
	private bool TouchedGroundBool { get; set; }

	public event EventHandler Jump;

	public sealed override void _Ready()
	{
		Tree = GetTree().Root;
		MaxSpeed = MaxSpeedWalk;
		Timers = new GTimers(this);

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

		ImmunityTimer = new GTimer(this, nameof(OnImmunityTimerFinished), ImmunityMs, false)
		{
			Loop = false
		};

		if (Label != null)
		{
			Label.Text = Name;
			Label.Visible = false;
		}

		// Setup nodes
		// assuming all entities will have the hardcoded paths
		Label = GetNodeOrNull<Label>("Label");
		AnimatedSprite = GetNodeOrNull<AnimatedSprite2D>("AnimatedSprite2D");

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

		// do not check for cliffs if FallOffCliff is set to true
		if (FallOffCliff)
		{
			SetRaycastsEnabled(RaycastsCliffLeft, false);
			SetRaycastsEnabled(RaycastsCliffRight, false);
		}

		// do not collide with walls if DontCollideWithWall is set to true
		if (DontCollideWithWall)
		{
			SetRaycastsEnabled(RaycastsWallLeft, false);
			SetRaycastsEnabled(RaycastsWallRight, false);
		}
		
		// all entities will use Init() instead of _Ready()
		Init();

		// if these are equal to each other then the player movement will not work as expected
		if (AccelerationGround == DampeningGround)
			DampeningGround -= 1;
		
		Commands.Values.ForEach(cmd => cmd.Initialize());
		Animations[EntityAnimationType.None] = new EntityAnimationNone();
	}

	public sealed override void _Process(double delta)
	{
		Update();
	}

	public sealed override void _PhysicsProcess(double delta)
	{
		if (HaltLogic) // perhaps SetPhysicsProcess(false) should be used instead of this
			return;

		ModGravityMaxSpeed = GravityMaxSpeed; // ???
		Delta = (float)delta; // convert Delta to a float as most Godot functions require float inputs

		// all entities will use UpdatePhysics() instead of _PhysicsProcess(double delta)
		UpdatePhysics();

		Animations[CurrentAnimation].UpdateState();
		Animations[CurrentAnimation].HandleStateTransitions();

		Commands.Values.ForEach(cmd => cmd.Update(Delta));

		// gravity
		if (GravityEnabled)
			Velocity = Velocity + new Vector2(0, Gravity * Delta); // appears to do the same thing as Velocity = Velocity.MoveToward(new Vector2(0, ModGravityMaxSpeed), Gravity * Delta);

		if (IsNearGround())
		{
			if (!TouchedGroundBool)
			{
				TouchedGroundBool = true;
				TouchedGround();
			}

			if (ClampDampenGround)
			{
				var velocity = Velocity;
				velocity.x += MoveDir.x * AccelerationGround;
				velocity.x = ClampAndDampen(velocity.x, DampeningGround, MaxSpeed);
				Velocity = velocity;
			}

			UpdatePhysicsGround();
		}
		else
		{
			TouchedGroundBool = false;

			if (ClampDampenAir)
			{
				var velocity = Velocity;
				velocity.x += MoveDir.x * AirAcceleration;
				velocity.x = ClampAndDampen(velocity.x, DampeningAir, MaxSpeedAir);
				Velocity = velocity;
			}

			UpdatePhysicsAir();

			Commands.Values.ForEach(cmd => cmd.UpdateAir(Delta));
		}

		MoveAndSlide();
	}

	public virtual void Init() { }
	public virtual void Update() { }
	public virtual void UpdatePhysicsGround() { }
	public virtual void UpdatePhysicsAir() { }
	public virtual void UpdatePhysics() { }
	public virtual void Kill() { }
	public virtual void TouchedGround() { }

	public virtual void AddHealth(int v)
	{
		HalfHearts = HalfHearts + Mathf.Min(MaximumHealth, v); // do not add health over maximum
	}

	public virtual void RemoveHealth(int v)
	{
		// Do not take damage if immunity timer is active
		if (ImmunityTimer.IsActive())
			return;

		// Damage the player
		HalfHearts = HalfHearts - Mathf.Min(HalfHearts, v); // do not take away more than what the entity does not have

		// Player has taken damage so start the immunity timer
		ImmunityTimer.Start();

		// If player has no health left, kill them
		if (HalfHearts == 0)
		{
			Kill();
			return;
		}

		// Stop any dashes in progress and apply a force in the opposite direction the player is moving
		Commands[EntityCommandType.Dash].Stop();
		Velocity = new Vector2(-MoveDir.x * DamageTakenForce, -DamageTakenForce);
	}

	protected virtual void OnJump() // code smell?
	{
		Jump?.Invoke(this, EventArgs.Empty);
	}

	private void OnImmunityTimerFinished() 
	{
		if (InDamageZone)
			RemoveHealth(1);
	}

	public void OnDashReady()
	{
		Audio.PlaySFX("dash_replenish");
		GetCommandClass<MovingEntityCommandDash>(EntityCommandType.Dash).DashCount = 0; // temporary fix
		GetCommandClass<MovingEntityCommandDash>(EntityCommandType.Dash).DashReady = true;
	}

	public void OnDashDurationDone()
	{
		GetCommandClass<MovingEntityCommandDash>(EntityCommandType.Dash).CurrentlyDashing = false;
		GravityEnabled = true;
		//GetCommandClass<MovingEntityCommandDash>(EntityCommandType.Dash).DashDurationDone?.Invoke(this, EventArgs.Empty);
	}

	protected float ClampAndDampen(float horzVelocity, int dampening, int maxSpeedGround) 
	{
		if (Mathf.Abs(horzVelocity) <= dampening)
			return 0;
		else if (horzVelocity > 0)
			return Mathf.Min(horzVelocity - dampening, maxSpeedGround);
		else
			return Mathf.Max(horzVelocity + dampening, -maxSpeedGround);
	}

	public bool IsFalling() => Velocity.y > 0;

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

	/// <summary>
	/// Attempts to get the command parsed as Type. If the parse is not successful, default(TCommand) is returned.
	/// </summary>
	/// <typeparam name="TCommand">EntityCommand to cast</typeparam>
	/// <param name="commandType">Entry into Commands</param>
	/// <returns>Gets the command parsed as Type or default(TCommand)</returns>
	public TCommand GetCommandClass<TCommand>(EntityCommandType commandType) where TCommand : MovingEntityCommand
	{
		if (Commands[commandType] is TCommand command)
			return command;

		return default(TCommand);
	}

	public override string ToString() => Name;
}
