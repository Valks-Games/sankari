namespace Sankari;

public partial class Trapper : Entity
{
	private Area2D AreaDamage { get; set; }
	private GTimer TimerReveal { get; set; }
	private GTimer TimerRevealCooldown { get; set; }
	private bool Revealed { get; set; }

	public override void Init()
	{
		AreaDamage = GetNode<Area2D>("Damage");
		TimerReveal = new GTimer(this, nameof(OnTimerReveal), 1500, false);
		TimerRevealCooldown = new GTimer(this, 2000, false);
		AnimatedSprite.Play("idle");
	}

	public override void UpdatePhysics()
	{
		
	}

	private void _on_damage_area_entered(Area2D area)
	{
		if (area.GetParent() is Player player)
			player.RemoveHealth(1);
	}

	private void _on_detection_area_entered(Area2D area)
	{
		if (Revealed || TimerRevealCooldown.IsActive())
			return;

		if (area.GetParent() is Player)
		{
			Revealed = true;
			AnimatedSprite.Play("reveal");
			AreaDamage.SetDeferred("monitoring", true);
			TimerReveal.Start();
		}
	}

	private void OnTimerReveal()
	{
		Revealed = false;
		AreaDamage.SetDeferred("monitoring", false);
		AnimatedSprite.Play("hide");
		TimerRevealCooldown.Start();
	}
}
