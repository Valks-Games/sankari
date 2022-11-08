namespace Sankari;

public class PlayerAnimationJumpStart : EntityAnimation<MovingEntity>
{
	private GTimer TimerDontCheckOnGround;

	public PlayerAnimationJumpStart(Player player) : base(player) { }

	public override void EnterState()
	{
		TimerDontCheckOnGround = Entity.Timers.CreateTimer(100);
		TimerDontCheckOnGround.Loop = false;

		Entity.AnimatedSprite.Play("jump_start");
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

		if (Entity.IsFalling())
			SwitchState(EntityAnimationType.JumpFall);
		else if
		(
			Entity is Player player &&
			player.PlayerInput.IsDash &&
			player.GetCommandClass<PlayerCommandDash>(PlayerCommandType.Dash).DashReady
		)
			SwitchState(EntityAnimationType.Dash);
		else if (Entity.IsNearGround() && Entity.MoveDir == Vector2.Zero && !TimerDontCheckOnGround.IsActive())
			SwitchState(EntityAnimationType.Idle);
	}

	public override void ExitState()
	{

	}
}
