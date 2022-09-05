namespace Sankari;

public class Turret : StaticBody2D, IEnemy
{
    [Export] protected readonly NodePath NodePathPositionEndOfRod;

    private Sprite rod;
    private GTimer shootTimer;
    private Position2D endOfRod;
    
    public override void _Ready()
    {
        rod = GetNode<Sprite>("Rod");
        endOfRod = GetNode<Position2D>(NodePathPositionEndOfRod);
        shootTimer = new GTimer(this, nameof(OnShoot), 1);
    }

    public override void _PhysicsProcess(float delta)
    {
        rod.LookAt(Player.Instance.GlobalPosition);
    }

    private void OnShoot()
    {
        var cannonBall = Prefabs.CannonBall.Instance<CannonBall>();
        cannonBall.Position = endOfRod.GlobalPosition;
        var cannonBallForce = 200f;
        cannonBall.LinearVelocity = (Player.Instance.Position - endOfRod.GlobalPosition).Normalized() * cannonBallForce;
        GetTree().Root.AddChild(cannonBall);
    }
}
