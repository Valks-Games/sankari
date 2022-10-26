namespace Sankari;

public class PlayerCommandDeath : EntityCommand<Player>
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

		var dieStartPos = Entity.Position.y;
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
		Entity.DieTween.Callback(() => OnDieTweenCompleted());
	}

	private async void OnDieTweenCompleted()
	{
		if (GameManager.PlayerManager.RemoveLife())
		{
			await GameManager.Transition.AlphaToBlack();
			await Task.Delay(1000);
			GameManager.LevelUI.ShowLives();
			await Task.Delay(1750);
			GameManager.LevelUI.SetLabelLives(GameManager.PlayerManager.Lives);
			await Task.Delay(1000);
			await GameManager.LevelUI.HideLivesTransition();
			await Task.Delay(250);
			GameManager.LevelUI.AddHealth(6);
			GameManager.LevelUI.SetLabelCoins(GameManager.PlayerManager.Coins);
			GameManager.Transition.BlackToAlpha();
			Entity.HaltLogic = false;
			LevelManager.LoadLevelFast();
			Entity.LevelScene.Camera.StartFollowingPlayer();
		}
		else
		{
			GameManager.PlayerManager.ResetCoins();
			await GameManager.Transition.AlphaToBlack();
			await Task.Delay(1000);
			GameManager.LevelUI.ShowGameOver();
			await Task.Delay(1750);
			GameManager.LoadMap();
			GameManager.Transition.BlackToAlpha();
			GameManager.LevelUI.HideGameOver();
			GameManager.LevelUI.SetLabelCoins(GameManager.PlayerManager.Coins);
		}
	}
}
