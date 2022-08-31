namespace Sankari;

public class BasicEnemy : KinematicBody2D, IEnemy
{
    [Export] public bool StartWalkingRight;

    private const float gravity = 30000f;
    private int speed = 40;
    private bool movingForward;
    private AnimatedSprite animatedSprite;
    private RayCast2D wallCheckLeft;
    private RayCast2D wallCheckRight;
    private string[] colliderGroups;

    public void PreInit(Player player)
    {

    }

    public override void _Ready()
    {
        animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        animatedSprite.Play();
        animatedSprite.Frame = (int)GD.RandRange(0, animatedSprite.Frames.GetFrameCount("default"));

        wallCheckLeft = GetNode<RayCast2D>("Wall Checks/Left");
        wallCheckLeft.AddException(this);
        wallCheckRight = GetNode<RayCast2D>("Wall Checks/Right");
        wallCheckRight.AddException(this);

        if (StartWalkingRight) 
        {
            movingForward = !movingForward;
            animatedSprite.FlipH = true;
        }

        colliderGroups = new string[] { "Tileset" };
    }

    public override void _PhysicsProcess(float delta)
    {
        var velocity = new Vector2(0, 0);

        velocity.y += delta * gravity;

        if (movingForward)
        {
            velocity.x += speed;

            if (IsNearRightWall())
            {
                movingForward = !movingForward;
                animatedSprite.FlipH = !animatedSprite.FlipH;
            }
        }
        else 
        {
            velocity.x -= speed;

            if (IsNearLeftWall())
            {
                movingForward = !movingForward;
                animatedSprite.FlipH = !animatedSprite.FlipH;
            }
        }
            
        MoveAndSlide(velocity, Vector2.Up);
    }

    private bool IsNearLeftWall() 
    {
        var collider = wallCheckLeft.GetCollider() as Node;

        if (collider != null)
        {
            foreach (var group in colliderGroups) 
            {
                if (collider.IsInGroup(group))
                    return true;
            }
        }

        return false;
    }

    private bool IsNearRightWall() 
    {
        var collider = wallCheckRight.GetCollider() as Node;

        if (collider != null)
        {
            foreach (var group in colliderGroups) 
            {
                if (collider.IsInGroup(group))
                    return true;
            }
        }

        return false;
    }
}
