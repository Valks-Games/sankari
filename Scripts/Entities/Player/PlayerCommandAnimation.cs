namespace Sankari;

public class PlayerCommandAnimation : PlayerCommand
{
	public PlayerCommandAnimation(Player player) : base(player) { }

	public override void Update(float delta)
	{
		if (Entity.MoveDir.x != 0)
			Entity.AnimatedSprite.Play("walk");
		else
			Entity.AnimatedSprite.Play("idle");

		if (Entity.IsFalling())
			Entity.AnimatedSprite.Play("jump_fall");

		Entity.AnimatedSprite.FlipH = Entity.MoveDir.x < 0; // flip sprite if moving left
	}

	public override void UpdateGroundWalking(float delta)
	{
		Entity.AnimatedSprite.SpeedScale = 1.0f;
	}

	public override void UpdateGroundSprinting(float delta)
	{
		Entity.AnimatedSprite.SpeedScale = 1.5f;
	}

	public override void Jump()
	{
		Entity.AnimatedSprite.Play("jump_start");
	}

	public override void Died()
	{
		Entity.HaltPlayerLogic = true;
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

	public override async void FinishedLevel()
	{
		Entity.HaltPlayerLogic = true;
		await LevelManager.CompleteLevel(LevelManager.CurrentLevel);
		Entity.HaltPlayerLogic = false;
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
		Entity.HaltPlayerLogic = false;
		LevelManager.LoadLevelFast();
		Entity.LevelScene.Camera.StartFollowingPlayer();
	}
}
