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
			Events.Generic.Notify(EventGeneric.OnCoinPickup);
			GameManager.PlayerManager.AddCoins();
			GameManager.LevelUI.SetLabelCoins(GameManager.PlayerManager.Coins);
            QueueFree();
        }
    }
}
