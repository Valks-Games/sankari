namespace MarioLikeGame;

public class Coin : AnimatedSprite
{
    public override void _Ready()
    {
        Playing = true;
        Frame = (int)GD.RandRange(0, Frames.GetFrameCount("default"));
    }
}
