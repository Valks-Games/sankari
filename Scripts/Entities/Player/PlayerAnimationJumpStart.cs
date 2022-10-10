using static Sankari.Player;

namespace Sankari;

public class PlayerAnimationJumpStart : PlayerAnimation
{
	public PlayerAnimationJumpStart(Player player) : base(player) { }

	public override void EnterState()
	{
		Player.AnimatedSprite.Play("jump_start");
	}

	public override void UpdateState()
	{
		FlipSpriteOnDirection();

		// JumpStart -> Idle
		// JumpStart -> JumpFall
		// JumpStart -> Dash

		if (Player.IsFalling())
			Transition(Player.AnimationJumpFall);
		else if (Player.PlayerInput.IsDash)
			Transition(Player.AnimationDash);
		else if (Player.IsOnGround() && Player.MoveDir == Vector2.Zero)
			Transition(Player.AnimationIdle);
	}
}
