namespace Sankari;

public class PlayerAnimationJumpFall : EntityAnimationJumpFall<Player>
{
	public Player Player { get; set; }

	public PlayerAnimationJumpFall(Player player) : base(player) => Player = player;

	public override void Update()
	{
		FlipSpriteOnDirection();
	}

	public override void HandleTransitions()
	{
		// JumpFall -> Idle
		// JumpFall -> Walking
		// JumpFall -> Running
		// JumpFall -> Dash
		// JumpFall -> JumpStart

		base.HandleTransitions();
			
		if (Player.PlayerInput.IsDash && Entity.GetCommandClass<PlayerCommandDash>(PlayerCommandType.Dash).DashReady)
			SwitchState(EntityAnimationType.Dash);
	}

	public override void HandleTransitionsNearGround()
	{
		if (Entity.MoveDir.x != 0)
			if (Player.PlayerInput.IsSprint)
				SwitchState(EntityAnimationType.Running);
			else
				SwitchState(EntityAnimationType.Walking);
		else
			SwitchState(EntityAnimationType.Idle);
	}
}
