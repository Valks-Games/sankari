namespace Sankari;

public partial class BasicEnemy : CharacterBody2D, IEnemy, IEntity
{
    [Export] public float Speed { get; set; } = 40;
    [Export] public bool Active { get; set; } = true;
    [Export] public bool StartWalkingRight { get; set; }
    [Export] public bool DontCollideWithWall { get; set; }
    [Export] public bool FallOffCliff { get; set; }

    private float Gravity { get; set; } = 30000f;
    private bool MovingForward { get; set; }
    
    private AnimatedSprite2D AnimatedSprite { get; set; }

    private RayCast2D RayCastWallLeft { get; set; }
    private RayCast2D RayCastWallRight { get; set; }
    private RayCast2D RayCastCliffLeft { get; set; }
    private RayCast2D RayCastCliffRight { get; set; }

    public void PreInit(Player player)
    {
     
    }

    public override void _Ready()
    {
        AnimatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        Activate();

        RayCastWallLeft = PrepareRaycast("Wall Checks/Left");
        RayCastWallRight = PrepareRaycast("Wall Checks/Right");
        RayCastCliffLeft = PrepareRaycast("Cliff Checks/Left");
        RayCastCliffRight = PrepareRaycast("Cliff Checks/Right");

        if (FallOffCliff) 
        {
            RayCastCliffLeft.Enabled = false;
            RayCastCliffRight.Enabled = false;
        }

        if (DontCollideWithWall)
        {
            RayCastWallLeft.Enabled = false;
            RayCastWallRight.Enabled = false;
        }

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
    }

    public override void _PhysicsProcess(double d)
    {
        var delta = (float)d;
        var velocity = new Vector2(0, 0);

        velocity.y += delta * Gravity; // delta needed here because it's an application of acceleration

        if (MovingForward)
        {
            velocity.x += Speed;

            if (!DontCollideWithWall && IsRaycastColliding(RayCastWallRight))
                ChangeDirection();

            if (!FallOffCliff && !IsRaycastColliding(RayCastCliffRight))
                ChangeDirection();
        }
        else
        {
            velocity.x -= Speed;

            if (!DontCollideWithWall && IsRaycastColliding(RayCastWallLeft))
                ChangeDirection();

            if (!FallOffCliff && !IsRaycastColliding(RayCastCliffLeft))
                ChangeDirection();
        }

		Velocity = velocity;

        MoveAndSlide(); // move and slide handles delta automatically (delta needed because moving over position between frames)
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

    private RayCast2D PrepareRaycast(string path)
    {
        var raycast = GetNode<RayCast2D>(path);
        raycast.AddException(this);
        return raycast;
    }

    private bool IsRaycastColliding(RayCast2D raycast)
    {
        var collider = raycast.GetCollider() as Node;

        if (collider != null)
        {
            if (collider.IsInGroup("Tileset"))
                return true;
        }

        return false;
    }

	private void _on_enemy_area_entered(Area2D area) 
	{
		// TODO: Fix player being able to hide in a enemy's area after touching the enemy
		// Immunity frames for the player needs to be implemented

		if (area.IsInGroup("Player"))
		{ 
			var player = area.GetParent<Player>();
			player.TakenDamage(player.GetCollisionSide(area), 1);
		}	
	}
}
