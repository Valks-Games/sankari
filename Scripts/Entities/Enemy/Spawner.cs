namespace Sankari;

public partial class Spawner : Marker2D, IEntity
{
    [Export] public EnemyType EnemyType { get; set; }
    [Export] public int RespawnInterval { get; set; } = 1000;

    private GTimer Timer { get; set; }

    public override void _Ready()
    {
        Timer = new GTimer(this, OnTimer, RespawnInterval)
		{
			Loop = true
		};
    }

    public void Activate() => Timer.Start();
    public void Deactivate() => Timer.Stop();
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
