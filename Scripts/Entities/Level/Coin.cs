namespace Sankari;

public partial class Coin : AnimatedSprite2D
{
    public override void _Ready()
    {
		Play();
        Frame = GD.RandRange(0, SpriteFrames.GetFrameCount("default"));
    }

    private void _on_Area_area_entered(Area2D area)
    {
        if (area.GetParent() is Player)
        {
			GameManager.Events.Notify(Event.OnCoinPickup);
			GameManager.PlayerManager.AddCoins();
			GameManager.LevelUI.SetLabelCoins(GameManager.PlayerManager.Coins);
            QueueFree();
        }
    }
}
