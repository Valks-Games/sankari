namespace Sankari;

public partial class BasicEnemy : MovingEntity, IEntity
{
	[Export] public float Speed { get; set; } = 40;
	[Export] public bool Active { get; set; } = true;
	[Export] public bool StartWalkingRight { get; set; }

	public override int AccelerationGround { get; set; } = 30;
	public override int Gravity { get; set; } = 300;
	public override int MaxSpeedWalk { get; set; } = 100;

	private bool MovingForward { get; set; }

	public override void Init()
	{
		Activate();

		if (StartWalkingRight)
		{
			MovingForward = !MovingForward;
			AnimatedSprite.FlipH = true;
		}

		if (!Active)
		{
			AnimatedSprite.Stop();
			AnimatedSprite.Frame = 0;
			SetPhysicsProcess(false);
		}

		FloorStopOnSlope = false;

		//Label.Visible = true;
	}

	public override void UpdatePhysics()
	{
		MoveDir = MovingForward ? Vector2.Right : Vector2.Left;

		if (MovingForward)
		{
			// Move forwards

			// If the entity is set to collide with a wall then change directions
			// when touching a wall
			if (!DontCollideWithWall && IsNearWallRight())
				ChangeDirection();

			// If the entity is set to not fall off a cliff and there is no ground
			// to the right in front of the entity then assume there is a cliff and
			// prevent the entity from falling off the cliff by changing directions
			if (!FallOffCliff && !IsNearCliffRight())
				ChangeDirection();
		}
		else
		{
			// Move backwards

			// If the entity is set to collide with a wall then change directions
			// when touching a wall
			if (!DontCollideWithWall && IsNearWallLeft())
				ChangeDirection();

			// If the entity is set to not fall off a cliff and there is no ground
			// to the left in front of the entity then assume there is a cliff and
			// prevent the entity from falling off the cliff by changing directions
			if (!FallOffCliff && !IsNearCliffLeft())
				ChangeDirection();
		}
	}

	public override void UpdateGround()
	{
		Commands.Values.ForEach(cmd => cmd.UpdateGroundWalking(Delta));
	}

	public void Activate()
	{
		SetPhysicsProcess(true);
		AnimatedSprite.Frame = GD.RandRange(0, AnimatedSprite.Frames.GetFrameCount("default"));
		AnimatedSprite.SpeedScale = 1 + (Speed * 0.002f);
		AnimatedSprite.Play();
	}

	public void Deactivate()
	{
		SetPhysicsProcess(false);
		AnimatedSprite.Stop();
	}

	public void Destroy()
	{
		QueueFree();
	}

	private void ChangeDirection()
	{
		MovingForward = !MovingForward;
		AnimatedSprite.FlipH = !AnimatedSprite.FlipH;
	}

	private void _on_enemy_area_entered(Area2D area)
	{
		if (area.IsInGroup("Player"))
		{
			var player = area.GetParent<Player>();
			player.RemoveHealth(1);
			player.InDamageZone = true;
		}
	}

	private void _on_enemy_area_exited(Area2D area) 
	{
		if (area.IsInGroup("Player"))
		{
			var player = area.GetParent<Player>();
			player.InDamageZone = false;
		}	
	}
}
