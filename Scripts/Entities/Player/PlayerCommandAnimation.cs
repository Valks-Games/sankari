namespace Sankari;

public class PlayerCommandAnimation : PlayerCommand
{
	public PlayerCommandAnimation(Player player) : base(player) { }

	public override void Update(float delta)
	{
		if (Player.MoveDir.x != 0)
			Player.AnimatedSprite.Play("walk");
		else
			Player.AnimatedSprite.Play("idle");

		if (Player.IsFalling())
			Player.AnimatedSprite.Play("jump_fall");

		Player.AnimatedSprite.FlipH = Player.MoveDir.x < 0; // flip sprite if moving left
	}

	public override void UpdateGroundWalking(float delta)
	{
		Player.AnimatedSprite.SpeedScale = 1.0f;
	}

	public override void UpdateGroundSprinting(float delta)
	{
		Player.AnimatedSprite.SpeedScale = 1.5f;
	}

	public override void Jump()
	{
		Player.AnimatedSprite.Play("jump_start");
	}

	public override void Died()
	{
		Player.HaltPlayerLogic = true;
		Player.LevelScene.Camera.StopFollowingPlayer();
		Player.AnimatedSprite.Stop();

		var dieStartPos = Player.Position.y;
		var goUpDuration = 1.25f;

		// animate y position
		Player.DieTween.InterpolateProperty
		(
			"position:y",
			dieStartPos - 80,
			goUpDuration,
			0 // delay
		);

		Player.DieTween.InterpolateProperty
		(
			"position:y",
			dieStartPos + 400,
			1.5f,
			goUpDuration, // delay
			true
		)
		.From(dieStartPos - 80);

		// animate rotation
		Player.DieTween.InterpolateProperty
		(
			"rotation",
			Mathf.Pi,
			1.5f,
			goUpDuration, // delay
			true
		);

		Player.DieTween.Start();
		Player.DieTween.Callback(() => OnDieTweenCompleted());
	}

	public override async void FinishedLevel()
	{
		Player.HaltPlayerLogic = true;
		await LevelManager.CompleteLevel(LevelManager.CurrentLevel);
		Player.HaltPlayerLogic = false;
	}

	private async void OnDieTweenCompleted()
	{
		await GameManager.Transition.AlphaToBlack();
		await Task.Delay(1000);
		GameManager.LevelUI.ShowLives();
		await Task.Delay(1750);
		GameManager.LevelUI.RemoveLife();
		await Task.Delay(1000);
		await GameManager.LevelUI.HideLivesTransition();
		await Task.Delay(250);
		GameManager.Transition.BlackToAlpha();
		Player.HaltPlayerLogic = false;
		LevelManager.LoadLevelFast();
		Player.LevelScene.Camera.StartFollowingPlayer();
	}
}
