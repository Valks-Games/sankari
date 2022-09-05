namespace Sankari;

public class Coin : AnimatedSprite
{
    public override void _Ready()
    {
        Playing = true;
        Frame = (int)GD.RandRange(0, Frames.GetFrameCount("default"));
    }

    private void _on_Area_area_entered(Area2D area)
    {
        if (area.GetParent() is Player)
        {
            GameManager.LevelUIManager.AddCoins();
            GameManager.Audio.PlaySFX("coin_pickup_1", 30);
            QueueFree();
        }
    }
}
