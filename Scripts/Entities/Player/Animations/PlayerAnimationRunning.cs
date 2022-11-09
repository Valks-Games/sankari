namespace Sankari;

public class PlayerAnimationRunning : EntityAnimation<MovingEntity>
{
	public Player Player { get; set; }

	public PlayerAnimationRunning(Player player) : base(player) => Player = player;

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

		if (Player.PlayerInput.IsJump)

			SwitchState(EntityAnimationType.JumpStart);

		else if (Player.PlayerInput.IsDash && Entity.GetCommandClass<PlayerCommandDash>(PlayerCommandType.Dash).DashReady)

			SwitchState(EntityAnimationType.Dash);

		else if (!Player.PlayerInput.IsSprint)

			SwitchState(EntityAnimationType.Walking);

		else if (Entity.MoveDir == Vector2.Zero || Entity.Velocity.y != 0)

			SwitchState(EntityAnimationType.Idle);
	}

	public override void Exit()
	{
		Entity.AnimatedSprite.SpeedScale = 1.0f;
	}
}
