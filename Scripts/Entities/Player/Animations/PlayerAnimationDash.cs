namespace Sankari;

public class PlayerAnimationDash : EntityAnimation<MovingEntity>
{
	public Player Player { get; set; }

	public PlayerAnimationDash(Player player) : base(player) => Player = player;

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
				if (Entity.Velocity.y > 0)
					SwitchState(EntityAnimationType.JumpFall);
				else
				if (Entity.MoveDir != Vector2.Zero)
					if (Player.PlayerInput.IsSprint)
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
