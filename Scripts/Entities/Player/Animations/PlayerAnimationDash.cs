namespace Sankari;

public class PlayerAnimationDash : EntityAnimation<Player>
{
	public PlayerAnimationDash(Player player) : base(player) { }

	public override void EnterState()
	{

	}

	public override void UpdateState()
	{
		FlipSpriteOnDirection();
	}

	public override void HandleStateTransitions()
	{
		// Dash -> Idle
		// Dash -> JumpFall
		// Dash -> Walking
		// Dash -> Running

		if (!((PlayerCommandDash)Entity.Commands[PlayerCommandType.Dash]).CurrentlyDashing)
			if (!Entity.IsNearGround())
				if (Entity.Velocity.y > 0)
					SwitchState(EntityAnimationType.JumpFall);
				else
				if (Entity.MoveDir != Vector2.Zero)
					if (Entity is Player player && player.PlayerInput.IsSprint)
						SwitchState(EntityAnimationType.Running);
					else
						SwitchState(EntityAnimationType.Walking);
				else
					SwitchState(EntityAnimationType.Idle);
			else
				// entity is touching the ground
				SwitchState(EntityAnimationType.Idle);
	}

	public override void ExitState()
	{

	}
}
