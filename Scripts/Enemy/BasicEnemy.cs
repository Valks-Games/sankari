namespace Sankari;

public class BasicEnemy : KinematicBody2D, IEnemy, IEntity
{
    [Export] public float Speed = 40;
    [Export] public bool Active = true;
    [Export] public bool StartWalkingRight;
    [Export] public bool DontCollideWithWall;
    [Export] public bool FallOffCliff;

    private const float gravity = 30000f;
    private bool movingForward;
    
    private AnimatedSprite animatedSprite;

    private RayCast2D rayCastWallLeft;
    private RayCast2D rayCastWallRight;
    private RayCast2D rayCastCliffLeft;
    private RayCast2D rayCastCliffRight;

    public override void _Ready()
    {
        animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        Activate();

        rayCastWallLeft = PrepareRaycast("Wall Checks/Left");
        rayCastWallRight = PrepareRaycast("Wall Checks/Right");
        rayCastCliffLeft = PrepareRaycast("Cliff Checks/Left");
        rayCastCliffRight = PrepareRaycast("Cliff Checks/Right");

        if (FallOffCliff) 
        {
            rayCastCliffLeft.Enabled = false;
            rayCastCliffRight.Enabled = false;
        }

        if (DontCollideWithWall)
        {
            rayCastWallLeft.Enabled = false;
            rayCastWallRight.Enabled = false;
        }

        if (StartWalkingRight)
        {
            movingForward = !movingForward;
            animatedSprite.FlipH = true;
        }

        if (!Active) 
        {
            animatedSprite.Stop();
            animatedSprite.Frame = 0;
            SetPhysicsProcess(false);
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        var velocity = new Vector2(0, 0);

        velocity.y += delta * gravity; // delta needed here because it's an application of acceleration

        if (movingForward)
        {
            velocity.x += Speed;

            if (!DontCollideWithWall && IsRaycastColliding(rayCastWallRight))
                ChangeDirection();

            if (!FallOffCliff && !IsRaycastColliding(rayCastCliffRight))
                ChangeDirection();
        }
        else
        {
            velocity.x -= Speed;

            if (!DontCollideWithWall && IsRaycastColliding(rayCastWallLeft))
                ChangeDirection();

            if (!FallOffCliff && !IsRaycastColliding(rayCastCliffLeft))
                ChangeDirection();
        }

        MoveAndSlide(velocity, Vector2.Up); // move and slide handles delta automatically (delta needed because moving over position between frames)
    }

    public void Activate() 
    {
        SetPhysicsProcess(true);
        animatedSprite.Frame = (int)GD.RandRange(0, animatedSprite.Frames.GetFrameCount("default"));
        animatedSprite.SpeedScale = 1 + (Speed * 0.002f);
        animatedSprite.Play();
    }

    public void Deactivate() 
    {
        SetPhysicsProcess(false);
        animatedSprite.Stop();
    }

    public void Destroy()
    {
        QueueFree();
    }

    private void ChangeDirection()
    {
        movingForward = !movingForward;
        animatedSprite.FlipH = !animatedSprite.FlipH;
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
}
