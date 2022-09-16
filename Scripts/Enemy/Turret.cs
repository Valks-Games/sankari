namespace Sankari;

public partial class Turret : StaticBody2D, IEnemy
{
    [Export] protected  NodePath NodePathPositionEndOfRod;

    private Sprite2D rod;
    private GTimer shootTimer;
    private Marker2D endOfRod;
    
    public override void _Ready()
    {
        rod = GetNode<Sprite2D>("Rod");
        endOfRod = GetNode<Marker2D>(NodePathPositionEndOfRod);
        shootTimer = new GTimer(this, nameof(OnShoot), 1);
    }

    public override void _PhysicsProcess(double delta)
    {
        rod.LookAt(Player.Instance.GlobalPosition);
    }

    private void OnShoot()
    {
        var cannonBall = Prefabs.CannonBall.Instantiate<CannonBall>();
        cannonBall.Position = endOfRod.GlobalPosition;
        var cannonBallForce = 200f;
        cannonBall.LinearVelocity = (Player.Instance.Position - endOfRod.GlobalPosition).Normalized() * cannonBallForce;
        GetTree().Root.AddChild(cannonBall);
    }
}
