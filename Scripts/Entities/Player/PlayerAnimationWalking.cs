﻿using System;
using static Sankari.Player;

namespace Sankari;

public class PlayerAnimationWalking : PlayerAnimation
{
	public PlayerAnimationWalking(Player player) : base(player) { }

	protected override void EnterState()
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

			SwitchState(Player.AnimationJumpStart);

		else if (Player.PlayerInput.IsDash)

			SwitchState(Player.AnimationDash);

		else if (Player.PlayerInput.IsSprint)

			SwitchState(Player.AnimationRunning);

		else if (Player.MoveDir == Vector2.Zero)

			SwitchState(Player.AnimationIdle);
	}

	protected override void ExitState()
	{
		
	}
}