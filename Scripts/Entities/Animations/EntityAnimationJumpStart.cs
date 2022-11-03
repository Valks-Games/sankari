﻿namespace Sankari;

public class EntityAnimationJumpStart : EntityAnimation<MovingEntity>
{
	private GTimer TimerDontCheckOnGround;

	public EntityAnimationJumpStart(MovingEntity entity) : base(entity) { }

	public override void EnterState()
	{
		TimerDontCheckOnGround = Entity.Timers.CreateTimer(new Callable(() => { }), 100, false, true);
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

		if (Entity is Player player)
		{
			if (player.IsFalling())
				SwitchState(EntityAnimationType.JumpFall);

			else if 
			(
				player.PlayerInput.IsDash && 
				player.GetCommandClass<MovingEntityCommandDash>(EntityCommandType.Dash).DashReady
			)
				SwitchState(EntityAnimationType.Dash);

			else if (player.IsNearGround() && Entity.MoveDir == Vector2.Zero && !TimerDontCheckOnGround.IsActive())
				SwitchState(EntityAnimationType.Idle);
		}
	}

	public override void ExitState()
	{
		
	}
}
