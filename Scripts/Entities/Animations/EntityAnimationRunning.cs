namespace Sankari;

public class EntityAnimationRunning<T> : EntityAnimation<T> where T : IEntityAnimation
{
	public EntityAnimationRunning(T entity) : base(entity) { }

	protected override void EnterState()
	{
		Player.AnimatedSprite.Play("walk");
		Player.AnimatedSprite.SpeedScale = 1.5f;
	}

	public override void UpdateState()
	{
		FlipSpriteOnDirection();
	}

	public override void HandleStateTransitions()
	{
		// Running -> Idle
		// Running -> Walking
		// Running -> Dash
		// Running -> JumpStart

		if (Player.PlayerInput.IsJump)

			SwitchState(Player.AnimationJumpStart);

		else if (Player.PlayerInput.IsDash)

			SwitchState(Player.AnimationDash);

		else if (!Player.PlayerInput.IsSprint)

			SwitchState(Player.AnimationWalking);

		else if (Player.MoveDir == Vector2.Zero)

			SwitchState(Player.AnimationIdle);
	}

	protected override void ExitState()
	{
		Player.AnimatedSprite.SpeedScale = 1.0f;
	}
}
