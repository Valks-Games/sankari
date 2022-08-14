namespace MarioLikeGame;

public class Turret : StaticBody2D, IEnemy
{
    [Export] protected readonly NodePath NodePathPositionEndOfRod;

    private Sprite _rod;
    private Player _player;
    private GTimer _shootTimer;
    private Position2D _endOfRod;

    public void PreInit(Player player)
    {
        _player = player;
    }
    
    public override void _Ready()
    {
        _rod = GetNode<Sprite>("Rod");
        _endOfRod = GetNode<Position2D>(NodePathPositionEndOfRod);
        _shootTimer = new GTimer(this, nameof(OnShoot), 1);
    }

    public override void _PhysicsProcess(float delta)
    {
        _rod.LookAt(_player.GlobalPosition);
    }

    private void OnShoot()
    {
        var cannonBall = Prefabs.CannonBall.Instance<CannonBall>();
        cannonBall.Position = _endOfRod.GlobalPosition;
        var cannonBallForce = 200f;
        cannonBall.LinearVelocity = (_player.Position - _endOfRod.GlobalPosition).Normalized() * cannonBallForce;
        GetTree().Root.AddChild(cannonBall);
    }
}
