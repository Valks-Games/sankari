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
			SwitchState(Player.AnimationJumpFall);
		else if (Player.PlayerInput.IsDash)
			SwitchState(Player.AnimationDash);
		else if (Player.IsOnGround() && Player.MoveDir == Vector2.Zero)
			SwitchState(Player.AnimationIdle);
	}

	public override void ExitState()
	{
		
	}
}
