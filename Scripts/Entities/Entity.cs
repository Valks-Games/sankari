namespace Sankari;

public partial class Entity : CharacterBody2D
{
	public Dictionary<EntityCommandType, EntityCommand> Commands { get; set; } = new();
	public Dictionary<EntityAnimationType, EntityAnimation> Animations { get; set; } = new();

	public EntityAnimationType CurrentAnimation { get; set; }

	public float Delta { get; private set; }

	public int Gravity            { get; set; } = 1200;
	public bool GravityEnabled    { get; set; } = true;
	public  List<RayCast2D> RayCast2DGroundChecks    { get; } = new();

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
	}

	public override void _PhysicsProcess(double delta)
	{
		Delta = (float)delta;

		Animations[CurrentAnimation].UpdateState();
		Animations[CurrentAnimation].HandleStateTransitions();

		Commands.Values.ForEach(cmd => cmd.Update(Delta));

		// gravity
		if (GravityEnabled)
			Velocity = Velocity + new Vector2(0, Gravity * Delta);

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

	// Checks from which side the collision occured. -1 if is on the left, 1 on the right, 0 if neither
	public int GetCollisionSide(Area2D area)
	{
		if (this.GlobalPosition.x < area.GlobalPosition.x)
			return -1;
		else if (this.GlobalPosition.x > area.GlobalPosition.x)
			return 1;

		return 0;
	}

	public void PrepareRaycasts(Node parent, List<RayCast2D> list)
	{
		foreach (RayCast2D raycast in parent.GetChildren())
		{
			raycast.AddException(this);
			list.Add(raycast);
		}
	}

	public virtual void UpdateGround() { }

	public virtual void UpdateAir() { }
}
