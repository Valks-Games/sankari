namespace Sankari;

public partial class Checkpoint : Node2D
{
    private AnimatedSprite2D animatedSprite;

    public override void _Ready()
    {
        animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
    }

    private void Capture()
    {
        animatedSprite.Animation = "captured";
        animatedSprite.Play();
    }

    private void _on_Area2D_area_entered(Area2D area) 
    {
        if (area.GetParent() is Player)
        {
            Player.HasTouchedCheckpoint = true;
            Player.RespawnPosition = Position;
            Capture();
        }
    }
}
