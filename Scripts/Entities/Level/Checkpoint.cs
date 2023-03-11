namespace Sankari;

public partial class Checkpoint : Node2D
{
    private AnimatedSprite2D AnimatedSprite { get; set; }

    public override void _Ready()
    {
        AnimatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
    }

    private void Capture()
    {
        AnimatedSprite.Animation = "captured";
        AnimatedSprite.Play();
    }

    private void _on_Area2D_area_entered(Area2D area) 
    {
        if (area.GetParent() is Player)
        {
            GameManager.PlayerManager.ActiveCheckpoint = true;
            GameManager.PlayerManager.RespawnPosition = Position;
            GameManager.PlayerManager.SetCheckpointCoins();
            Capture();
        }
    }
}
