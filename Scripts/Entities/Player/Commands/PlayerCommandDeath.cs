﻿namespace Sankari;

public class PlayerCommandDeath : PlayerCommand<Player>
{
	public PlayerCommandDeath(Player player) : base(player) { }

	public override void Start()
	{
		// death animation goes through killzone a 2nd time, make sure Kill() isnt called twice
		if (Entity.HaltLogic) 
			return;

		Entity.HaltLogic = true;

		GameManager.EventsPlayer.Notify(EventPlayer.OnDied);
		
		Entity.LevelScene.Camera.StopFollowingPlayer();
		Entity.AnimatedSprite.Stop();

		var dieStartPos = Entity.Position.Y;
		var goUpDuration = 1.25f;

		// animate y position
		Entity.DieTween.InterpolateProperty
		(
			"position:y",
			dieStartPos - 80,
			goUpDuration,
			0 // delay
		);

		Entity.DieTween.InterpolateProperty
		(
			"position:y",
			dieStartPos + 400,
			1.5f,
			goUpDuration, // delay
			true
		)
		.From(dieStartPos - 80);

		// animate rotation
		Entity.DieTween.InterpolateProperty
		(
			"rotation",
			Mathf.Pi,
			1.5f,
			goUpDuration, // delay
			true
		);

		Entity.DieTween.Start();
		Entity.DieTween.Callback(nameof(Entity.OnDieTweenCompleted));
	}
}
