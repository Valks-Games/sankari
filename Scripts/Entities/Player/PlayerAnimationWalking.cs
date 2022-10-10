using System;
using static Sankari.Player;

namespace Sankari;

public class PlayerAnimationWalking : PlayerAnimation
{
	public PlayerAnimationWalking(Player player) : base(player) { }

	public override void EnterState()
	{
		Player.AnimatedSprite.Play("walk");
	}

	public override void UpdateState()
	{
		FlipSpriteOnDirection();

		// Walking -> Idle
		// Walking -> Running
		// Walking -> Dash
		// Walking -> JumpStart

		if (Player.PlayerInput.IsJump)

			Transition(Player.AnimationJumpStart);

		else if (Player.PlayerInput.IsDash)

			Transition(Player.AnimationDash);

		else if (Player.PlayerInput.IsSprint)

			Transition(Player.AnimationRunning, 1.5f);

		else if (Player.MoveDir == Vector2.Zero)

			Transition(Player.AnimationIdle);

	}
}
