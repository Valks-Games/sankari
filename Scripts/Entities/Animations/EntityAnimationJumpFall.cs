namespace Sankari;

public class EntityAnimationJumpFall : EntityAnimation<IEntityAnimation>
{
	public EntityAnimationJumpFall(IEntityAnimation entity) : base(entity)
	{
	}

	public override void EnterState()
	{
		Entity.AnimatedSprite.Play("jump_fall");
	}

	public override void UpdateState()
	{
		FlipSpriteOnDirection();
	}

	public override void HandleStateTransitions()
	{
		// JumpFall -> Idle JumpFall -> Walking JumpFall -> Running JumpFall -> Dash

		if (Entity is Player player)
		{
			if (player.IsOnGround())
				if (Entity.MoveDir != Vector2.Zero)
					if (player.PlayerInput.IsSprint)
						SwitchState(Entity.AnimationRunning);
					else
						SwitchState(Entity.AnimationWalking);
				else
					SwitchState(Entity.AnimationIdle);
			else if (player.PlayerInput.IsDash)
				SwitchState(Entity.AnimationDash);
		}
	}

	public override void ExitState()
	{
	}
}
