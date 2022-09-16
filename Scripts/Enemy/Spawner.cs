namespace Sankari;

public partial class Spawner : Marker2D, IEntity
{
    [Export] public EnemyType EnemyType;
    [Export] public int RespawnInterval = 1000;

    private GTimer timer;

    public override void _Ready()
    {
        timer = new GTimer(this, nameof(OnTimer), RespawnInterval, true, false);
    }

    public void Activate() => timer.Start();
    public void Deactivate() => timer.Stop();
    public void Destroy() => QueueFree();

    private void OnTimer()
    {
        switch (EnemyType)
        {
            case EnemyType.BasicEnemy:
                var enemy = Prefabs.BasicEnemy.Instantiate<BasicEnemy>();
                enemy.DontCollideWithWall = true;
                enemy.FallOffCliff = true;
                enemy.Position = GlobalPosition;
                GetTree().Root.AddChild(enemy);
                break;
        }
    }
}

public enum EnemyType 
{
    BasicEnemy
}
