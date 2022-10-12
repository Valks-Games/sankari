﻿namespace Sankari;

public class EntityAnimationIdle : EntityAnimation<IEntityAnimation>
{
	public EntityAnimationIdle(IEntityAnimation entity) : base(entity)
	{
	}

	public override void EnterState()
	{
		Entity.AnimatedSprite.Play("idle");
	}

	public override void UpdateState()
	{
	}

	public override void HandleStateTransitions()
	{
		// Idle -> Walking Idle -> Sprinting Idle -> JumpStart Idle -> JumpFall

		if (Entity is Player player)
		{
			if (player.IsOnGround())
			{
				if (player.PlayerInput.IsJump)
					SwitchState(Entity.AnimationJumpStart);

				if (Entity.MoveDir != Vector2.Zero)
					if (player.PlayerInput.IsSprint)
						SwitchState(Entity.AnimationRunning);
					else
						SwitchState(Entity.AnimationWalking);
			}
			else
			if (player.IsFalling())
				SwitchState(Entity.AnimationJumpFall);
		}
	}

	public override void ExitState()
	{
	}
}
