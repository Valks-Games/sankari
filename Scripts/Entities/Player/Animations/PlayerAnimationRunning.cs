namespace Sankari;

public class PlayerAnimationRunning : EntityAnimation<Player>
{
	public PlayerAnimationRunning(Player player) : base(player) => Entity = player;

	public override void Enter()
	{
		Entity.AnimatedSprite.Play("walk");
		Entity.AnimatedSprite.SpeedScale = 1.5f;
	}

	public override void Update()
	{
		FlipSpriteOnDirection();
	}

	public override void HandleTransitions()
	{
		// Running -> Idle
		// Running -> Walking
		// Running -> Dash
		// Running -> JumpStart

		if (Entity.PlayerInput.IsJumpJustPressed)

			SwitchState(EntityAnimationType.JumpStart);

		else if (Entity.PlayerInput.IsDash && Entity.GetCommandClass<PlayerCommandDash>(PlayerCommandType.Dash).DashReady)

			SwitchState(EntityAnimationType.Dash);

		else if (!Entity.PlayerInput.IsSprint)

			SwitchState(EntityAnimationType.Walking);

		else if (Entity.MoveDir == Vector2.Zero || Entity.Velocity.Y != 0)

			SwitchState(EntityAnimationType.Idle);
	}

	public override void Exit()
	{
		Entity.AnimatedSprite.SpeedScale = 1.0f;
	}
}
