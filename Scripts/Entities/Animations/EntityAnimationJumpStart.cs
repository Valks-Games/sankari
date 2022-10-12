namespace Sankari;

public class EntityAnimationJumpStart<T> : EntityAnimation<T> where T : IEntityAnimation
{
	private GTimer TimerDontCheckOnGround;

	public EntityAnimationJumpStart(T entity) : base(entity) { }

	protected override void EnterState()
	{
		TimerDontCheckOnGround = new GTimer(Player, 100, false, true);
		Player.AnimatedSprite.Play("jump_start");
	}

	public override void UpdateState()
	{
		FlipSpriteOnDirection();
	}

	public override void HandleStateTransitions()
	{
		// JumpStart -> Idle
		// JumpStart -> JumpFall
		// JumpStart -> Dash

		if (Player.IsFalling())

			SwitchState(Player.AnimationJumpFall);

		else if (Player.PlayerInput.IsDash)

			SwitchState(Player.AnimationDash);

		else if (Player.IsOnGround() && Player.MoveDir == Vector2.Zero && !TimerDontCheckOnGround.IsActive())

			SwitchState(Player.AnimationIdle);
	}

	protected override void ExitState()
	{

	}
}
