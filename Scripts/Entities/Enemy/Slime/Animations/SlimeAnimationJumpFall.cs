﻿namespace Sankari;

public class SlimeAnimationJumpFall : EntityAnimation<Slime>
{
	public SlimeAnimationJumpFall(Slime entity) : base(entity) { }

	public override void EnterState()
	{
		Entity.AnimatedSprite.Play("jump_fall");
	}

	public override void ExitState()
	{
		
	}

	public override void HandleStateTransitions()
	{
		// JumpFall -> Idle
		// JumpFall -> JumpStart

		if (Entity.IsNearGround())
			SwitchState(EntityAnimationType.Idle);
		else if (!Entity.IsFalling() && Entity.Velocity.y != 0)
			SwitchState(EntityAnimationType.JumpStart);
	}

	public override void UpdateState()
	{
		
	}
}
