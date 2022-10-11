using static Sankari.Player;

namespace Sankari;

public class PlayerAnimationJumpStart : PlayerAnimation
{
	private GTimer TimerDontCheckOnGround;

	public PlayerAnimationJumpStart(Player player) : base(player) { }

	protected override void EnterState()
	{
		TimerDontCheckOnGround = new GTimer(Player, 100, false, true);
		Player.AnimatedSprite.Play("jump_start");
	}

	public override void UpdateState()
	{
		FlipSpriteOnDirection();

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
