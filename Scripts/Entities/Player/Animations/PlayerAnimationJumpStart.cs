namespace Sankari;

public class PlayerAnimationJumpStart : EntityAnimationJumpStart<MovingEntity>
{
	private Player Player { get; set; }

	public PlayerAnimationJumpStart(Player player) : base(player) => Player = player;

	public override void Update()
	{
		FlipSpriteOnDirection();
	}

	public override void HandleTransitions()
	{
		// JumpStart -> Idle
		// JumpStart -> JumpFall
		// JumpStart -> Dash

		base.HandleTransitions();

		if
		(
			Player.PlayerInput.IsDash &&
			Player.GetCommandClass<PlayerCommandDash>(PlayerCommandType.Dash).DashReady
		)
			SwitchState(EntityAnimationType.Dash);
	}
}
