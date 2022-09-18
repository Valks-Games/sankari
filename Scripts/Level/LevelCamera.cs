namespace Sankari;

public partial class LevelCamera : Camera2D
{
    private Player Player { get; set; }

    public override void _Ready()
    {
        Player = GetNode<Player>("../Player");
		Current = true;
    }

    public override void _Process(double delta) => Position = Player.Position;

	public void StopFollowingPlayer() => SetProcess(false);
    public void StartFollowingPlayer() => SetProcess(true);
}
