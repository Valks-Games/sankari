namespace Sankari;

public partial class Entity : CharacterBody2D
{
	public Dictionary<EntityCommandType, EntityCommand>     Commands   { get; set; } = new();
	public Dictionary<EntityAnimationType, EntityAnimation> Animations { get; set; } = new();

	public EntityAnimationType CurrentAnimation { get; set; } = EntityAnimationType.None;

	public float           Delta                   { get; protected set; }
	public Vector2         MoveDir                 { get; protected set; }
	public GTimers         Timers                  { get; set; }
	public virtual int     Gravity                 { get; set; } = 1200;
	public bool            GravityEnabled          { get; set; } = true;
	public List<RayCast2D> RayCast2DGroundChecks   { get;      } = new();
	public bool            HaltLogic               { get; set; }
	public virtual int     ModGravityMaxSpeed { get; set; } = 1200;

	protected int gravityMaxSpeed = 1200;

	public override void _Ready()
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

		Commands.Values.ForEach(cmd => cmd.Initialize());
		Animations[EntityAnimationType.None] = new EntityAnimationNone();
	}

	public override void _PhysicsProcess(double delta)
	{
		if (HaltLogic)
			return;

		ModGravityMaxSpeed = gravityMaxSpeed;
		Delta = (float)delta;
		Animations[CurrentAnimation].UpdateState();
		Animations[CurrentAnimation].HandleStateTransitions();

		Commands.Values.ForEach(cmd => cmd.Update(Delta));

		// gravity
		if (GravityEnabled)
			Velocity = Velocity.MoveToward(new Vector2(0, ModGravityMaxSpeed), Gravity * Delta);

		if (IsOnGround())
			UpdateGround();
		else
		{
			UpdateAir();
			Commands.Values.ForEach(cmd => cmd.UpdateAir(Delta));
		}

		MoveAndSlide();
	}

	public bool IsFalling() => Velocity.y > 0;

	public bool IsOnGround()
	{
		foreach (var raycast in RayCast2DGroundChecks)
			if (raycast.IsColliding())
				return true;

		return false;
	}

	// Checks from which side the collision occurred. -1 if is on the left, 1 on the right, 0 if neither
	public int GetCollisionSide(Area2D area)
	{
		if (this.GlobalPosition.x < area.GlobalPosition.x)
			return -1;
		else if (this.GlobalPosition.x > area.GlobalPosition.x)
			return 1;

		return 0;
	}

	protected RayCast2D PrepareRaycast(string path)
	{
		var raycast = GetNode<RayCast2D>(path);
		raycast.AddException(this);
		return raycast;
	}

	protected void PrepareRaycasts(Node parent, List<RayCast2D> list)
	{
		foreach (RayCast2D raycast in parent.GetChildren())
		{
			raycast.AddException(this);
			list.Add(raycast);
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
	protected TCommand GetCommandClass<TCommand>(EntityCommandType commandType) where TCommand : EntityCommand
	{
		if (Commands[commandType] is TCommand command)
		{
			return command;
		}
		return default(TCommand);
	}
}
