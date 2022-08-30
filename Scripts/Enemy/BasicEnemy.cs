namespace Sankari;

public class BasicEnemy : KinematicBody2D, IEnemy
{
    [Export] public bool StartWalkingRight;

    private const float gravity = 6000f;
    private bool movingForward;
    private AnimatedSprite animatedSprite;
    private RayCast2D wallCheckLeft;
    private RayCast2D wallCheckRight;

    public void PreInit(Player player)
    {

    }

    public override void _Ready()
    {
        animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
        animatedSprite.Play();

        wallCheckLeft = GetNode<RayCast2D>("Wall Checks/Left");
        wallCheckLeft.AddException(this);
        wallCheckRight = GetNode<RayCast2D>("Wall Checks/Right");
        wallCheckRight.AddException(this);

        if (StartWalkingRight) 
        {
            movingForward = !movingForward;
            animatedSprite.FlipH = true;
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        var velocity = new Vector2(0, 0);

        velocity.y += delta * gravity;

        if (movingForward)
        {
            velocity.x += 10;

            if (IsNearRightWall())
            {
                movingForward = !movingForward;
                animatedSprite.FlipH = !animatedSprite.FlipH;
            }
        }
        else 
        {
            velocity.x -= 10;

            if (IsNearLeftWall())
            {
                movingForward = !movingForward;
                animatedSprite.FlipH = !animatedSprite.FlipH;
            }
        }
            
        MoveAndSlide(velocity, new Vector2(0, -1));
    }//

    private bool IsNearLeftWall() 
    {
        var collider = wallCheckLeft.GetCollider() as Node;

        if (collider != null && collider.IsInGroup("Tileset"))
            return true;

        return false;
    }

    private bool IsNearRightWall() 
    {
        var collider = wallCheckRight.GetCollider() as Node;

        if (collider != null && collider.IsInGroup("Tileset"))
            return true;

        return false;
    }
}
