namespace Sankari;

public class PlayerAnimationDash : EntityAnimation<Player>
{
	public PlayerAnimationDash(Player player) : base(player) => Entity = player;

	public override void Update()
	{
		FlipSpriteOnDirection();
	}

	public override void HandleTransitions()
	{
		// Dash -> Idle
		// Dash -> JumpFall
		// Dash -> Walking
		// Dash -> Running

		if (!((PlayerCommandDash)Entity.Commands[PlayerCommandType.Dash]).CurrentlyDashing)
			if (!Entity.IsNearGround())
				if (Entity.Velocity.Y > 0)
					SwitchState(EntityAnimationType.JumpFall);
				else
				if (Entity.MoveDir.X != 0)
					if (Entity.PlayerInput.IsSprint)
						SwitchState(EntityAnimationType.Running);
					else
						SwitchState(EntityAnimationType.Walking);
				else
					SwitchState(EntityAnimationType.Idle);
			else
				// entity is touching the ground
				SwitchState(EntityAnimationType.Idle);
	}
}
