namespace Sankari;

public partial class Turret : StaticBody2D, IEnemy
{
    [Export] protected  NodePath NodePathPositionEndOfRod { get; set; }

    private Sprite2D Rod { get; set; }
    private GTimer ShootTimer { get; set; }
    private Marker2D EndOfRod { get; set; }
    
    public override void _Ready()
    {
        Rod = GetNode<Sprite2D>("Rod");
        EndOfRod = GetNode<Marker2D>(NodePathPositionEndOfRod);
        ShootTimer = new GTimer(this, nameof(OnShoot), 1);
    }

    public override void _PhysicsProcess(double delta)
    {
        Rod.LookAt(Player.Instance.GlobalPosition);
    }

    private void OnShoot()
    {
        var cannonBall = Prefabs.CannonBall.Instantiate<CannonBall>();
        cannonBall.Position = EndOfRod.GlobalPosition;
        var cannonBallForce = 200f;
        cannonBall.LinearVelocity = (Player.Instance.Position - EndOfRod.GlobalPosition).Normalized() * cannonBallForce;
        GetTree().Root.AddChild(cannonBall);
    }
}
