namespace Sankari;

public partial class LevelFinish : Area2D
{
    private async void _on_level_finish_area_entered(Area2D area) 
    {
        if (area.GetParent() is Player player)
        {
            await player.FinishedLevel();
        }
    }
}
