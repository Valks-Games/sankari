namespace Sankari;

public class EntityAnimationIdle : EntityAnimation<MovingEntity>
{
	public EntityAnimationIdle(MovingEntity entity) : base(entity) { }

	public override void EnterState()
	{
		Entity.AnimatedSprite.Play("idle");
		// Idle animation should only occur if nothing else interesting is happening
		// Transition immediately to see if there is a better animation to be in
		HandleStateTransitions();

		Entity.Jump += OnJump;
	}

	private void OnJump(object sender, EventArgs e) 
	{
		
	}

	public override void UpdateState()
	{
		
	}

	public override void HandleStateTransitions()
	{
		// Idle -> Walking
		// Idle -> Sprinting
		// Idle -> JumpStart
		// Idle -> JumpFall

		if (Entity.IsNearGround() && Entity is Player player)
		{
			if (player.PlayerInput.IsJump)
				SwitchState(EntityAnimationType.JumpStart);

			if (Entity.MoveDir != Vector2.Zero)
				if (player.PlayerInput.IsSprint)
					SwitchState(EntityAnimationType.Running);
				else
					SwitchState(EntityAnimationType.Walking);
		}
		else if (!Entity.IsNearGround() && Entity.IsFalling())
			SwitchState(EntityAnimationType.JumpFall);
		else if (!Entity.IsNearGround() && Entity.Velocity.y != 0)
			SwitchState(EntityAnimationType.JumpStart);
	}

	public override void ExitState()
	{
		Entity.Jump -= OnJump;
	}
}
