namespace Sankari;

public class Checkpoint : Node2D
{
    private AnimatedSprite animatedSprite;

    public override void _Ready()
    {
        animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
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
