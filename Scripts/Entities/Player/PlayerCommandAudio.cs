namespace Sankari;

public class PlayerCommandAudio : PlayerCommand
{
	public PlayerCommandAudio(Player player) : base(player) { }

	public override void Jump()
	{
		Audio.PlaySFX("player_jump", 80);
	}

	public override void Died()
	{
		Audio.StopMusic();
		Audio.PlaySFX("game_over_1");
	}
}
