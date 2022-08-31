namespace Sankari;

public class BasicEnemy : KinematicBody2D, IEnemy, IEntity
{
    [Export] public bool Active = true;
    [Export] public bool StartWalkingRight;
    [Export] public bool FallOffCliff;

    private const float gravity = 30000f;
    private int speed = 40;
    private bool movingForward;
    
    private AnimatedSprite animatedSprite;

    private RayCast2D rayCastWallLeft;
    private RayCast2D rayCastWallRight;
    private RayCast2D rayCastCliffLeft;
    private RayCast2D rayCastCliffRight;

    public void PreInit(Player player)
    {

    }

    public override void _Ready()
    {
        animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        animatedSprite.Play();
        animatedSprite.Frame = (int)GD.RandRange(0, animatedSprite.Frames.GetFrameCount("default"));

        rayCastWallLeft = PrepareRaycast("Wall Checks/Left");
        rayCastWallRight = PrepareRaycast("Wall Checks/Right");
        rayCastCliffLeft = PrepareRaycast("Cliff Checks/Left");
        rayCastCliffRight = PrepareRaycast("Cliff Checks/Right");

        if (FallOffCliff) 
        {
            rayCastCliffLeft.Enabled = false;
            rayCastCliffRight.Enabled = false;
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
            velocity.x += speed;

            if (IsRaycastColliding(rayCastWallRight))
                ChangeDirection();

            if (!FallOffCliff && !IsRaycastColliding(rayCastCliffRight))
                ChangeDirection();
        }
        else
        {
            velocity.x -= speed;

            if (IsRaycastColliding(rayCastWallLeft))
                ChangeDirection();

            if (!FallOffCliff && !IsRaycastColliding(rayCastCliffLeft))
                ChangeDirection();
        }

        MoveAndSlide(velocity, Vector2.Up); // move and slide handles delta automatically (delta needed because moving over position between frames)
    }

    public void Activate() 
    {
        SetPhysicsProcess(true);
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
