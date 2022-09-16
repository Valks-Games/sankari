namespace Sankari;

public partial class Coin : AnimatedSprite2D
{
    public override void _Ready()
    {
        Playing = true;
        Frame = GD.RandRange(0, Frames.GetFrameCount("default"));
    }

    private void _on_Area_area_entered(Area2D area)
    {
        if (area.GetParent() is Player)
        {
            GameManager.LevelUI.AddCoins();
            GameManager.Audio.PlaySFX("coin_pickup_1", 30);
            QueueFree();
        }
    }
}
