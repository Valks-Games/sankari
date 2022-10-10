using static Sankari.Player;

namespace Sankari;

public class PlayerAnimationRunning : PlayerAnimation
{
	public PlayerAnimationRunning(Player player) : base(player) { }

	public override void EnterState()
	{
		Player.AnimatedSprite.Play("walk");
		Player.AnimatedSprite.SpeedScale = 1.5f;
	}

	public override void UpdateState()
	{
		FlipSpriteOnDirection();

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

	public override void ExitState()
	{
		Player.AnimatedSprite.SpeedScale = 1.0f;
	}
}
