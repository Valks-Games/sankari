namespace Sankari;

public partial class LevelCamera : Camera2D
{
    private Player Player { get; set; }

    private double shakingTime = 0;
    private int intensity = 24;

    private Random random = new Random();

    public override void _Ready()
    {
        Player = GetNode<Player>("../Player");
		MakeCurrent();
    }

    //public override void _Process(double delta) => Position = Player.Position;
    public override void _Process(double delta){ 
        Position = Player.Position;

        if(Player.ShakeWhenIHitTheGround && Player.IsOnFloor()) //idk how you want me to do this i just make everything public because i am hardcore
        {
            shakingTime = 0.25;
            Player.ShakeWhenIHitTheGround = false;
        }
        if(shakingTime > 0){
            Offset = new Vector2(random.Next(0, 2 * intensity) - intensity,random.Next(0, 2 * intensity) - intensity);
            shakingTime -= delta;
        }
        else{
            Offset = new Vector2();
            shakingTime = 0;
        }
    }

	public void StopFollowingPlayer() => SetProcess(false);
    public void StartFollowingPlayer() => SetProcess(true);
}
