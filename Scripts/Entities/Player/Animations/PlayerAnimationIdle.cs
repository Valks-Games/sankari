namespace Sankari;

public class PlayerAnimationIdle : EntityAnimationIdle<Player>
{
	public PlayerAnimationIdle(Player player) : base(player) { }

	public override void Enter()
	{
		Entity.AnimatedSprite.Play("idle");
		// Idle animation should only occur if nothing else interesting is happening
		// Transition immediately to see if there is a better animation to be in
		HandleTransitions();

		Entity.Jump += OnJump;
	}

	private void OnJump(object sender, EventArgs e)
	{

	}

	public override void HandleTransitions()
	{
		// Idle -> Walking
		// Idle -> Sprinting
		// Idle -> JumpStart
		// Idle -> JumpFall

		base.HandleTransitions();

		if (Entity.IsNearGround())
		{
			if (Entity.PlayerInput.IsJump)
				SwitchState(EntityAnimationType.JumpStart);

			if (Entity.MoveDir != Vector2.Zero)
				if (Entity.PlayerInput.IsSprint)
					SwitchState(EntityAnimationType.Running);
				else
					SwitchState(EntityAnimationType.Walking);
		}
	}

	public override void Exit()
	{
		Entity.Jump -= OnJump;
	}
}
